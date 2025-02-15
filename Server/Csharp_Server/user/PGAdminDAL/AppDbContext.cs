using PGAdminDAL.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PGAdminDAL
{
    public class AppDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public AppDbContext GetDbContext() => (AppDbContext)_configuration;


        public DbSet<UserModel> User { get; set; }
        public DbSet<Appeal> Appeals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>().Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<UserModel>().Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<UserModel>().Property(x => x.Avatar)
                .HasMaxLength(2000)
                .IsRequired();


            modelBuilder.Entity<Appeal>()
                .HasOne(a => a.User) 
                .WithMany() 
                .HasForeignKey(a => a.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
