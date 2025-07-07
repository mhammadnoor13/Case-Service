using API.Models.Requests;
using API.Models.Responses;
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

        public CasesController(ICaseService caseService)
        {
            _caseService = caseService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(CreateCaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitCase(
            [FromBody] CreateCaseRequest req,
            CancellationToken ct)
        {
            var caseId = await _caseService.SubmitCaseAsync(
                req.Email, req.Description, req.Speciality, ct);

            var response = new CreateCaseResponse(caseId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = caseId },
                response);
        }

        // 2) Get a single Case by Id
        // GET /api/cases/{id}
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

        // 3) List all Cases in a given speciality
        // GET /api/cases/speciality/{speciality}

        [HttpGet("speciality/{speciality}")]
        public async Task<ActionResult<IEnumerable<CaseDto>>> GetBySpeciality(
            string speciality,
            CancellationToken ct)
        {
            var list = await _caseService.GetCasesBySpecialityAsync(speciality, ct);
            return Ok(list);
        }

        // 4) Transition a Case to InReview
        // POST /api/cases/{id}/inreview
        [HttpPost("{id:guid}/inreview")]
        public async Task<IActionResult> MoveToInReview(
            Guid id,
            CancellationToken ct)
        {
            await _caseService.MoveToInReviewAsync(id, ct);
            return NoContent();  // 204 No Content
        }

        // 5) Finish a Case
        // POST /api/cases/{id}/finish
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
            [FromBody] CaseSuggestions req,
            CancellationToken ct)
        {
            await _caseService.AddSuggestionsAsync(id,req.suggestions, ct);
            return NoContent();
        }


        /*
		// 6) Delete a Case
		// DELETE /api/cases/{id}
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
