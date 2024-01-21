using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EnitBook.BL.Entities;
using Microsoft.AspNetCore.Identity;

namespace EnitBook.DAL
{
    public class EnitBookDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Profil> Profils { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public EnitBookDbContext(DbContextOptions<EnitBookDbContext> options)
            : base(options)
        {
        }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
          

            // Configure one-to-many relationship between User and Post
            modelBuilder.Entity<User>()
             .HasMany(u => u.Posts)
             .WithOne(p => p.User)
             .HasForeignKey(p => p.UserId);
            
            // Configure one-to-one relationship between User and Profile
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profil)
                .WithOne(p => p.User)
                .HasForeignKey<Profil>(p => p.UserId)
                .IsRequired(false); // If the relationship is optional

            // Configure primary key for Post entity
            modelBuilder.Entity<Post>()
                .HasKey(p => p.PostId);

            // Configure primary key for Profil entity
            modelBuilder.Entity<Profil>()
                .HasKey(pr => pr.IdProfil);

            // You may add additional configurations as needed
            modelBuilder.Entity<Friend>()
                .HasKey(f => f.ID);

            // Configure the relationship between Comment and User
            modelBuilder.Entity<User>()
           .HasMany(u => u.Comments)
           .WithOne(c => c.User)
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
           .WithOne(c => c.post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
