
using Microsoft.AspNetCore.Authorization;

namespace PTM.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class ProjectController(ISender sender) : ControllerBase
{
    [HttpGet(ProjectsEndPoint.GetAll)]
    [Authorize(Roles ="Admin,User")]
    public async Task<ActionResult<ApiResponse<List<ProjectResponseDto>>>> GetAllProjectsAsync( CancellationToken token)
    {
        return await sender.Send(new GetProjectsQuery(),token);
    }

    [HttpPost(ProjectsEndPoint.Create)]
    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> CreateProjectAsync([FromForm] CreateProjectDto project, CancellationToken token)
    {
        return await sender.Send(new CreateProjectCommand(project),token);
    }

}