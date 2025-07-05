namespace PTM.Server;

public class ApiEndpoints
{
    public const string ApiBase = "api";
    public static class ProjectsEndPoints
    {
        public const string Create = "";
        public const string GetAll = "";
        public const string Remove = "{projectId:int}";
    }
    public static class TasksEndPoints
    {
        public const string Create = "{projectId:int}/tasks";
        public const string GetAll = "{projectId:int}/tasks";
        public const string Remove = "{taskId:int}/tasks";
    }
    public static class AuthEndPoints
    {
        public const string Login = "login";
        public const string Register = "Register";
    }
}
