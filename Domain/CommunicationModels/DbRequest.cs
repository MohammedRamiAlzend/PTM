namespace PTM.Domain.CommunicationModels;

public class DbRequest
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public static DbRequest Success(params string[] messages)
    {
        return new DbRequest
        {
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    }

    public static DbRequest Failure(params string[] messages)
    {
        return new DbRequest
        {
            IsSuccess = false,
            Message = string.Join(", ", messages)
        };
    }
}

public class DbRequest<T> : DbRequest
{
    public T? Data { get; set; }

    public new static DbRequest<T> Success(params string[] messages)
    {
        return new DbRequest<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    }

    public static DbRequest<T> Success(T data, params string[] messages)
    {
        return new DbRequest<T>
        {
            Data = data,
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    }

    public new static DbRequest<T> Failure(params string[] messages)
    {
        return new DbRequest<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages)
        };
    }
}
