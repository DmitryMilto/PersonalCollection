using Microsoft.EntityFrameworkCore;

namespace PersonalCollections.Models.Context
{
    public class Class : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CollectionItem> CollectionItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Thema> Themas { get; set; }
        public Class(DbContextOptions<Class> options)
           : base(options)
        {
            Database.EnsureCreated();
        }
    }
}