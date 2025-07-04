namespace PTM.Server;

public class ApiEndpoints
{
    public const string ApiBase = "api";
    public static class ProjectsEndPoint
    {
        public const string Create = "";
        public const string GetAll = "";
    }
    public static class TasksEndPoint
    {
        public const string Create = "/{projectId:int}/tasks";
        public const string GetAll = "/{projectId:int}/tasks";
    }
}
