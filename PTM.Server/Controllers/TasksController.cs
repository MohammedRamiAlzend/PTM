
using Microsoft.AspNetCore.Authorization;

namespace PTM.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class TasksController(ISender sender) : ControllerBase
{
    [HttpGet(TasksEndPoint.GetAll)]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ApiResponse<List<TaskResponseDto>>>> GetAllTasks([FromRoute] int projectId,CancellationToken token)
    {
        return await sender.Send(new GetTasksQuery(projectId),token);
    }

    [HttpPost(TasksEndPoint.Create)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> CreateProject([FromRoute] int projectId, [FromForm] CreateTaskDto task, CancellationToken token)
    {
        return await sender.Send(new CreateTaskCommand(projectId, task),token);
    }

}