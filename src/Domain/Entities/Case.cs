using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;
using System.Net.Mail;

namespace CaseService.API.CaseService.Domain.Entities
{
    
    public class Case
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Email { get; set; } = default!;
        public required string Title { get; set; } = default!;
        public required string Description { get; set; } = default!;
        public required string Speciality { get; set; } = default!;
        public string Status { get; set; } = "Submitted";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Suggestion>? Suggestions { get; set; } = new List<Suggestion>();
        public string Solution { get; set; } = default!;

        private Case() { }

        #region Factory
        public static Case Create(string email, string title, string description, string speciality)
        {
            // 1) Basic null/empty checks
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email is required.");
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Title is required.");
            if (string.IsNullOrWhiteSpace(description))
                throw new Exception("Description is required.");
            if (string.IsNullOrWhiteSpace(speciality))
                throw new Exception("Speciality is required.");

            try
            {
                var addr = new MailAddress(email.Trim());
                email = addr.Address; // normalizes the address
            }
            catch
            {
                throw new Exception("Email format is invalid.");
            }

            return new Case
            {
                Id = Guid.NewGuid(),
                Title = title,
                Email = email,
                Description = description,
                Speciality = speciality
            };
        }

        #endregion

        public void AddSuggestion (string suggestion)
        {
            Suggestion sug = new Suggestion { text = suggestion };
            Suggestions.Add(sug);
        }

        public void AddSuggestions (IEnumerable<string> suggestions)
        {
            foreach (var s in suggestions) AddSuggestion(s);

        }

        public void SetSolution (string solution)
        {
            Solution = solution;
        }

        

        #region Status Transitions
        public void MoveToInReview()
        {
            if (Status != "Submitted")
                throw new Exception("Can only move submitted cases to InReview.");
            Status = "InReview";
        }

        public void Finish()
        {
            if (Status == "Finished")
                throw new Exception("Case is already Finished.");
            if (Status != "InReview")
                throw new Exception("Can only finish cases that are InReview.");
            Status = "Finished";
        }
        #endregion
    }

}
