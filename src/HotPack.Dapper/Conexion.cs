using Dapper;
using HotPack.Classes;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;

namespace HotPack.Dapper
{
    public class Conexion
    {
        private readonly string connectionString;

        public Conexion(string connectionString)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            this.connectionString = connectionString;
        }

        private static DynamicParameters? TransformParams(ConexionParameters? parameters)
        {
            if (parameters == null) return null;

            var dapperParameters = new DynamicParameters();

            foreach (var p in parameters.Parameters)
            {
                dapperParameters.Add(p.Name, p.Value, p.Type, p.Direction, p.Size);
            }

            return dapperParameters;
        }

        private static Result GetResult(DynamicParameters? parameters)
        {
            var result = new Result(true, "the query has been executed correctly");

            if(parameters is null) return result;

            var resultName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "result" || p.ToLower() == "presult" || p.ToLower() == "resultado" || p.ToLower() == "presultado");

            if (resultName != null)
            {
                result.Value = parameters.Get<bool>(resultName);
            }

            var messageName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "msg" || p.ToLower() == "pmsg" || p.ToLower() == "message" || p.ToLower() == "pmessage" || p.ToLower() == "mensaje" || p.ToLower() == "pmensaje");

            if (messageName != null)
            {
                result.Message = parameters.Get<string>(messageName);
            }

            var codeName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "codigo" || p.ToLower() == "pcodigo" || p.ToLower() == "code" || p.ToLower() == "pcode");

            if (codeName != null)
            {
                result.Code = parameters.Get<int>(codeName);
            }

            return result;
        }

        private ResultList<T> GetResultList<T>(DynamicParameters? parameters)
        {
            var r = GetResult(parameters);

            var result = new ResultList<T>()
            {
                Value = r.Value,
                Message = r.Message
            };

            return result;
        }        
        
        public async Task<Result> ExecuteAsync(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using(var conexion = new SqlConnection(connectionString))
            {
                var p = TransformParams(parameters);

                await conexion.ExecuteAsync(query, p, commandType: commandType, commandTimeout: commandTimeout);  
                
                return GetResult(p);
            }
        }

        public async Task<Result> ExecuteAsync(string query, object? param, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {   
                await conexion.ExecuteAsync(query, param, commandType: commandType, commandTimeout: commandTimeout);

                return GetResult(null);
            }
        }

        public async Task ExecuteWithResultsAsync(string query, ConexionParameters? parameters, Action<ConexionCastableRow> action, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = TransformParams(parameters);

                var reader = await conexion.ExecuteReaderAsync(query, p, commandType: commandType, commandTimeout: commandTimeout);

                while (reader.Read())
                {
                    var castableRow = new ConexionCastableRow(reader);

                    action(castableRow);
                }                
            }
        }

        public async Task ExecuteWithResultsAsync(string query, ConexionParameters? parameters, DataTable dataTable, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = TransformParams(parameters);

                var reader = await conexion.ExecuteReaderAsync(query, p, commandType: commandType, commandTimeout: commandTimeout);

                dataTable.Load(reader); 
            }
        }

        public async Task<ResultList<T>> ExecuteWithResultsAsync<T>(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = TransformParams(parameters);

                var records = await conexion.QueryAsync<T>(query, p, commandType: commandType, commandTimeout: commandTimeout);

                var result = GetResultList<T>(p);
                result.Data = records.ToList();

                return result;
            }
        }

        public async Task<Result<List<T>>> ExecuteWithResultsAsync<T>(string query, object? param, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {

                var records = await conexion.QueryAsync<T>(query, param, commandType: commandType, commandTimeout: commandTimeout);

                var result = GetResultList<T>(null);
                result.Data = records.ToList();

                return result;
            }
        }



        public async Task ExecuteWithMultipleResultsAsync(string query, ConexionParameters? parameters, Func<ConexionMultipleReader, Task> readerFunc, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = TransformParams(parameters);

                using(var multi = await conexion.QueryMultipleAsync(query, p, commandType: commandType, commandTimeout: commandTimeout))
                {
                    var cr = new ConexionMultipleReader(multi);

                    await readerFunc(cr);
                }
            }
        }

        public async Task ExecuteWithMultipleResultsAsync(string query, object? param, Func<ConexionMultipleReader, Task> readerFunc, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                using (var multi = await conexion.QueryMultipleAsync(query, param, commandType: commandType, commandTimeout: commandTimeout))
                {
                    var cr = new ConexionMultipleReader(multi);

                    await readerFunc(cr);
                }
            }
        }

        public async Task<T?> ExecuteToObjectAsync<T>(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = TransformParams(parameters);

                var result = await conexion.QuerySingleOrDefaultAsync<T>(query, p, commandType: commandType, commandTimeout: commandTimeout);

                return result;
            }
        }

        public async Task<T?> ExecuteToObjectAsync<T>(string query, object? param, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var result = await conexion.QuerySingleOrDefaultAsync<T>(query, param, commandType: commandType, commandTimeout: commandTimeout);

                return result;
            }
        }


    }
}
