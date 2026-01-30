using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using PGAdminDAL.Model;
using System;
using System.Text.Json;

namespace PGAdminDAL
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public AppDbContext GetDbContext() => (AppDbContext)_configuration;


        public DbSet<Appeal> Appeals { get; set; }
        public DbSet<Sessions> Sessions { get; set; }

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

            modelBuilder.HasDefaultSchema("Povidom");

            modelBuilder.Entity<UserModel>().ToTable("AspNetUsers", schema: "Povidom");
            modelBuilder.Entity<Appeal>().ToTable("Appeals", schema: "Povidom");
            modelBuilder.Entity<Sessions>().ToTable("Sessions", schema: "Povidom");
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var commentsConverter = new ValueConverter<List<Сoments>, string>(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<List<Сoments>>(v, jsonOptions) ?? new()
            );

            var commentsComparer = new ValueComparer<List<Сoments>>(
                (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) ==
                            JsonSerializer.Serialize(c2, jsonOptions),

                c => JsonSerializer.Serialize(c, jsonOptions).GetHashCode(),

                c => JsonSerializer.Deserialize<List<Сoments>>(
                        JsonSerializer.Serialize(c, jsonOptions),
                        jsonOptions
                    ) ?? new()
            );

            modelBuilder.Entity<UserModel>()
                .Property(u => u.CommentsId)
                .HasConversion(commentsConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(commentsComparer);

            modelBuilder.Entity<UserModel>()
                .Property(u => u.LikeComments)
                .HasConversion(commentsConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(commentsComparer);

            modelBuilder.Entity<UserModel>()
                .Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .Property(x => x.Avatar)
                .HasMaxLength(2000)
                .IsRequired();

            modelBuilder.Entity<Appeal>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sessions>()
                .HasOne(us => us.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
