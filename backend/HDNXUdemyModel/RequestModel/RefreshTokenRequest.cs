using System.ComponentModel.DataAnnotations;

namespace HDNXUdemyModel.RequestModel
{
    public class RefreshTokenRequest
    {
        [Required]
        public string? RefreshToken { get; set; }
    }
}
