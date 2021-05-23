using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Signaler.Models.Home
{
    public class LoginViewModel
    {
        [Required]
        [FromForm]
        public string Username { get; set; }

        [Required]
        [FromForm]
        public string Password { get; set; }
    }
}