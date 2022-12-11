using LegoasApp.Core.Interfaces;
using LegoasApp.Core.Services;
using LegoasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string constring = builder.Configuration.GetConnectionString("LegoasAppDB");
builder.Services.AddDbContext<LegoasAppContext>(opt => opt.UseSqlServer(constring));


// Add Service interface
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRoleService, AccountRoleService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IMenuScreenService, MenuScreenService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleMenuService, RoleMenuService>();
builder.Services.AddScoped<IUserBranchService, UserBranchService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSession();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
