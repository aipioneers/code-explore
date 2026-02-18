namespace EnterpriseApp.Application.Common;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class PagedList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
    /// </summary>
    /// <param name="items">The items in the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PagedList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets the index of the first item on the current page (1-based).
    /// </summary>
    public int FirstItemIndex => (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Gets the index of the last item on the current page (1-based).
    /// </summary>
    public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Creates an empty paged list.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    public static PagedList<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedList<T>(Array.Empty<T>(), 0, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates a paged list from a queryable source.
    /// </summary>
    /// <param name="source">The source queryable.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var totalCount = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates a paged list from a collection.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="totalCount">The total count.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    public static PagedList<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PagedList<T>(items.ToList(), totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Maps the items to a new type.
    /// </summary>
    /// <typeparam name="TResult">The new item type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    public PagedList<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        var mappedItems = Items.Select(mapper).ToList();
        return new PagedList<TResult>(mappedItems, TotalCount, PageNumber, PageSize);
    }
}

/// <summary>
/// Extension methods for creating paged lists.
/// </summary>
public static class PagedListExtensions
{
    /// <summary>
    /// Creates a paged list from a queryable source asynchronously.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source queryable.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Note: In actual implementation, use EF Core's async methods
        // This is a placeholder that will be implemented in Infrastructure layer
        await Task.CompletedTask;
        return PagedList<T>.Create(source, pageNumber, pageSize);
    }
}
