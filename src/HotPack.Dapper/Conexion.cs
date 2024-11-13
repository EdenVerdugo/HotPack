using Dapper;
using HotPack.Classes;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        static string[] resultsNames = { "result", "presult", "resultado", "presultado" };
        static string[] msgNames = { "msg", "pmsg", "message", "pmessage", "mensaje", "pmensaje" };
        static string[] codeNames = { "codigo", "pcodigo", "code", "pcode" };


        //private static DynamicParameters? TransformParams(ConexionParameters? parameters)
        //{
        //    if (parameters == null) return null;

        //    var dapperParameters = new DynamicParameters();

        //    foreach (var p in parameters.Parameters)
        //    {
        //        dapperParameters.Add(p.Name, p.Value, p.Type, p.Direction, p.Size);
        //    }

        //    return dapperParameters;
        //}

        private static Result GetResult(DynamicParameters? parameters)
        {
            var result = new Result(true, "the query has been executed correctly");

            if (parameters is null) return result;

            //var resultName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "result" || p.ToLower() == "presult" || p.ToLower() == "resultado" || p.ToLower() == "presultado");
            var resultName = parameters.ParameterNames.FirstOrDefault(p => resultsNames.Any(palabra => p.Contains(palabra, StringComparison.OrdinalIgnoreCase)));

            if (resultName != null)
            {
                result.Value = parameters.Get<bool>(resultName);
            }

            //var messageName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "msg" || p.ToLower() == "pmsg" || p.ToLower() == "message" || p.ToLower() == "pmessage" || p.ToLower() == "mensaje" || p.ToLower() == "pmensaje");
            var messageName = parameters.ParameterNames.FirstOrDefault(p => msgNames.Any(palabra => p.Contains(palabra, StringComparison.OrdinalIgnoreCase)));

            if (messageName != null)
            {
                result.Message = parameters.Get<string>(messageName);
            }

            //var codeName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "codigo" || p.ToLower() == "pcodigo" || p.ToLower() == "code" || p.ToLower() == "pcode");
            var codeName = parameters.ParameterNames.FirstOrDefault(p => codeNames.Any(palabra => p.Contains(palabra, StringComparison.OrdinalIgnoreCase)));

            if (codeName != null)
            {
                result.Code = parameters.Get<int>(codeName);
            }

            return result;
        }
        private static Result<T> GetResult<T>(DynamicParameters? parameters)
        {
            var result = new Result<T>(true, "the query has been executed correctly");

            if(parameters is null) return result;

            //var resultName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "result" || p.ToLower() == "presult" || p.ToLower() == "resultado" || p.ToLower() == "presultado");
            var resultName = parameters.ParameterNames.FirstOrDefault(p => resultsNames.Any(palabra => p.Contains(palabra, StringComparison.OrdinalIgnoreCase)));

            if (resultName != null)
            {
                result.Value = parameters.Get<bool>(resultName);
            }

            //var messageName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "msg" || p.ToLower() == "pmsg" || p.ToLower() == "message" || p.ToLower() == "pmessage" || p.ToLower() == "mensaje" || p.ToLower() == "pmensaje");
            var messageName = parameters.ParameterNames.FirstOrDefault(p => msgNames.Any(palabra => p.Contains(palabra, StringComparison.OrdinalIgnoreCase)));

            if (messageName != null)
            {
                result.Message = parameters.Get<string>(messageName);
            }

            //var codeName = parameters.ParameterNames.FirstOrDefault(p => p.ToLower() == "codigo" || p.ToLower() == "pcodigo" || p.ToLower() == "code" || p.ToLower() == "pcode");
            var codeName = parameters.ParameterNames.FirstOrDefault(p => codeNames.Any(palabra => p.Contains(palabra, StringComparison.OrdinalIgnoreCase)));

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

        private void AddToBag<T>(ConexionParameters? parameters, Result<T> result)
        {
            var iparametros = parameters?.GetParameters()
                .Where(p => !resultsNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && !msgNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && !codeNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && p.Direction == ParameterDirection.Output);

            foreach (var param in iparametros ?? [])
            {
                if (param.Direction == ParameterDirection.Output)
                {
                    if (!string.IsNullOrWhiteSpace(param.BagAlias))
                    {
                        var value = parameters?.Get<object>(param.Name);

                        result.Bag.Add(param.BagAlias ?? param.Name, value ?? "");
                    }                    
                }
            }
        }

        private void AddToBag<T>(ConexionParameters? parameters, ResultList<T> result)
        {
            var iparametros = parameters?.GetParameters()
                .Where(p => !resultsNames.Any(palabra => p.Name.ToLower() == palabra.ToLower()) 
                            && !msgNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && !codeNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && p.Direction == ParameterDirection.Output);

            foreach (var param in  iparametros ?? [])
            {
                if (param.Direction == ParameterDirection.Output)
                {
                    if (!string.IsNullOrWhiteSpace(param.BagAlias))
                    {
                        var value = parameters?.Get<object>(param.Name);

                        result.Bag.Add(param.BagAlias ?? param.Name, value ?? "");
                    }
                }
            }
        }

        private void AddToBag(ConexionParameters? parameters, Result result)
        {
            var iparametros = parameters?.GetParameters()
                .Where(p => !resultsNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && !msgNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && !codeNames.Any(palabra => p.Name.ToLower() == palabra.ToLower())
                            && p.Direction == ParameterDirection.Output);

            foreach (var param in iparametros ?? [])
            {
                if (param.Direction == ParameterDirection.Output)
                {
                    if (!string.IsNullOrWhiteSpace(param.BagAlias))
                    {
                        var value = parameters?.Get<object>(param.Name);

                        result.Bag.Add(param.BagAlias ?? param.Name, value ?? "");
                    }
                }
            }
        }

        public async Task<Result> ExecuteAsync(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using(var conexion = new SqlConnection(connectionString))
            {
                var p = parameters?.GetDynamicParameters();

                await conexion.ExecuteAsync(query, p, commandType: commandType, commandTimeout: commandTimeout);  
                
                var result = GetResult(p);

                AddToBag(parameters, result);

                return result;
            }
        }

        public async Task ExecuteWithResultsAsync(string query, ConexionParameters? parameters, Action<ConexionCastableRow> action, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = parameters?.GetDynamicParameters();

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
                var p = parameters?.GetDynamicParameters();

                var reader = await conexion.ExecuteReaderAsync(query, p, commandType: commandType, commandTimeout: commandTimeout);

                dataTable.Load(reader); 
            }
        }

        public async Task<ResultList<T>> ExecuteWithResultsAsync<T>(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = parameters?.GetDynamicParameters();

                var records = await conexion.QueryAsync<T>(query, p, commandType: commandType, commandTimeout: commandTimeout);

                var result = GetResultList<T>(p);
                result.Data = records.ToList();

                AddToBag<T>(parameters, result);

                return result;
            }
        }

        public async Task<Result> ExecuteWithMultipleResultsAsync(string query, ConexionParameters? parameters, Func<ConexionMultipleReader, Task> readerFunc, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = parameters?.GetDynamicParameters();

                using (var multi = await conexion.QueryMultipleAsync(query, p, commandType: commandType, commandTimeout: commandTimeout))
                {
                    var cr = new ConexionMultipleReader(multi);

                    await readerFunc(cr);

                    var result = GetResult(p);                    

                    AddToBag(parameters, result);

                    return result;
                }
            }
        }

        public async Task<Result<T>> ExecuteWithMultipleResultsAsync<T>(string query, ConexionParameters? parameters, Func<ConexionMultipleReader, Task<T>> readerFunc, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = parameters?.GetDynamicParameters();

                using (var multi = await conexion.QueryMultipleAsync(query, p, commandType: commandType, commandTimeout: commandTimeout))
                {
                    var cr = new ConexionMultipleReader(multi);

                    var data = await readerFunc(cr);

                    var result = GetResult<T>(p);
                    result.Data = data;

                    AddToBag<T>(parameters, result);

                    return result;
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
                var p = parameters?.GetDynamicParameters();

                var result = await conexion.QuerySingleOrDefaultAsync<T>(query, p, commandType: commandType, commandTimeout: commandTimeout);

                return result;
            }
        }

        public async Task<T?> ExecuteScalarAsync<T>(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var p = parameters?.GetDynamicParameters();

                var result = await conexion.ExecuteScalarAsync<T>(query, p, commandType: commandType, commandTimeout: commandTimeout);

                return result;
            }
        }
    }
}
