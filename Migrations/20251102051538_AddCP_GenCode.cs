using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCP_GenCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                 CREATE OR ALTER PROCEDURE [dbo].[CP_GenCode]
					@Prefix NVARCHAR(50),
					@Table NVARCHAR(50),
					@Column NVARCHAR(50),
					@ReturnNewId NVARCHAR(50) OUTPUT
				AS
				BEGIN
					-- SET NOCOUNT ON added to prevent extra result sets from
					-- interfering with SELECT statements.
					SET NOCOUNT ON;
					DECLARE @SQL NVARCHAR(500) = N''
					DECLARE @MaxId NVARCHAR(50) = N''
					--SELECT @ReturnNewUserId = MAX(u.UserId) FROM dbo.Users as u WHERE u.UserId LIKE @RoleId + N'%'
					SET @SQL = N'SELECT @MaxId = MAX(' + QUOTENAME(@Column) + ') 
								 FROM dbo.' + QUOTENAME(@Table) + ' 
								 WHERE ' + QUOTENAME(@Column) + ' LIKE @Prefix + N''%'''

					EXEC sp_executesql 
					@SQL, 
					N'@MaxId NVARCHAR(50) OUTPUT, @Prefix NVARCHAR(50)',
					@MaxId OUTPUT, 
					@Prefix

					IF @MaxId IS NULL OR @MaxId  = N''
						BEGIN
							SET @ReturnNewId = @Prefix + N'0001'
						END
					ELSE
						BEGIN
							DECLARE @Stt INT = 0
							SET @Stt = CAST((SELECT REPLACE(@MaxId, @Prefix, '') as stt) AS int)
							SET @Stt = @Stt + 1

							DECLARE @ConvertIntToNvarchar NVARCHAR (10) = N''
							SET @ConvertIntToNvarchar = RIGHT('0000' + CAST(@Stt as nvarchar), 4)
							SET @ReturnNewId = @Prefix + @ConvertIntToNvarchar
						END
					SELECT @ReturnNewId AS NewID
				END
            ";
            migrationBuilder.Sql(sql);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CP_GenCode");
        }
    }
}
