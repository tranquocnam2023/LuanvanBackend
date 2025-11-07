using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.CategoryDtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.RoleDtos;
using EMS_Backend.Dtos.SupplierDtos;
using EMS_Backend.Dtos.UserDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using EMS_Backend.Services.RoleFunctionServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS_Backend.Services.RoleServices
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly IFunctionRoleService _functionRoleService;
        
        public RoleService(AppDbContext context, IFunctionRoleService functionRoleService)
        {
            _context = context;
            _functionRoleService = functionRoleService;
        }



        public async Task<ServiceResponse<RoleResponse>> AddRoleAsync(RoleRequest request)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(request.RoleId) || string.IsNullOrEmpty(request.RoleName))
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid Role Request",
                };
            }

            try
            {
                if (await _context.Roles.AnyAsync(r => r.RoleId == request.RoleId))
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.ROLE_ADD_CONFLICT,
                        Message = "Role already exists"
                    };
                }

                var newRole = new Role 
                { 
                    RoleId = request.RoleId, 
                    RoleName = request.RoleName 
                };
                //tao role nhung chua them vao csdl
                await _context.Roles.AddAsync(newRole);

                if (request.roleFunctionRequests != null && request.roleFunctionRequests.Count > 0)
                {
                    //tao vong lap de them cai RoleId vao trong cái RoleFunctionRequest va gan gia tri RoleID tu NewRole 
                    foreach (var rf in request.roleFunctionRequests)
                    {
                        rf.RoleId = newRole.RoleId;
                    }
                    var resultRF =  await _functionRoleService.CreateOrUpdateRoleFunction(request.roleFunctionRequests);

                    if (resultRF.Any() == false || resultRF.Count <= 0)
                    {
                        return new ServiceResponse<RoleResponse>
                        {
                            Success = false,
                            StatusCode = StatusCodes.Status409Conflict,
                            ErrorCode = ErrorCodes.ROLE_ADD_CONFLICT,
                            Message = "Error adding role functions."
                        };
                    }
                }

                await _context.SaveChangesAsync();

                var response = await GetRoleByIdAsync(newRole.RoleId);

                if (response.Success == false)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.ROLE_ADD_CONFLICT,
                        Message = "Error retrieving added user."
                    };
                }

                return new ServiceResponse<RoleResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Role added successfully",
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<RoleResponse>> DeleteRoleAsync(string roleId)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(roleId))
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid Role Request"
                };
            }

            try
            {
                var existingRole = await _context.Roles.FindAsync(roleId);

                if (existingRole == null)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.ROLE_NOT_FOUND,
                        Message = "Role not found"
                    };
                }

                bool resultDeleteRoleFunctions = await _functionRoleService.DeleteRoleFunction(roleId);
                if (resultDeleteRoleFunctions == false)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.ROLE_DELETE_CONFLICT,
                        Message = "Error deleting role functions."
                    };
                }

                _context.Roles.Remove(existingRole);
                await _context.SaveChangesAsync();

                var roleResponse = await GetRoleByIdAsync(roleId);
                if (roleResponse.Success == false)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Role deleted successfully"
                    };
                }
                else
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.SUPPLIER_DELETE_CONFLICT,
                        Message = "Error retrieving deleted role."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<RoleResponse>>> GetAllRolesAsync(PaginationParams @params)
        {
            //throw new NotImplementedException();
            try
            {
                var query = _context.Roles.AsQueryable().AsNoTracking();
                Expression<Func<Role, RoleResponse>> selectors = role => new RoleResponse
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName
                };

                var results = await PaginationExtension.ToPaginationAsync(query, selectors, @params.PageIndex, @params.PageSize);
                foreach (var item in results.Items)
                {
                    item.RoleFunctions = await _functionRoleService.GetRoleFunctionsWithRoleId(item.RoleId);
                }

                return new ServiceResponse<PaginationResponse<RoleResponse>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get All Roles successfully.",
                    Data = results
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResponse<RoleResponse>>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR
                };
            }
        }

        public async Task<ServiceResponse<RoleResponse>> GetRoleByIdAsync(string roleId)
        {
            //throw new NotImplementedException();
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid Role Id",
                    };
                }

                var roleFunctionsResponse = await _functionRoleService.GetRoleFunctionsWithRoleId(roleId);

                var response = await _context.Roles.AsNoTracking()
                                .Where(r => r.RoleId == roleId)
                                .Select(r => new RoleResponse
                                {
                                    RoleId = r.RoleId,
                                    RoleName = r.RoleName,
                                    RoleFunctions = roleFunctionsResponse
                                })
                                .FirstOrDefaultAsync();

                if (response == null)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.ROLE_NOT_FOUND,
                        Message = "Invalid Role Id",
                    };
                }

                return new ServiceResponse<RoleResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get Role By Id Successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<RoleResponse>> UpdateRoleAsync(RoleRequest request)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid Role Request",
                };
            }

            try
            {
                var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId);
                if (existingRole == null)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.ROLE_NOT_FOUND,
                        Message = "Role not found",
                    };
                }

                existingRole.RoleName = request.RoleName;

                if (request.roleFunctionRequests != null && request.roleFunctionRequests.Count > 0)
                {
                    foreach (var rf in request.roleFunctionRequests)
                    {
                        rf.RoleId = existingRole.RoleId;
                    }
                    var resultRF = await _functionRoleService.CreateOrUpdateRoleFunction(request.roleFunctionRequests);

                    if (resultRF.Any() == false || resultRF.Count <= 0)
                    {
                        return new ServiceResponse<RoleResponse>
                        {
                            Success = false,
                            StatusCode = StatusCodes.Status409Conflict,
                            ErrorCode = ErrorCodes.ROLE_ADD_CONFLICT,
                            Message = "Error adding role functions."
                        };
                    }
                }
                await _context.SaveChangesAsync();
                var response = await GetRoleByIdAsync(existingRole.RoleId);
                if (response.Success == false)
                {
                    return new ServiceResponse<RoleResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.ROLE_UPDATE_CONFLICT,
                        Message = "Error retrieving updated role."
                    };
                }
                return new ServiceResponse<RoleResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role updated successfully",
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoleResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }
    }
}
