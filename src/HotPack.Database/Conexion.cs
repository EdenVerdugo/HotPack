using HotPack.Classes;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;

namespace HotPack.Database
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

        private static Result GetResult(ConexionParameters? parameters)
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
        private static Result<T> GetResult<T>(ConexionParameters? parameters)
        {
            var result = new Result<T>(true, "the query has been executed correctly");

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

        private ResultList<T> GetResultList<T>(ConexionParameters? parameters)
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

        public async Task<Result> ExecuteAsync(string query, ConexionParameters? parameters = null, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var rows = await cmd.ExecuteNonQueryAsync();

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult(parameters);

            AddToBag(parameters, result);

            return result;
        }

        public async Task<Result> ExecuteWithResultsAsync(string query, ConexionParameters? parameters, Action<ConexionCastableRow> action, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                var castableRow = new ConexionCastableRow(reader);

                action(castableRow);
            }

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult(parameters);

            AddToBag(parameters, result);

            return result;
        }

        public async Task<Result> ExecuteWithResultsAsync(string query, ConexionParameters? parameters, DataTable dataTable, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            dataTable.Load(reader);

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult(parameters);

            AddToBag(parameters, result);

            return result;
        }        

        private List<T> ResultsInDataReader<T>(ref SqlDataReader reader)
        {
            List<T> lst = new List<T>();
            string? currentColumn = null;

            try
            {
                Type temp = typeof(T);

                while (reader.Read())
                {
                    //crear la instancia del objeto 
                    T obj = default(T)!;
                    bool isPrimitive = true;
                    if (temp != typeof(string) && temp != typeof(int) && temp != typeof(decimal) && temp != typeof(long) && temp != typeof(bool) && temp != typeof(short) && temp != typeof(float) && temp != typeof(double) && temp != typeof(char))
                    {
                        obj = Activator.CreateInstance<T>();
                        isPrimitive = false;
                    }


                    //buscar todos sus campos y propiedades
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader[i].GetType() == typeof(System.DBNull))
                            continue;

                        if (isPrimitive)
                        {
                            obj = (T)reader[i];
                            continue;
                        }

                        foreach (FieldInfo pro in temp.GetFields())
                        {
                            string name = pro.Name.ToLowerInvariant();
                            string columnName = reader.GetName(i).ToLowerInvariant();
                            currentColumn = columnName;

                            var attributes = pro.GetCustomAttributes(true);

                            foreach (var a in attributes)
                            {
                                if (a is ConexionColumnAttribute)
                                {
                                    name = ((ConexionColumnAttribute)a).Name.ToLowerInvariant();
                                }
                            }
                            // si se encuentra se asigna el valor encontrado
                            // nota : esto puede causar una excepcion si el tipo de valor que se encuentra no es el mismo tipo que regresa la consulta
                            if (name == columnName)
                            {
                                pro.SetValue(obj, reader[i]);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        foreach (PropertyInfo pro in temp.GetProperties())
                        {
                            string propertyName = pro.Name.ToLowerInvariant();
                            string columnName = reader.GetName(i).ToLowerInvariant();
                            currentColumn = columnName;

                            var attributes = pro.GetCustomAttributes(true);

                            foreach (var a in attributes)
                            {
                                if (a is ConexionColumnAttribute)
                                {
                                    propertyName = ((ConexionColumnAttribute)a).Name.ToLowerInvariant();
                                }
                            }

                            if (propertyName == columnName)
                            {
                                pro.SetValue(obj, reader[i], null);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    lst?.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Falló al serializar la columna '{currentColumn}'.\n{ex.Message}", ex);
            }

            return lst!;
        }

        public async Task<ResultList<T>> ExecuteWithResultsAsync<T>(string query, ConexionParameters? parameters = null, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            var lst = ResultsInDataReader<T>(ref reader);

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResultList<T>(parameters);
            result.Data = lst;

            AddToBag(parameters, result);

            return result;
        }
        
        public async Task<Result> ExecuteWithMultipleResultsAsync(string query, ConexionParameters? parameters, Func<ConexionMultipleReader, Task> readerFunc, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            var cr = new ConexionMultipleReader(reader);

            await readerFunc(cr);            

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult(parameters);            

            AddToBag(parameters, result);

            return result;           
        }

        public async Task<Result<T>> ExecuteWithMultipleResultsAsync<T>(string query, ConexionParameters? parameters, Func<ConexionMultipleReader, Task<T>> readerFunc, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            var cr = new ConexionMultipleReader(reader);

            var data = await readerFunc(cr);

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult<T>(parameters);
            result.Data = data;

            AddToBag<T>(parameters, result);

            return result;
        }

        public async Task<Result<T?>> ExecuteToObjectAsync<T>(string query, ConexionParameters? parameters, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            var lst = ResultsInDataReader<T>(ref reader);

            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult<T>(parameters);
            result.Data = lst.FirstOrDefault();

            AddToBag(parameters, result);

            return result!;
        }

        public async Task<Result<T>> ExecuteScalarAsync<T>(string query, ConexionParameters? parameters = null, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = 30)
        {
            using var conexion = new SqlConnection(connectionString);

            await conexion.OpenAsync();

            using var cmd = conexion.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = commandType ?? CommandType.StoredProcedure;
            cmd.CommandTimeout = commandTimeout ?? 30;

            if (parameters != null)
            {
                foreach (var p in parameters.GetParameters())
                {
                    var pt = cmd.CreateParameter();
                    pt.ParameterName = p.Name;
                    pt.DbType = p.Type;
                    pt.Value = p.Value;
                    pt.Size = p.Size;
                    pt.Direction = p.Direction;

                    cmd.Parameters.Add(pt);
                }
            }

            var scalar = await cmd.ExecuteScalarAsync();
            
            await conexion.CloseAsync();

            if (parameters != null)
            {
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    var param = parameters.GetParameters().FirstOrDefault(x => x.Name == p.ParameterName);
                    if (param != null)
                    {
                        param.Value = p.Value;
                    }
                }
            }

            var result = GetResult<T>(parameters);

            result.Data = (T)Convert.ChangeType(scalar!, typeof(T));

            AddToBag(parameters, result);

            return result;
        }
    }
}
