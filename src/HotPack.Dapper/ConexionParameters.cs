using HotPack.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Dapper
{
    public class ConexionParameters
    {
        public List<ConexionParameter> Parameters { get; set; }        

        public ConexionParameters()
        {
            Parameters = new List<ConexionParameter>();             
        }        

        public void Add(string name, ConexionDbType type, object value)
        {
            Parameters.Add(new ConexionParameter(name, type, value));
        }
        public void Add(string name, ConexionDbType type, ParameterDirection direction)
        {
            Parameters.Add(new ConexionParameter(name, type, null!, type == ConexionDbType.VarChar || type == ConexionDbType.NVarChar ? 300 : 0, direction));
        }
        public void Add(string name, ConexionDbType type, ParameterDirection direction, int size)
        {
            Parameters.Add(new ConexionParameter(name, type, null!, size, direction));
        }           
        public void Add(ConexionParameter parameter)
        {
            Parameters.Add(parameter);
        }

        public void AddOutput(string name, ConexionDbType type)
        {
            Parameters.Add(new ConexionParameter(name, type, null!, type == ConexionDbType.VarChar || type == ConexionDbType.NVarChar ? 300 : 0, ParameterDirection.Output));
        }

        public void AddOutput(string name, ConexionDbType type, int size)
        {
            Parameters.Add(new ConexionParameter(name, type, null!, size, ParameterDirection.Output));
        }

        public Castable Value(string parameterName)
        {
            var p = Parameters.FirstOrDefault(x => x.Name == parameterName);

            if (p == null)
            {
                throw new Exception("No se encontro el parametro.");
            }
            return new Castable(p.Value!);
        }

        public Castable this[string parameterName]
        {
            get
            {
                return Value(parameterName);
            }
        }

        public void Clear()
        {
            Parameters.Clear();
        }
    }
}
