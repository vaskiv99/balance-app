using Balance.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Balance.DAL
{
    public class BalanceDbContext : DbContext
    {
        public BalanceDbContext(DbContextOptions<BalanceDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEntity>()
                .HasMany(pt => pt.Transactions)
                .WithOne(p => p.Sender)
                .HasForeignKey(pt => pt.SenderId);
        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<TransactionEntity> Transactions { get; set; }
    }
}