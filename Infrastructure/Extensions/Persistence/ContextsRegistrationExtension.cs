using Domain.Exceptions.Custom;
using Infrastructure.Logging;
using Infrastructure.Persistence.EFC.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions.Persistence;

public static class ContextsRegistrationExtension
{
    public static IServiceCollection AddEfcContexts(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        if (environment.IsDevelopment())
        {
            services.AddSingleton<SqliteConnection>(_ =>
            {
                var conn = new SqliteConnection("Data Source=:memory:;");
                conn.Open();
                return conn;
            });

            services.AddDbContext<CoreFitnessContext>((sp, options) =>
            {
                var conn = sp.GetRequiredService<SqliteConnection>();
                options.UseSqlite(conn);
            });
        }
        else
        {
            services.AddDbContext<CoreFitnessContext>((sp, options) =>
            {
                try
                {
                    var conn = configuration.GetConnectionString("Prod_DB_CoreFitness") 
                        ?? throw new NotFoundDomainException("Production Database connection string not found.");
                    options.UseSqlServer(conn);
                }
                catch(Exception ex)
                {
                    Logger logger = new();
                    logger.Log(ex);
                    throw;
                }
            });
        }

        return services;
    }
}