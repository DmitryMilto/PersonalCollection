using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models
{
    public class PersonalCollectionContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CollectionItem> CollectionItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Thema> Themas { get; set; }
        public PersonalCollectionContext(DbContextOptions<PersonalCollectionContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
