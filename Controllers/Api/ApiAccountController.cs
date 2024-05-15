using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;
using AutoMapper;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ResourceAllocationTool.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ApiAccountController : Controller
    {
        #region variables
        private readonly ILogger<ApiAccountController> _logger;
        private readonly IUserRepository _usrRepository;
        private readonly IMapper _mapper;
        private const int APPLICATION_ERROR = 500;
        #endregion

        public ApiAccountController(IUserRepository usrRepository,IMapper mapper, ILogger<ApiAccountController> logger)
        {
            _logger = logger;
            _usrRepository = usrRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Login.cshtml- btnLogin_Click - @index.js
        /// </summary>
        /// <param name="model">JSON- Login credentials</param>
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            string errMsg;
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            string userName = model.UserName;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(model.Password))
            {
                errMsg = "Credentials not detected";
                return BadRequest(errMsg);
            }

            try
            {
                //create user model
                var oUserModel = await this.AuthenticateUserAsync(model);
                await this.SetCookie(oUserModel);
                return Ok();
            }
            catch (NotFoundException oNotFoundException)
            {
                errMsg = oNotFoundException.Message;
                _logger.LogError($"{userName} - {errMsg}");
                return BadRequest(errMsg);
            }
            catch (Exception oException)
            {
                errMsg = $"{oException.Message} \n @ {oException.StackTrace}";
                _logger.LogError(errMsg);
                return StatusCode(APPLICATION_ERROR, errMsg);
            }

        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="oLoginModel">JSON - UserName/Password</param>
        protected async Task<UserModel> AuthenticateUserAsync(LoginViewModel oLoginModel)
        {

            string sUserName = oLoginModel.UserName;

            UserModel oUserModel = null;

            //LDAP - AD verification
            using (var oPrincipalContext = new PrincipalContext(ContextType.Domain))
            {

                string sLdapUrl = $"LDAP://{oPrincipalContext.ConnectedServer}";
                _logger.LogInformation($"LDAP URL: {sLdapUrl}");

                if (!oPrincipalContext.ValidateCredentials(sUserName, oLoginModel.Password))
                {
                    var msg = "Unknown username and/or password";
                    _logger.LogError($"Authenticate:UserName:{sUserName} - {msg}");
                    throw new NotFoundException(msg);
                }

                using DirectoryEntry oDirectoryEntry = new(sLdapUrl);
                using DirectorySearcher oDirectorySearcher = new DirectorySearcher(oDirectoryEntry);
                oDirectorySearcher.Filter = $"(SAMAccountName={sUserName})";

                oDirectorySearcher.PropertiesToLoad.Add(Constants.LdapAttributes.LastName);
                oDirectorySearcher.PropertiesToLoad.Add(Constants.LdapAttributes.FirstName);

                SearchResult oSearchResult = oDirectorySearcher.FindOne();

                //user info by login
                var result = await _usrRepository.LoadByLoginAsync(sUserName);
                oUserModel = _mapper.Map<UserModel>(result);
            }

            return oUserModel;
        }

        /// <summary>
        /// Set Authentication cookie
        /// </summary>
        /// <param name="oUserModel"></param>
        protected async Task SetCookie(UserModel oUserModel)
        {
            string sUserName = oUserModel.Login;

            var lstClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, sUserName),
                new Claim("FullName", oUserModel.FullName),
                //custom claims - role == permissions
                new Claim("IsSupervisor", (oUserModel.Role.IsSupervisor && !oUserModel.Role.IsAdministrator).ToString()),
                new Claim("IsAdministrator", (oUserModel.Role.IsAdministrator).ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                      lstClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
            };

            //sign user in 
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User {username} logged in @ {Time}.", sUserName, DateTime.Now);


        }

        /// <summary>
        /// Log Out user
        /// </summary>
        [HttpPatch("")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User {Name} logged out @ {Time}.", User.Identity.Name, DateTime.Now);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.Ok();
        }
    }
}
