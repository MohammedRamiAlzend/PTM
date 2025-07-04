namespace PTM.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class ProjectController(ISender sender) : ControllerBase
{
    [HttpGet(ProjectsEndPoint.GetAll)]
    public async Task<ActionResult<ApiResponse<List<ProjectResponseDto>>>> GetAllProjectsAsync()
    {
        return await sender.Send(new GetAllProjectsQuery());
    }

    [HttpPost(ProjectsEndPoint.Create)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> CreateProjectAsync([FromForm] CreateProjectDto project)
    {
        return await sender.Send(new CreateProjectCommand(project));
    }

}