using API.Models.Requests;
using API.Models.Responses;
using Application.Dtos;
using Application.Interfaces;
using CaseService.API.CaseService.Application.Dtos;
using CaseService.API.CaseService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace CaseService.Api.Controllers
{
    [ApiController]
    [Route("cases")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly IMailService _mailService;
        private readonly ILogger<CasesController> _logger;

        public CasesController(ICaseService caseService, IMailService mailService, ILogger<CasesController> logger)
        {
            _caseService = caseService;
            _mailService = mailService;
            _logger = logger;
        }


        [HttpPost]
        [ProducesResponseType(typeof(CreateCaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitCase(
            [FromBody] CreateCaseRequest req,
            CancellationToken ct)
        {
            _logger.LogInformation("SubmitCase called for {Email}, speciality={Speciality}", req.Email, req.Speciality);

            var caseId = await _caseService.SubmitCaseAsync(
                req.Email,req.Title, req.Description, req.Speciality, ct);

            var response = new CreateCaseResponse(caseId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = caseId },
                response);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CaseDto>> GetById(
            Guid id,
            CancellationToken ct)
        {
            var dto = await _caseService.GetCaseByIdAsync(id, ct);
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> GetByIds([FromBody] List<Guid> caseIds, CancellationToken ct)
        {
            if (caseIds == null || !caseIds.Any())
                return BadRequest("Must supply at least one caseId.");

            var cases = await _caseService.GetCasesByIdsAsync(caseIds,ct);
            return Ok(cases);
        }


        [HttpGet("speciality/{speciality}")]
        public async Task<ActionResult<IEnumerable<CaseDto>>> GetBySpeciality(
            string speciality,
            CancellationToken ct)
        {
            var list = await _caseService.GetCasesBySpecialityAsync(speciality, ct);
            return Ok(list);
        }

        [HttpPost("{caseId:guid}/add-solution")]
        public async Task<IActionResult> AddSolution(
            Guid caseId,
            [FromBody] string solution,
            CancellationToken ct)
        {
            if (!Guid.TryParse(Request.Headers["X-User-Id"], out var consultantId))
                return Unauthorized();

            await _caseService.AddSolutionAsync(caseId, solution, consultantId, ct);
            return NoContent();
        }
       
        [HttpPost("{id:guid}/inreview")]
        public async Task<IActionResult> MoveToInReview(
            Guid id,
            CancellationToken ct)
        {
            await _caseService.MoveToInReviewAsync(id, ct);
            return NoContent();  // 204 No Content
        }


        [HttpPost("{id:guid}/finish")]
        public async Task<IActionResult> FinishCase(
            Guid id,
            CancellationToken ct)
        {
            await _caseService.FinishCaseAsync(id, ct);
            return NoContent();
        }


        [HttpPost("{id:guid}/add-suggestions")]
        public async Task<IActionResult> FinishCase(
            Guid id,
            [FromBody] CaseSuggestionsDto req,
            CancellationToken ct)
        {

            var command = new AddSuggestionsRequest(
                CaseId: id,
                Suggestions: req.suggestions.Select(s => s.text)
                );

            await _caseService.AddSuggestionsAsync(command, ct);
            return NoContent();
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail(
            SendMailRequest sendMailRequest,
            CancellationToken ct
            )
        {

            await _mailService.SendSolutionMailAsync(sendMailRequest, ct);
            return Ok("Email sent successfully");
        }
        /*
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteCase(
			Guid id,
			CancellationToken ct)
		{
			await _caseService.DeleteCaseAsync(id, ct);
			return NoContent();
		}
		*/
    }



}
