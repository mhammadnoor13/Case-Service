# Case-Service

## Responsibility
The Case Service manages user-submitted cases. It handles case submission, persistence, status tracking, consultant solutions, and case-related event publishing. It also calls the Embedding Service over HTTP to embed solution/case content when needed.

## Tech Stack
- ASP.NET Core Web API
- C#
- MongoDB
- RabbitMQ
- MassTransit
- MailKit / MimeKit
- HttpClient 
- Swagger / OpenAPI
- Docker / Docker Compose

## Architecture

The Case Service follows Clean Architecture principles. The code is separated into the following layers:

- Presentation/API: exposes HTTP endpoints through ASP.NET Core controllers.
- Application: contains use cases, service interfaces, and application logic.
- Domain: contains the `Case` entity and domain behavior.
- Infrastructure: contains technical implementations such as MongoDB repositories, RabbitMQ event publishing, HTTP clients, and mail clients.

The Application layer depends on abstractions such as `ICaseRepository`, `ICaseEventPublisher`, and `IEmbeddingClient`. The Infrastructure layer implements these abstractions using MongoDB, MassTransit/RabbitMQ, and HttpClient.

## Main Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/cases` | Submits a new case. |
| `GET` | `/cases/{id}` | Retrieves a case by ID. |
| `POST` | `/cases/{caseId:guid}/add-solution` | Adds a consultant solution to a case. |
| `POST` | `/cases/{caseId:guid}/add-suggestions` | Adds AI draft suggestions to a case. |

## Main Components
- CasesController: exposes HTTP endpoints for case operations.
- CaseService: application service that coordinates case use cases.
- ICaseRepository: application abstraction for case persistence.
- MongoCaseRepository: MongoDB implementation of case persistence.
- ICaseEventPublisher: abstraction for publishing case events.
- RabbitMqCaseEventPublisher: MassTransit/RabbitMQ implementation of event publishing.
- IEmbeddingClient: abstraction for calling the Embedding Service.
- EmbeddingClient: HTTP client used to send case text to the Embedding Service.

## Data Flow
### Submit Case Flow

1. The client sends a `POST /cases` request.
2. `CasesController` receives the request and passes the data to `ICaseService`.
3. `CaseService` creates a new `Case` domain entity.
4. The case is saved using `ICaseRepository`.
5. A `CaseSubmittedEvent` is published through `ICaseEventPublisher`.
6. The case status is updated and saved again.
7. The API returns `201 Created` with the created case ID.

### Add Solution Flow

1. The consultant sends a `POST /cases/{caseId:guid}/add-solution` request.
2. The controller reads the `caseId` from the route.
3. The controller reads the solution text from the request body.
4. The controller reads the authenticated consultant ID from the `X-User-Id` header.
5. `CaseService` loads the case using `ICaseRepository`.
6. The solution is added to the case.
7. The Case Service calls the Embedding Service over HTTP to embed the consultant solution and associate it with the consultant.
8. The case is saved again with the added solution and updated status.
9. An email containing the case information and the consultant solution is sent to the client.
10. The API returns `204 No Content`.

### Add AI Suggestions Flow

1. The AI Service sends a `POST /cases/{caseId:guid}/add-suggestions` request.
2. The controller reads the `caseId` from the route.
3. The controller reads the AI-generated draft suggestions from the request body.
4. `CaseService` loads the case using `ICaseRepository`.
5. The AI draft suggestions are added to the case.
6. The case is saved with the added suggestions and updated status.
7. The API returns `204 No Content`.

## Communication With Other Services

| Service | Communication Type | Purpose |
|---|---|---|
| API Gateway  | HTTP Header | Provides the authenticated user ID through `X-User-Id`. |
| Embedding Service | HTTP Request | Embeds consultant solutions and associates them with the consultant. |
| AI Service | HTTP Request | Sends AI draft suggestions to be added to a case. |
| RabbitMQ / MassTransit | Message Broker | Publishes case-related events. |
| Consultant Service | RabbitMQ Consumer | Consumes `CaseSubmittedEvent` to react to new submitted cases. |
| Mail Service / SMTP Provider | HTTP/SMTP | Sends the final case and solution email to the client. |

## Environment Variables

The Case Service uses environment variables to configure external dependencies such as MongoDB, RabbitMQ, and the Embedding Service. These values should be changed depending on the execution environment.

For local Docker Compose execution, service names such as `mongo`, `rabbitmq`, and `embedding` can be used as hostnames. For local execution without Docker, these values may need to be changed to `localhost`.


