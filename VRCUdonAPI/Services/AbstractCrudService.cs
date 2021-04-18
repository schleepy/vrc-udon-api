using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Repositories;

namespace VRCUdonAPI.Services
{
    public abstract class AbstractCrudService<TEntity, TInput, TDto, TDetailDto> : ICrudService<TEntity, TInput, TDto, TDetailDto>
        where TEntity : class
        where TInput : class
        where TDto : class
        where TDetailDto : class
    {
        protected VUAContext Context { get; }
        protected IMapper Mapper { get; }

        protected AbstractCrudService(VUAContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public virtual async Task<TDto> Create(TInput input)
        {
            TEntity entity = Mapper.Map<TEntity>(input);
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
            return Mapper.Map<TDto>(entity);
        }

        public virtual void Delete(params object[] id)
        {
            TEntity entity = Context.Find<TEntity>(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            Context.Remove(entity);
            Context.SaveChanges();
        }

        public virtual async Task<IList<TDto>> GetAll()
        {
            return await Context.Set<TEntity>()
                .AsNoTracking()
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public virtual async Task<TDetailDto> GetSingle(params object[] id)
        {
            return Mapper.Map<TDetailDto>(await Context.FindAsync<TEntity>(id));
        }

        public virtual void Update(TInput input, params object[] id)
        {
            TEntity entity = Context.Find<TEntity>(id);
            entity = Mapper.Map<TEntity>(input);
            Context.Update(entity);
            Context.SaveChanges();
        }
    }
}
