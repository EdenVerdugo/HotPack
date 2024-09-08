using HotPack.Classes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Dapper
{
    public class ConexionCastableRow
    {
        private readonly DbDataReader reader;

        public ConexionCastableRow(DbDataReader reader)
        {
            this.reader = reader;
        }

        public Castable this[int columnIndex]
        {
            get
            {
                return new Castable(this.reader[columnIndex]);
            }
        }

        public Castable this[string columnName]
        {
            get
            {
                return new Castable(this.reader[columnName]);
            }
        }
        public Castable Column(string columnName)
        {
            return this[columnName];
        }

        public Castable Column(int columnIndex)
        {
            return this[columnIndex];
        }
    }
}
