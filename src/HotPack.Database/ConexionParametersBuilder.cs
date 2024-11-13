using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Database
{
    public class ConexionParametersBuilder
    {
        private ConexionParameters defaultParameters = new ConexionParameters();

        public ConexionParametersBuilder()
        {

        }

        public void AddDefaultParam(string name, ConexionDbType type, object value)
        {
            defaultParameters.Add(new ConexionParameter(name, type, value));
        }
        public void AddDefaultParam(string name, ConexionDbType type, ParameterDirection direction)
        {
            defaultParameters.Add(new ConexionParameter(name, type, null!, type == ConexionDbType.VarChar || type == ConexionDbType.NVarChar ? 300 : 0, direction));
        }
        public void AddDefaultParam(string name, ConexionDbType type, ParameterDirection direction, int size)
        {
            defaultParameters.Add(new ConexionParameter(name, type, null!, size, direction));
        }
        public void AddDefaultParam(ConexionParameters defaultParameters)
        {
            this.defaultParameters = defaultParameters;
        }

        public void AddDefaultOutputParam(string name, ConexionDbType type)
        {
            defaultParameters.Add(new ConexionParameter(name, type, null!, type == ConexionDbType.VarChar || type == ConexionDbType.NVarChar ? 300 : 0, ParameterDirection.Output));
        }

        public void AddDefaultOutputParam(string name, ConexionDbType type, int size)
        {
            defaultParameters.Add(new ConexionParameter(name, type, null!, size, ParameterDirection.Output));
        }

        public ConexionParameters CreateDefault()
        {
            var parameters = new ConexionParameters();

            foreach (var p in this.defaultParameters.GetParameters())
            {
                var dp = new ConexionParameter
                {
                    Name = p.Name,
                    Direction = p.Direction,
                    Value = p.Value,
                    Type = p.Type,
                    Size = p.Size,
                };

                parameters.Add(dp);
            }

            return parameters;
        }
    }
}
