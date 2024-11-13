using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Database
{
    public class ConexionMultipleReader
    {
        public readonly SqlDataReader reader;
        private bool next = true;

        public ConexionMultipleReader(SqlDataReader reader)
        {
            this.reader = reader;
        }

        //public T? ReadSingleOrDefault<T>()
        //{
        //    return reader.ReadSingleOrDefault<T>();
        //}

        public async Task<T?> ResultsToObjectAsync<T>()
        {
            if (!next)
            {
                throw new Exception("No hay mas recordsets que leer");
            }

            return await Task.Run<T?>(() =>
            {
                var list = ResultsInDataReader<T>();

                var item = list.FirstOrDefault();
                
                next = reader.NextResult();

                return item;
            });
        }

        //public List<T> Read<T>()
        //{
        //    return reader.Read<T>().ToList();
        //}

        public async Task<List<T>> ResultsAsync<T>()
        {
            if (!next)
            {
                throw new Exception("No hay mas recordsets que leer");
            }
            
            return await Task.Run<List<T>>(() =>
            {
                var list = ResultsInDataReader<T>();

                next = reader.NextResult();

                return list;
            });
        }


        private List<T> ResultsInDataReader<T>()
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
    }
}
