using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Entities;

namespace VRCUdonAPI.Services
{
    public interface IQueryService
    {
        Task<Query> Get(IPAddress address);
        Task<Query> Update(Query query);
        Task<Query> Create(Query query);
        void Delete(Query query);
    }
}
