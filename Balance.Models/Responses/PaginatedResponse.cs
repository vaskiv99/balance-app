using System.Collections.Generic;

namespace Balance.Models.Responses
{
    public class PaginatedResponse<T>
    {
        public PaginatedResponse(ICollection<T> data, long numberOfPages, long numberOfRecords)
        {
            Result = data;
            Paging = new Paging(numberOfRecords, numberOfPages);
        }

        public PaginatedResponse(ICollection<T> data, Paging paging)
        {
            Result = data;
            Paging = paging;
        }

        public PaginatedResponse()
        {
            Result = new List<T>();
            Paging = new Paging(0, 0);
        }


        public ICollection<T> Result { get; }
        public Paging Paging { get; }

    }
}