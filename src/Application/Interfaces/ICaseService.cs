using Application.Dtos;
using CaseService.API.CaseService.Application.Dtos;
using Contracts.Shared.Responses;

namespace CaseService.API.CaseService.Application.Interfaces
{
    public interface ICaseService
    {
        Task<Guid> SubmitCaseAsync(string email, string title, string description, string speciality, CancellationToken ct);
        Task MoveToInReviewAsync(Guid id, CancellationToken ct);
        Task FinishCaseAsync(Guid id, CancellationToken ct);
        Task<CaseDto?> GetCaseByIdAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<CaseToCardDto>> GetCasesByIdsAsync(IEnumerable<Guid> caseIds, CancellationToken ct);
        Task<IEnumerable<CaseDto>> GetCasesBySpecialityAsync(String speciality, CancellationToken ct);
        Task AddSuggestionsAsync (Guid id, List<string> suggestions, CancellationToken ct);
        Task AddSolutionAsync (Guid caseId, string solution, Guid consultantId, CancellationToken ct);

    }
}
