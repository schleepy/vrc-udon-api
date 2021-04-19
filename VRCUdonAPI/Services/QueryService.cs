using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Entities;
using VRCUdonAPI.Repositories;

namespace VRCUdonAPI.Services
{
    public class QueryService : IQueryService
    {
        private QueryContext Context { get; set; }

        public QueryService(QueryContext context)
        {
            Context = context;
        }

        public async Task<Query> Get(IPAddress address)
        {
            return await Context.Queries.FirstOrDefaultAsync(q => q.Address == address);
        }

        public async Task<Query> Update(Query query)
        {
            query.WhenUpdated = DateTime.UtcNow;
            Context.Queries.Update(query);
            await Context.SaveChangesAsync();
            return query;
        }

        public async Task<Query> Create(Query query)
        {
            query.Calls++;
            await Context.Queries.AddAsync(query);
            await Context.SaveChangesAsync();
            return query;
        }

        public async void Delete(Query query)
        {
            Context.Queries.Remove(query);
            await Context.SaveChangesAsync();
        }
    }
}
