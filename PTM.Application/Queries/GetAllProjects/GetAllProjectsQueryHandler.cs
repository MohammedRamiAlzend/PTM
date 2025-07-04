using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.Queries.GetAllProjects
{
    public record GetAllProjectsQuery():IRequest<ApiResponse<List<ProjectResponseDto>>>;
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, ApiResponse<List<ProjectResponseDto>>>
    {
        public async Task<ApiResponse<List<ProjectResponseDto>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
