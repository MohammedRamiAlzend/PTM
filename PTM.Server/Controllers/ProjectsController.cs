using Microsoft.AspNetCore.Authorization;

namespace PTM.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class ProjectController(ISender sender) : ControllerBase
{
    [HttpGet(ProjectsEndPoints.GetAll)]
    [Authorize(Roles ="Admin,User")]
    public async Task<ActionResult<ApiResponse<List<ProjectResponseDto>>>> GetAllProjectsAsync( CancellationToken token)
    {
        var result = await sender.Send(new GetProjectsQuery(),token);
        return new ObjectResult(result) { StatusCode = (int?)result.Code };
    }

    [HttpPost(ProjectsEndPoints.Create)]
    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> CreateProjectAsync([FromForm] CreateProjectDto project, CancellationToken token)
    {
        var result = await sender.Send(new CreateProjectCommand(project),token);
        return new ObjectResult(result) { StatusCode = (int?)result.Code };
    }


    [HttpGet(TasksEndPoints.GetAll)]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ApiResponse<List<TaskResponseDto>>>> GetAllTasks([FromRoute] int projectId, CancellationToken token)
    {
        var result = await sender.Send(new GetTasksQuery(projectId), token);
        return new ObjectResult(result) { StatusCode = (int?)result.Code };
    }

    [HttpPost(TasksEndPoints.Create)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> CreateTask([FromRoute] int projectId, [FromForm] CreateTaskDto task, CancellationToken token)
    {
        var result = await sender.Send(new CreateTaskCommand(projectId, task), token);
        return new ObjectResult(result) { StatusCode = (int?)result.Code };
    }
    [HttpDelete(TasksEndPoints.Remove)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> RemoveTask([FromRoute] int taskId, CancellationToken token)
    {
        var result = await sender.Send(new RemoveTaskCommand(taskId), token);
        return new ObjectResult(result) { StatusCode = (int?)result.Code };
    }

    [HttpDelete(ProjectsEndPoints.Remove)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> RemoveProject([FromRoute] int projectId, CancellationToken token)
    {
        var result = await sender.Send(new RemoveProjectCommand(projectId), token);
        return new ObjectResult(result) { StatusCode = (int?)result.Code };
    }

}