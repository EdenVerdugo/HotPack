using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class DateTimeExtensions
    {
        public static DateTime Default(this DateTime d)
        {
            return new DateTime(1900, 1, 1);
        }
    }
}
