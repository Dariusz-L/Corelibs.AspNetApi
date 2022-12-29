using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corelibs.AspNetApi
{
    public static class AspNetApiExtensions
    {
        public static void AddDbContext<TDbContext>(
            this IServiceCollection services, IHostEnvironment environment, Func<string, string> getConfigDevConnectionString)
            where TDbContext : DbContext
        {
            var conn = GetDBConnectionString(environment, getConfigDevConnectionString);

            services.AddDbContext<TDbContext>((sp, opts) =>
            {
                opts.UseSqlServer(conn);
            });
        }

        public static string GetDBConnectionString(IHostEnvironment environment, Func<string, string> getConfigDevConnectionString)
        {
            if (environment.IsDevelopment())
                return getConfigDevConnectionString("MSSqlDB");
            else
                return Environment.GetEnvironmentVariable("MSSqlDB");
        }
    }
}
