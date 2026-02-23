using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Template> Templates => Set<Template>();
        public DbSet<RepairsContent> RepairsContent => Set<RepairsContent>();

        // ✅ ДОДАНО
        public DbSet<BudgetContent> BudgetContent => Set<BudgetContent>();
    }
}