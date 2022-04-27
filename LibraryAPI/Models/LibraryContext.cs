using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
         : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Lending> Lendings { get; set; }


        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

            if (!entities.Any())
                return;

            var now = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                var baseModel = (BaseModel)entity.Entity;

                if (entity.State == EntityState.Added)
                {
                    baseModel.CreatedAt = now;
                }

                baseModel.UpdatedAt = now;
            }
        }

    }
}
