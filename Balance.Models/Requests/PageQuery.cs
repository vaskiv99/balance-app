using System.ComponentModel.DataAnnotations;

namespace Balance.Models.Requests
{
    public class PageQuery
    {
        [Range(0, 1_000_000)]
        public int PageIndex { get; set; }

        public int PageSize { get; set; } = 20;
    }
}