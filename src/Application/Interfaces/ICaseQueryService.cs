using CaseService.API.CaseService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICaseQueryService
    {
        Task<IReadOnlyList<CaseDto>> GetCasesByIdsAsync(IEnumerable<Guid> caseIds);

    }
}
