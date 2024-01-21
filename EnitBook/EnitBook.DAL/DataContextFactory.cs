using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
namespace EnitBook.DAL
{
    public class DataContextFactory : IDesignTimeDbContextFactory<EnitBookDbContext>
    {
        public EnitBookDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EnitBookDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EnitDB;Trusted_Connection = True; MultipleActiveResultSets = true");
        return new EnitBookDbContext(optionsBuilder.Options);
        }
    }
}