using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Data
{
    public sealed class ConnectionString
    {
        public ConnectionString()
        {

        }

        public ConnectionString(string server, string database, string user, string password)
        {
            Server = server;
            Database = database;
            User = user;
            Password = password;
        }

        public string? Server { get; set; } = string.Empty;
        public string? Database { get; set; } = string.Empty;
        public string? User { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;

        public string ToMsqlConnectionString()
        {
            return $"data source = {Server}; initial catalog = {Database}; user id = {User}; password = {Password}";
        }

    }
}
