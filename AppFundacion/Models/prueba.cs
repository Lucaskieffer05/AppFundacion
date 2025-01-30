using Microsoft.EntityFrameworkCore;


namespace AppFundacion.Models
{
    

    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-3EJ63U7;Initial Catalog=ATLANTIS;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
        }
    }
}
