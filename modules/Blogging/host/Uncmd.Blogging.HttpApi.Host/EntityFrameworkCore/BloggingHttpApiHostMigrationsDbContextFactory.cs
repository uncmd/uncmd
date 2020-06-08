using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Uncmd.Blogging.EntityFrameworkCore
{
    public class BloggingHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<BloggingHttpApiHostMigrationsDbContext>
    {
        public BloggingHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<BloggingHttpApiHostMigrationsDbContext>()
                .UseNpgsql(configuration.GetConnectionString("Blogging"));

            return new BloggingHttpApiHostMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
