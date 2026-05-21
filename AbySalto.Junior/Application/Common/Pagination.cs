using AbySalto.Junior.Application.DTOs.Common;

namespace AbySalto.Junior.Application.Common;

public static class Pagination
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public static (int Page, int PageSize) Normalize(PaginationQuery query)
    {
        var page = query.Page is > 0 ? query.Page.Value : DefaultPage;
        var pageSize = query.PageSize is > 0 ? query.PageSize.Value : DefaultPageSize;

        return (page, pageSize);
    }
}
