namespace EMS_Backend.Dtos.PaginationDtos
{
    public class PaginationResponse<T> 
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; private set; } // Trang hiện tại 
        public int PageSize { get; private set; } // Số lượng record trả ra
        public int TotalPages { get; private set; } // Tổng số trang
        public int TotalCounts { get; private set; } // Tổng số record trong db

        public PaginationResponse(List<T> items, int count, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCounts = count;
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
