using Domain.Entities;

namespace CaseService.API.CaseService.Application.Dtos
{
    public class CaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Status { get; set; } = "Submitted";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Suggestion>? Suggestions { get; set; } = new List<Suggestion>();
        public CaseDto(Guid id, string title, string description, string status, DateTime createdAt, List<Suggestion> sugs)
        {
            Id = id;
            Title = title;
            Description = description;
            Status = status;
            CreatedAt = createdAt;
            Suggestions = sugs;

        }
    }
}
