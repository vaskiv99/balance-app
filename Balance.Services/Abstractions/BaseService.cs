using Balance.Models.Requests;
using Balance.Models.Responses;

namespace Balance.Services.Abstractions
{
    public abstract class BaseService
    {
        private static long DetermineNumberOfPages(int pageSize, long numberOfRecords)
        {
            if (pageSize == 0) return 0;
            return numberOfRecords / pageSize + (numberOfRecords % pageSize > 0 ? 1 : 0);
        }

        protected static Paging CreatePaging(PageQuery query, long numberOfRecords)
        {
            var numberOfPages = DetermineNumberOfPages(query.PageSize, numberOfRecords);
            return new Paging(numberOfRecords, numberOfPages);
        }
    }
}