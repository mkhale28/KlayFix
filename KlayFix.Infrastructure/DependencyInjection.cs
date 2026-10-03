using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Services;
using KlayFix.Infrastructure.Data;
using KlayFix.Infrastructure.Repositories;

namespace KlayFix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IRepairRepository, RepairRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IRepairService, RepairService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
