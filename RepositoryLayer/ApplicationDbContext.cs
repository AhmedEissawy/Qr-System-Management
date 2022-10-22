using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Entity <IdentityUserRole<string>>().HasKey(a=>new {a.UserId,a.RoleId });

            builder.Entity<Owner>()
                .HasOne(u => u.Unit)
                .WithOne(o => o.Owner)
                .HasForeignKey<Unit>(oi => oi.OwnerId);

            builder.Entity<Owner>()
                .HasMany(i => i.Invitaions)
                .WithOne(i=>i.Owner)
                .HasForeignKey(a=>a.OwnerId);

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Owner)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<Owner>(a => a.ApplicationUserId);
 
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Owner> Owners { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<Invitaion> Invitaions { get; set; }
    }

}
