using AutoMapper;
using HotPack.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.AutoMapper.Extensions
{
    public class ResultBuilder
    {        
        private readonly IMapper mapper;

        public ResultBuilder(IConfigurationProvider config) {
            this.mapper = config.CreateMapper();
        }

        public ResultBuilder(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ResultBuilder(Profile profile)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profile);

            });

            this.mapper = config.CreateMapper();
        }

        public Result<T> Map<M, T>(Result<M> result) where T : class
        {
            var result2 = new Result<T>();
            result2.Value = result.Value;
            result2.Message = result.Message;
            result2.Code = result.Code;

            var data = mapper.Map<T>(result.Data);
            result2.Data = data;

            return result2;
        }

        public ResultList<T> MapList<M, T>(ResultList<M> result) where T : class
        {

            var result2 = new ResultList<T>();
            result2.Value = result.Value;
            result2.Message = result.Message;
            result2.Code = result.Code;

            var data = mapper.Map<List<T>>(result.Data);
            result2.Data = data;

            return result2;

        }
    }
}
