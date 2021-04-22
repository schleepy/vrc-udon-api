using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Dtos;
using VRCUdonAPI.Models.Entities;

namespace VRCUdonAPI.Services
{
    public interface IErrorService : ICrudService<Error, Error, ErrorDto, ErrorDto>
    {
    }
}
