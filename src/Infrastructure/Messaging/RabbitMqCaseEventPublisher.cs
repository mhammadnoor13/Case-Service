using Application.Events;
using CaseService.API.CaseService.Application.Interfaces;
using MassTransit;

namespace CaseService.API.CaseService.Infrastructure.Messaging
{
    public class RabbitMqCaseEventPublisher : ICaseEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMqCaseEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishCaseSubmittedAsync(Guid caseId, string speciality, CancellationToken ct)
        {
            var caseSubmittedEvent = new CaseSubmittedEvent(caseId, speciality);
            return _publishEndpoint.Publish(caseSubmittedEvent,ct);
        }

        public Task PublishCaseFinishedAsync(Guid caseId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task PublishCaseInReviewAsync(Guid caseId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
