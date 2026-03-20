using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Users
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } 

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmNewPassword { get; set; }
    }
}
