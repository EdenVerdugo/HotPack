using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Dapper
{
    public class ConexionParameter
    {
        public string Name { get; set; } = string.Empty;
        public DbType Type { get; set; }
        public object? Value { get; set; }
        public int Size { get; set; }
        public ParameterDirection Direction { get; set; }

        public ConexionParameter()
        {
            Name = "";
            Type = DbType.String;
            Size = 0;
            Direction = ParameterDirection.Input;
        }

        public ConexionParameter(string name, ConexionDbType type, object value, int size = 0, ParameterDirection direction = ParameterDirection.Input)
        {
            Name = name;
            Type = (DbType)type;
            Value = value ?? DBNull.Value;
            Size = size;
            Direction = direction;
        }
    }
}
