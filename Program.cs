using EMS_Backend.Data;
using EMS_Backend.Seeds;
using EMS_Backend.Services.AuthServices;
using EMS_Backend.Services.CategoryServices;
using EMS_Backend.Services.SupplierServices;
using EMS_Backend.Services.RoleServices;
using EMS_Backend.Services.UserServices; // Add this using directive
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Add this using directive
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen; // Add this using directive
using Swashbuckle.AspNetCore.SwaggerUI;
using EMS_Backend.Services.ImageProductServices;
using EMS_Backend.Services.ProductServices;
using EMS_Backend.Services.GenerateCode;
using EMS_Backend.Services.FunctionServices;
using EMS_Backend.Services.RoleFunctionServices; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); // Add this line
builder.Services.AddSwaggerGen(); // Add this line
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

//Dependency Injection Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IImageProductService, ImageProductService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGenerateCode, GenerateCode>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IFunctionRoleService, FunctionRoleService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    await SeedMasterData.SeedData(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // This will now work
    app.UseSwaggerUI(); // This will now work
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.Run();
