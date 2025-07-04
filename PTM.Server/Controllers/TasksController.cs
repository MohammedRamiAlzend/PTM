namespace PTM.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class TasksController(ISender sender) : ControllerBase
{
    [HttpGet(TasksEndPoint.GetAll)]
    public async Task<ActionResult<ApiResponse<List<TaskResponseDto>>>> GetAllTasks([FromRoute] int projectId)
    {
        return await sender.Send(new GetTasksQuery(projectId));
    }

    [HttpPost(TasksEndPoint.Create)]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> CreateProject([FromRoute] int projectId, [FromForm] CreateTaskDto task)
    {
        return await sender.Send(new CreateTaskCommand(projectId,task));
    }

}