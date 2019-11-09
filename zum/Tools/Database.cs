using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Threading.Tasks;

namespace zum
{
    public static class Database
    {
        private static string _conStr;
        public static string ConnectionString
        {
            get { return _conStr; }
            set
            {
                MySqlConnection.ClearAllPoolsAsync();
                connection = new MySqlConnection(value);
                _conStr = value;
            }
        }

        public static MySqlConnection connection { get; private set; }

        public static async Task<DbDataReader> RunQuery(string sqlstring, params MySqlParameter[] parameters)
        {
            if (connection == null) return null;
            await connection.OpenAsync();
            using (var cmd = connection.CreateCommand())
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 5;
                return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
            }
        }

        public static async Task<object> QueryRow(string sqlstring, params MySqlParameter[] parameters)
        {
            if (connection == null) return null;
            await connection.OpenAsync();
            using (var cmd = connection.CreateCommand())
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 5;
                object x = await cmd.ExecuteScalarAsync();
                await connection.CloseAsync();
                return x;
            }
        }

        public static async Task<long> Execute(string sqlstring, params dynamic[] parameters)
        {
            if (connection == null) return 0;
            await connection.OpenAsync();
            using (var cmd = connection.CreateCommand())
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 5;
                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                return cmd.LastInsertedId;
            }
        }
    }
}
