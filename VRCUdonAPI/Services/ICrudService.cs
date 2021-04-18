using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Services
{
    public interface ICrudService<TEntity, TInput, TDto, TDetailDto>
        where TEntity : class
        where TInput : class
        where TDto : class
        where TDetailDto : class
    {
        Task<TDto> Create(TInput input);
        void Delete(params object[] id);
        void Delete(TEntity entity);
        void Update(TInput input, params object[] id);
        Task<TDetailDto> GetSingle(params object[] id);
        Task<IList<TDto>> GetAll();
    }
}
