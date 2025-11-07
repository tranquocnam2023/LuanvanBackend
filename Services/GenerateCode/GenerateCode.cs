
using EMS_Backend.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EMS_Backend.Services.GenerateCode
{
    public class GenerateCode : IGenerateCode
    {
        private readonly AppDbContext _context;
        public GenerateCode(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateCodeAsync(string prefix, string table, string column)
        {
            //throw new NotImplementedException();
            var prefixParam = new SqlParameter("@Prefix", prefix);
            var tableParam = new SqlParameter("@Table", table);
            var columnParam = new SqlParameter("@Column", column);

            var returnNewIdParam = new SqlParameter
            {
                ParameterName = "@ReturnNewId",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Size = 50,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.CP_GenCode @Prefix, @Table, @Column, @ReturnNewId OUTPUT",
                prefixParam, tableParam, columnParam, returnNewIdParam);

            return returnNewIdParam.Value.ToString() ?? string.Empty;
        }
    }
}
