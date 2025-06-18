using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "姓名為必填")]
        [StringLength(50, ErrorMessage = "姓名長度不可超過 50 字元")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email 為必填")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "密碼至少需要 6 字元")]
        public string Password { get; set; }

        [Required(ErrorMessage = "請再次輸入密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "兩次密碼輸入不一致")]
        public string ConfirmPassword { get; set; } // ❌ 不存入資料庫

        public string? Phone { get; set; } // ✅ 可選欄位（Nullable）

        public string? Gender { get; set; } // ✅ 可選欄位（Nullable）
    }
}
