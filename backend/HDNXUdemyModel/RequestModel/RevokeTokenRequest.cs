using System.ComponentModel.DataAnnotations;

namespace HDNXUdemyModel.RequestModel
{
    public class RevokeTokenRequest
    {
        [Required]
        public string? RefreshToken { get; set; }
    }
}
