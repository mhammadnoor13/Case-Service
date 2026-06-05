using Application.Dtos;
using Application.Events;
using Application.Helpers;
using Application.Interfaces;
using CaseService.API.CaseService.Application.Dtos;
using CaseService.API.CaseService.Application.Interfaces;
using CaseService.API.CaseService.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CaseService.API.CaseService.Application.Services
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _repo;
        private readonly IEmbeddingClient _embeddingClient;
        private readonly IMailService _mailService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICaseEventPublisher _caseEventPublisher;
        private readonly ILogger<CaseService> _logger;


        public CaseService(
            ICaseRepository repo,
            IPublishEndpoint publishEndpoint,
            ILogger<CaseService> logger,
            IEmbeddingClient embeddingClient,
            IMailService mailService)
        {
            _repo = repo;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _embeddingClient = embeddingClient;
            _mailService = mailService;
        }

        public async Task<Guid> SubmitCaseAsync(
            string email,
            string title,
            string description,
            string speciality,
            CancellationToken ct)
        {
            var c = Case.Create(email, title, description, speciality);

            await _repo.SaveAsync(c, ct);

            await _caseEventPublisher.PublishCaseSubmittedAsync(c.Id, c.Speciality, ct);

            c.MoveToAssigned();

            await _repo.SaveAsync(c, ct);
            
            return c.Id;
        }

        public async Task MoveToInReviewAsync(Guid id, CancellationToken ct)
        {
            var c = await _repo.GetByIdAsync(id, ct)
                   ?? throw new KeyNotFoundException($"Case {id} not found");

            c.MoveToReview();

            await _repo.SaveAsync(c, ct);

            // TODO Publish the “InReview” integration event - not necessary now
            //await _events.PublishCaseInReviewAsync(id, ct);
        }

        public async Task FinishCaseAsync(Guid id, CancellationToken ct)
        {
            var c = await _repo.GetByIdAsync(id, ct)
                   ?? throw new KeyNotFoundException($"Case {id} not found");

            c.Finish();

            await _repo.SaveAsync(c, ct);

            // TODO Publish the “Finished” integration event
            //await _events.PublishCaseFinishedAsync(id, ct);
        }

        public async Task<CaseDto?> GetCaseByIdAsync(Guid id, CancellationToken ct)
        {
            var c = await _repo.GetByIdAsync(id, ct);
            if (c is null) return null;
            return ToDto(c);
        }

        private static CaseDto ToDto(Case c)
            => new CaseDto(
                c.Id,
                c.Title,
                c.Description,
                c.Status,
                c.CreatedAt,
                c.Suggestions);

        public async Task<IEnumerable<CaseDto>> GetCasesBySpecialityAsync(string speciality, CancellationToken ct)
        {
            var cases = await _repo.GetBySpecialityAsync(speciality, ct);

            return cases.Select(c => new CaseDto(
                c.Id,
                c.Title,
                c.Description,
                c.Status,
                c.CreatedAt,
                c.Suggestions));
        }

        public async Task<IEnumerable<CaseToCardDto>> GetCasesByIdsAsync(IEnumerable<Guid> caseIds, CancellationToken ct)
        {
            const int BriefLength = 100;
            var cases = await _repo.GetBulkByIdsAsync(caseIds, ct);
            
            var dtos = cases.Select(c=> new CaseToCardDto(
                c.Id,
                c.Title, 
                c.Description.Truncate(BriefLength),
                c.Status,
                c.CreatedAt)).ToList();

            return dtos;
        }

        public async Task AddSuggestionsAsync(AddSuggestionsRequest request, CancellationToken ct)
        {
            var c = await _repo.GetByIdAsync(request.CaseId, ct)
                 ?? throw new KeyNotFoundException($"Case {request.CaseId} not found");

            c.AddSuggestions(request.Suggestions);

            await _repo.SaveAsync(c,ct);
        }

        public async Task AddSolutionAsync(Guid caseId, string solution, Guid consultantId, CancellationToken ct)
        {
            var cs = await _repo.GetByIdAsync(caseId, ct)
                 ?? throw new KeyNotFoundException($"Case {caseId} not found");

            cs.SetSolution(solution);
            await _repo.SaveAsync(cs, ct);

            var embedOk = await _embeddingClient.EmbedCaseAsync(solution, consultantId, ct);

            if (!embedOk)
                throw new InvalidOperationException("Embedding service failed.");

            var sendMailRequest = new SendMailRequest(cs.Email, "Solved", solution);
            _logger.LogInformation(sendMailRequest.Recipient);

            await _mailService.SendSolutionMailAsync(sendMailRequest, ct);

        }
    }
}
