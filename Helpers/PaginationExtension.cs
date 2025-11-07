using EMS_Backend.Dtos.PaginationDtos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS_Backend.Helpers
{
    public static class PaginationExtension
    {
        public static async Task<PaginationResponse<TDestination>> ToPaginationAsync<TSource, TDestination>
            (this IQueryable<TSource> sources, Expression<Func<TSource, TDestination>> selector, int pageIndex = 1, int pageSize = 10)
        {
            // Lấy tổng số bản ghi
            var count = await sources.CountAsync();
            // Phân trang
            var items = await sources.Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .Select(selector)
                        .ToListAsync();
            // trả kết quả
            return new PaginationResponse<TDestination>(items, count, pageIndex, pageSize);
        }
    }
}
