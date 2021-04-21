using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Entities;

namespace VRCUdonAPI.Repositories
{
    public class VUAContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Error> Errors { get; set; }
        public VUAContext(DbContextOptions<VUAContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<User>(eb =>
            {
                eb.HasKey(a => a.Id);
            });

            mb.Entity<Error>(eb =>
            {
                eb.HasKey(e => e.Id);

                eb.Property(e => e.Created)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("datetime('now')");

                eb.Property(e => e.InnerException).HasConversion(
                    l => JsonConvert.SerializeObject(l),
                    l => JsonConvert.DeserializeObject<Exception>(l));
            });
        }
    }
}
