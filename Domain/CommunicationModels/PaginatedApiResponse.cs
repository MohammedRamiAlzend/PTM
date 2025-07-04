using System.Net;

namespace PTM.Domain.CommunicationModels;

public class PaginatedApiResponse<T> : ApiResponse
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = [];

    public static PaginatedApiResponse<T> Success(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        HttpStatusCode code = HttpStatusCode.OK,
        params string[] messages)
    {
        return new PaginatedApiResponse<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Code = code
        };
    }

    public new static PaginatedApiResponse<T> Failure(HttpStatusCode code = HttpStatusCode.BadRequest,
        params string[] messages)
    {
        return new PaginatedApiResponse<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0,
            Code = code
        };
    }

    public static implicit operator PaginatedApiResponse<T>(PaginatedDbRequest<T> request)
    {
        return new PaginatedApiResponse<T>
        {
            Items = request.Items,
            IsSuccess = request.IsSuccess,
            Message = request.Message,
            TotalCount = request.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Code = request.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest
        };
    }
}