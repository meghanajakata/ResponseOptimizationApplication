using BundlingAndMinification.Models;
using Microsoft.EntityFrameworkCore;

namespace BundlingAndMinification.Data
{
    public class DbEmployeeContext : DbContext
    {
        public DbEmployeeContext(DbContextOptions<DbEmployeeContext> options): base(options) 
        {
            
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
