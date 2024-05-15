using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

using System.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace ResourceAllocationTool
{
    public class ExceptionHandlerMiddleware
    {
        #region variables

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        private const string General_Error = "An Unexpected  error has occurred. Please contact the system administrator.";

        #endregion

        #region constructor

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }
        #endregion

        #region Methods
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var _request = context.Request;

                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Handle errors @ each request
        /// </summary>
        /// <param name="oHttpContext"></param>
        /// <param name="oException">application error</param>
        /// <returns></returns>
        private async Task HandleExceptionMessageAsync(HttpContext oHttpContext, Exception oException)
        {
            try
            {
                string sErrorMsg;

                if (oException.InnerException == null)    //Message specified @ request,unhandled exeption being thrown 
                {
                    sErrorMsg = oException.ToString();
                }
                else
                {
                    sErrorMsg = oException.InnerException.ToString();
                }

                //log error 
                _logger.LogError(sErrorMsg);

                int statusCode = (int)HttpStatusCode.InternalServerError;
                var sMessage = oException.Message.Replace("\r\n", " ");

                sMessage = General_Error;

                //type of  exption thrown:
                switch (oException)
                {
                    case SqlException     //any database specific sql ex, e.g. network error connecting
                        AmbiguousMatchException:
                        break;
                    case BadHttpRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        sMessage = oException.Message;
                        break;
                    case DbUpdateException:
                        break;
                }

                var errorResponse = new { Status = statusCode, Message = sMessage };
                var oResponse = oHttpContext.Response;

                oResponse.ContentType = "application/json";
                oResponse.StatusCode = statusCode;

                //write  out error message
                sMessage = JsonConvert.SerializeObject(errorResponse);
                await oResponse.WriteAsync(sMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }




        #endregion
    }
}
