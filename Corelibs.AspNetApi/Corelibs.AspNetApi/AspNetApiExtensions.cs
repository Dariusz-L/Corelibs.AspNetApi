using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Formats.Asn1.AsnWriter;

namespace Corelibs.AspNetApi
{
    public static class AspNetApiExtensions
    {
        public static void AddDbContext<TDbContext>(
            this IServiceCollection services, IHostEnvironment environment, Func<string, string> getConfigDevConnectionString)
            where TDbContext : DbContext
        {
            var conn = GetDBConnectionString(environment, getConfigDevConnectionString);
            services.AddDbContextFactory<TDbContext>(
                opts => opts.UseSqlServer(conn));
        }

        public static string GetDBConnectionString(IHostEnvironment environment, Func<string, string> getConfigDevConnectionString)
        {
            if (environment.IsDevelopment())
                return getConfigDevConnectionString("MSSqlDB");
            else
                return Environment.GetEnvironmentVariable("MSSqlDB");
        }

        public static async Task InitilizeDatabase<TDbContext>(this IServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            //using (var scope = serviceProvider.CreateScope())
            //{
            //    var ctx = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext();
            //    ctx.Database.EnsureCreated();

            //    if ((await ctx.Database.GetPendingMigrationsAsync()).Any())
            //    {
            //        await ctx.Database.MigrateAsync();
            //    }
            //}
        }
    }
}
