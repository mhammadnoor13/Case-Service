using System.ComponentModel.DataAnnotations;

namespace API.Models.Requests
{
    public record CreateCaseRequest(
        [Required, EmailAddress] string Email,
        [Required] string Title,
        [Required, StringLength(2000,MinimumLength=10)] string Description,
        [Required] string Speciality
    );
}
