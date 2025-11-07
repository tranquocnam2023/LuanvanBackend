using EMS_Backend.Data;
using EMS_Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EMS_Backend.Seeds
{
    public class SeedMasterData
    {
        public static async Task SeedData(IServiceProvider services)
        {
            var db = services.GetRequiredService<AppDbContext>();

            try
            {

                var roles = new List<Role>()
                {
                    new Role { RoleId = "Administrator", RoleName = "Quản trị hệ thống" },
                    new Role { RoleId = "Employee", RoleName = "Nhân viên" },
                    new Role { RoleId = "Manager", RoleName = "Quản lý" },
                    new Role { RoleId = "Customer", RoleName = "Khách hàng" }
                };

                var existingRoles = await db.Roles.Select(r => r.RoleId).ToListAsync();

                roles.RemoveAll(item => existingRoles.Contains(item.RoleId));

                if (roles.Count > 0)
                {
                    await db.Roles.AddRangeAsync(roles);
                }

                //Users
                var users = new List<User>()
                {
                    new User { UserId = "administrator", Password = "123", FullName = "Administrator", RoleId = "Administrator" }
                };

                var existingUsers = await db.Users.Select(u => u.UserId).ToListAsync();
                users.RemoveAll(item => existingUsers.Contains(item.UserId));
                if (users.Count > 0)
                {
                    await db.Users.AddRangeAsync(users);
                }

                // Categories
                var categories = new List<Category>()
                {
                    new Category { Name = "Electronics", ParentCategoryId = null },
                    new Category { Name = "Computers", ParentCategoryId = 1 },
                    new Category { Name = "Laptops", ParentCategoryId = 2 },
                    new Category { Name = "Desktops", ParentCategoryId = 2 },
                    new Category { Name = "Smartphones", ParentCategoryId = 1 },
                    new Category { Name = "Home Appliances", ParentCategoryId = null },
                    new Category { Name = "Refrigerators", ParentCategoryId = 6 },
                    new Category { Name = "Washing Machines", ParentCategoryId = 6 }
                };

                var countCategories = await db.Categories.CountAsync();

                if(countCategories == 0)
                {
                    await db.Categories.AddRangeAsync(categories);
                }

                // FunctionMasters
                var functions = new List<FunctionMaster>()
                {
                    new FunctionMaster { FunctionId = "Func001" ,FunctionName = "Quản lý quyền" },
                    new FunctionMaster { FunctionId = "Func002" ,FunctionName = "Quản lý người dùng" },
                    new FunctionMaster { FunctionId = "Func003" ,FunctionName = "Quản lý hàng hóa" },
                    new FunctionMaster { FunctionId = "Func004" ,FunctionName = "Quản lý nhà cung cấp" },
                    new FunctionMaster { FunctionId = "Func005" ,FunctionName = "Quản lý loại hàng hóa" },
                };
                var existingFunctions = await db.FunctionMasters.Select(f => f.FunctionId).ToListAsync();
                functions.RemoveAll(item => existingFunctions.Contains(item.FunctionId));
                if (functions.Count > 0)
                {
                    await db.FunctionMasters.AddRangeAsync(functions);
                }

                var roleFuntions = new List<RoleFunctions>();

                foreach (var functionId in existingFunctions) {
                    var rf = new RoleFunctions { FunctionId = functionId, RoleId = "Administrator", IsActive = true, };
                    roleFuntions.Add(rf);
                }

                await db.RoleFunctions.AddRangeAsync(roleFuntions);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
