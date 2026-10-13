using System.ComponentModel.DataAnnotations;

namespace HDNXUdemyModel.RequestModel
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reset code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Reset code must be 6 digits.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
