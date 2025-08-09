namespace TodoMinimal.Application.Common
{
    public class PagedResult<T>
    {
        public IList<T> Items { get; set; }
        public PageData PageData { get; set; }

        public PagedResult()
        {
            Items = [];
            PageData = new PageData(1, 10, 0);
        }

        public PagedResult(IList<T> items, PageData pageData)
        {
            Items = items;
            PageData = pageData;
        }

        public PagedResult(IList<T> items, int pageNumber, int pageSize, int total)
        {
            Items = items;
            PageData = new PageData(pageNumber, pageSize, total);
        }
    }
}