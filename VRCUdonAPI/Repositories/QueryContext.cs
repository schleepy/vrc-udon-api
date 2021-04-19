using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Query = VRCUdonAPI.Models.Entities.Query;

namespace VRCUdonAPI.Repositories
{
    public class QueryContext : DbContext
    {
        public DbSet<Query> Queries { get; set; }
        public QueryContext(DbContextOptions<QueryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<Query>(eb =>
            {
                eb.HasKey(c => c.Address);
            });
        }
    }
}
