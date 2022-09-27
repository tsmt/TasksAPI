using Microsoft.EntityFrameworkCore;

namespace TasksAPI.Extensions
{
    public class PagedList<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public List<T> Items { get; set; }

        public async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            TotalCount = source.Count();
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            if (pageSize <= 0)
            {
                pageSize = TotalCount;
            }

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
            Items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return this;
        }
    }
}
