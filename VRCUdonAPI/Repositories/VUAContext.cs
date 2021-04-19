using Microsoft.EntityFrameworkCore;
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
        public VUAContext(DbContextOptions<VUAContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<User>(eb =>
            {
                eb.HasKey(a => a.Id);
            });

            mb.Entity<Query>(eb =>
            {
                eb.HasKey(c => c.Address);
            });
        }
    }
}
