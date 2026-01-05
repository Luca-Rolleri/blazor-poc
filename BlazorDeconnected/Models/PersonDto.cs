using System.ComponentModel.DataAnnotations;

namespace BlazorDeconnected.Models
{
    public class PersonDto
    {
        [Required]
        public string Name { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }
}
