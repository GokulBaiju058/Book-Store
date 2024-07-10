
namespace BookStore_API.Data
{
    public class Meta
    {

        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public int[] ItemRange { get; set; } = null!;

        public Meta() { }
        public Meta(int currentpage = 0, int pageSize = 0, int totalCount = 0, int totalPages = 0)
        {
            this.CurrentPage = currentpage;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.TotalPages = totalPages;
        }
    }
}
