using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Database
{
    public class ConexionParameters
    {
        private List<ConexionParameter> Parameters { get; set; } = new List<ConexionParameter>();

        public string[] ParameterNames
        {
            get
            {
                return Parameters.Select(x => x.Name).ToArray();
            }
        }

        public ConexionParameters()
        {
            Parameters = new List<ConexionParameter>();
            
        }

        public ConexionParameters Add(string name, ConexionDbType type, object value)
        {
            Parameters.Add(new ConexionParameter(name, type, value));            

            return this;
        }
        public ConexionParameters Add(string name, ConexionDbType type, ParameterDirection direction)
        {
            Parameters.Add(new ConexionParameter(name, type, null!, type == ConexionDbType.VarChar || type == ConexionDbType.NVarChar ? 300 : 0, direction));            

            return this;
        }
        public ConexionParameters Add(string name, ConexionDbType type, ParameterDirection direction, int size)
        {
            Parameters.Add(new ConexionParameter(name, type, null!, size, direction));            

            return this;
        }
        public ConexionParameters Add(ConexionParameter parameter)
        {
            Parameters.Add(parameter);
            
            return this;
        }

        public ConexionParameters AddOutput(string name, ConexionDbType type, string bagAlias = "")
        {
            Parameters.Add(new ConexionParameter(name, type, null!, type == ConexionDbType.VarChar || type == ConexionDbType.NVarChar ? 300 : 0, ParameterDirection.Output, bagAlias));
            
            return this;
        }

        public ConexionParameters AddOutput(string name, ConexionDbType type, int size, string bagAlias = "")
        {
            Parameters.Add(new ConexionParameter(name, type, null!, size, ParameterDirection.Output, bagAlias));            

            return this;
        }

        /// <summary>
        /// Obtiene el valor del parametro
        /// </summary>
        /// <typeparam name="T">Tipo de dato devuelto</typeparam>
        /// <param name="name">Nombre del parametro</param>
        /// <returns></returns>
        public T Get<T>(string name)
        {
            // Verifica si Parameters es null o está vacía
            if (this.Parameters == null || !this.Parameters.Any())
            {
                throw new InvalidOperationException("No hay parámetros disponibles.");
            }

            // Usa FirstOrDefault en lugar de Single para evitar excepciones si no hay coincidencias
            var parameter = this.Parameters.FirstOrDefault(x => x.Name == name);

            if (parameter == null)
            {
                throw new KeyNotFoundException($"El parámetro con el nombre '{name}' no fue encontrado.");
            }

            // Intenta convertir el valor a T de manera segura
            try
            {
                return (T)Convert.ChangeType(parameter.Value!, typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"No se pudo convertir el valor del parámetro '{name}' al tipo {typeof(T)}.");
            }
        }       

        public List<ConexionParameter> GetParameters()
        {
            return this.Parameters;
        }

        //public Castable Value(string parameterName)
        //{
        //    var p = Parameters.FirstOrDefault(x => x.Name == parameterName);

        //    if (p == null)
        //    {
        //        throw new Exception("No se encontro el parametro.");
        //    }
        //    return new Castable(p.Value!);
        //}

        //public Castable this[string parameterName]
        //{
        //    get
        //    {
        //        return Value(parameterName);
        //    }
        //}

        public void Clear()
        {
            Parameters.Clear();
        }
    }
}
