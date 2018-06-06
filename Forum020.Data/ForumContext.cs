using Forum020.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Forum020.Data
{
    public class ForumContext : DbContext
    {
        public ForumContext(DbContextOptions<ForumContext> options) : base(options) { }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Board
            builder.Entity<Board>().HasMany(e => e.Threads).WithOne(e => e.Board);
            builder.Entity<Board>().HasOne(e => e.Config).WithOne()
                .HasForeignKey<Board>(e => e.ConfigId).IsRequired();
            builder.Entity<Board>().Property(e => e.Name).IsRequired();
            builder.Entity<Board>().HasIndex(e => e.Name).IsUnique();
            builder.Entity<Board>().Property(e => e.NameShort).IsRequired();
            builder.Entity<Board>().HasIndex(e => e.NameShort).IsUnique();
            builder.Entity<Board>().Property(e => e.ConfigId).IsRequired();

            //Post
            builder.Entity<Post>().HasMany(e => e.Posts).WithOne(e => e.Thread);
            builder.Entity<Post>().HasIndex(e => new { e.IdEffective, e.BoardId }).IsUnique();
            builder.Entity<Post>().Property(e => e.IdEffective).IsRequired();
            builder.Entity<Post>().Property(e => e.IdEffective).ValueGeneratedOnAdd();
            builder.Entity<Post>().Property(e => e.BoardId).IsRequired();

            //Config
            builder.Entity<Config>().Property(e => e.MaximumReplyCount).HasDefaultValue(100);
            builder.Entity<Config>().Property(e => e.MaximumThreadCount).HasDefaultValue(12);
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateDates();
            return (await base.SaveChangesAsync(true, cancellationToken));
        }

        public override int SaveChanges()
        {
            UpdateDates();
            return base.SaveChanges();
        }

        private void UpdateDates()
        {
            var changes = from e in this.ChangeTracker.Entries<BaseEntity>()
                          where e.State != EntityState.Unchanged
                          select e;

            foreach (var change in changes)
            {
                if (change.State == EntityState.Added)
                {
                    change.Entity.DateCreated = DateTime.UtcNow;
                }
                else if (change.State == EntityState.Modified)
                {
                    change.Entity.DateEdited = DateTime.UtcNow;
                }
            }
        }
    }
}