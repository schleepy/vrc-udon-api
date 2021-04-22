using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Dtos;
using VRCUdonAPI.Models.Entities;
using VRCUdonAPI.Repositories;

namespace VRCUdonAPI.Services
{
    public class ErrorService : AbstractCrudService<Error, Error, ErrorDto, ErrorDto>, IErrorService
    {
        public ErrorService(VUAContext context, IMapper mapper) : base(context, mapper) { }
    }
}
