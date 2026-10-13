using System.ComponentModel.DataAnnotations;

namespace HDNXUdemyModel.RequestModel
{
    public class LoginWithGoogle
    {
        public string? Provider { get; set; }
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public string? AuthToken { get; set; }

        [Required]
        public string? IdToken { get; set; }

        public string? AuthorizationCode { get; set; }
        public dynamic? response { get; set; }
    }
}