namespace PTM.Domain.CommunicationModels;

public class PaginatedDbRequest<T> : DbRequest
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = [];

    public static PaginatedDbRequest<T> Success(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        params string[] messages)
    {
        return new PaginatedDbRequest<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public new static PaginatedDbRequest<T> Failure(params string[] messages)
    {
        return new PaginatedDbRequest<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0
        };
    }
}