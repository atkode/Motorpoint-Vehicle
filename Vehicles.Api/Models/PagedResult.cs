namespace Vehicles.Api.Models;

public sealed class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public int PageNumber { get; set; }

    public int ListSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / ListSize);
}