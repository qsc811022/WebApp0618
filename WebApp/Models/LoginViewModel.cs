using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "請輸入 Email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
