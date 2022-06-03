using GDemoExpress.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GDemoExpress.DataBase
{
    public class DBContextFactory : IDesignTimeDbContextFactory<DboContext>
    {
        DboContext IDesignTimeDbContextFactory<DboContext>.CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DboContext>();
            var connectionString = "Server=localhost;Database=example;User Id=root;Password=1qaz2wsx";

            _ = builder.UseNpgsql(connectionString);

            return new DboContext(builder.Options);
        }
    }
}
