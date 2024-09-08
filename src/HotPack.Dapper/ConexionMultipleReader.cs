using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Dapper
{
    public class ConexionMultipleReader
    {
        private readonly SqlMapper.GridReader reader;

        public ConexionMultipleReader(SqlMapper.GridReader reader) 
        {
            this.reader = reader;  
        }        

        //public T? ReadSingleOrDefault<T>()
        //{
        //    return reader.ReadSingleOrDefault<T>();
        //}

        public async Task<T?> ResultsToObjectAsync<T>()
        {
            return await reader.ReadSingleOrDefaultAsync<T>();
        }

        //public List<T> Read<T>()
        //{
        //    return reader.Read<T>().ToList();
        //}

        public async Task<List<T>> ResultsAsync<T>()
        {
            var list = await reader.ReadAsync<T>();

            return list.ToList();
        }
    }
}
