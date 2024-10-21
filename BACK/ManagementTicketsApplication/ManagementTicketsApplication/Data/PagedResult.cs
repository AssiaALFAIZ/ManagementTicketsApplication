namespace ManagementTicketsApplication.Data
{
    /// <summary>
    /// Represents a paged result set containing a collection of records and pagination information.
    /// This class is used to encapsulate the results of a query that supports pagination, 
    /// including the current page number, total records, total pages, and the actual records retrieved.
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Records { get; set; }
        public int Page { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        public PagedResult(IEnumerable<T> records, int page, int totalRecords, int pageSize)
        {
            Records = records;
            Page = page;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        }
    }
}