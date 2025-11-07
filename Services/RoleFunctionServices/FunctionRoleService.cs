using EMS_Backend.Data;
using EMS_Backend.Dtos.RoleFunctionDtos;
using EMS_Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS_Backend.Services.RoleFunctionServices
{
    public class FunctionRoleService : IFunctionRoleService
    {
        private readonly AppDbContext _context;

        public FunctionRoleService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<RoleFunctionResponse>> CreateOrUpdateRoleFunction(List<RoleFunctionRequest> roleFunctionRequests)
        {
            //throw new NotImplementedException();

            if (!roleFunctionRequests.Any())
                throw new ArgumentException("Request không được rỗng!");

            try
            {
                var existingRoleFunctions = _context.RoleFunctions
                    .Where(rf => rf.RoleId == roleFunctionRequests.First().RoleId)
                    .ToList();

                /*
                 * cái này là trường hợp truyền functionId không đủ nên mới có đoạn code này còn truyền lên đủ thì có thể bỏ
                 */

                var functions = await _context.FunctionMasters.Select(f => f.FunctionId).ToListAsync();
                var roleFunctions = new List<RoleFunctions>();

                if (existingRoleFunctions.Any() == false) // thêm mới
                {
                    foreach (var functionId in functions)
                    {
                        var newRoleFunction = new RoleFunctions
                        {
                            RoleId = roleFunctionRequests.First().RoleId,
                            FunctionId = functionId,
                            IsActive = roleFunctionRequests.Any(rf => rf.FunctionId == functionId)
                                        ? roleFunctionRequests.FirstOrDefault(rf => rf.FunctionId == functionId)!.IsActive
                                        : false
                        };
                        roleFunctions.Add(newRoleFunction);
                    }
                    await _context.RoleFunctions.AddRangeAsync(roleFunctions);
                }
                else
                {
                    foreach (var item in existingRoleFunctions)
                    {
                        item.IsActive = roleFunctionRequests.Any(rf => rf.FunctionId == item.FunctionId)
                                        ? roleFunctionRequests.FirstOrDefault(rf => rf.FunctionId == item.FunctionId)!.IsActive
                                        : false;
                    }
                }

                await _context.SaveChangesAsync();
                var response = await GetRoleFunctionsWithRoleId(roleFunctionRequests.First().RoleId);
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> DeleteRoleFunction(string roleId)
        {
            //throw new NotImplementedException();
            try
            {
                var roleFunctions = await _context.RoleFunctions
                    .Where(rf => rf.RoleId == roleId)
                    .ToListAsync();
                if (roleFunctions.Any() == false)
                {
                    return false;
                }
                _context.RoleFunctions.RemoveRange(roleFunctions);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<RoleFunctionResponse>> GetAllRoleFunction()
        {
            //throw new NotImplementedException();
            var response = await _context.FunctionMasters.Select(f => new RoleFunctionResponse
                {
                    FunctionId = f.FunctionId,
                    FunctionName = f.FunctionName,
                    IsActive = false
                }).ToListAsync();

            return response;
        }

        public Task<List<RoleFunctionResponse>> GetRoleFunctionsWithRoleId(string roleId)
        {
            //throw new NotImplementedException();
            var response = (from rf in _context.RoleFunctions
                            join f in _context.FunctionMasters on rf.FunctionId equals f.FunctionId
                            where rf.RoleId == roleId
                            select new RoleFunctionResponse
                            {
                                FunctionId = rf.FunctionId,
                                FunctionName = f.FunctionName,
                                IsActive = rf.IsActive
                            }).ToListAsync();
            return response;
        }
    }
}
