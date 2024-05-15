using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    [AllowAnonymous]
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User Name/Login is required")]
        [MaxLength(255)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        // [MaxLength(8)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

    }
}
