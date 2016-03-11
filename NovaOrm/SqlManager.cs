using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class SqlManager
    {
        List<NovaConnection> _connections = new List<NovaConnection>();
        List<string> CommandList = new List<string>();
        List<string> Tables = new List<string>();

        int idleConnections = 2;

        int maxAttempts = 5;

        int _currentConnection = 0;

        int _successful = 0;

        

        System.Timers.Timer commandtmr = new System.Timers.Timer(200);

        string _connectionString;

        public SqlManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Connect()
        {
            for (int i = 0; i < idleConnections; i++)
            {
                _connections.Add(new NovaConnection(_connectionString));
            }

            foreach (NovaConnection connection in _connections)
            {
                connection.Open();
            }
        }
        public void Disconnect()
        {
            foreach (NovaConnection connection in _connections)
            {
                connection.Close();
            }
        }
        private NovaConnection Next()
        {
            _currentConnection++;
            if (_currentConnection >= idleConnections)
            {
                _currentConnection = 0;
            }
            return _connections[_currentConnection];

        }
        public NovaConnection GetConnection(int attempt = 1)
        {
            for(int i = 0; i < _connections.Count; i++){
                NovaConnection connection = Next();
                if (connection.Available())
                {
                    _successful++;
                    return connection;
                }
            }

            if (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt + (_currentConnection * 7));
                attempt++;
                return GetConnection(attempt);
            }
            throw new Exception("Could not get connection. Max:" + idleConnections + " Successful:" + _successful);
        }
        public static bool Available(System.Data.ConnectionState state)
        {
            return (state != System.Data.ConnectionState.Executing && state != System.Data.ConnectionState.Fetching && state != System.Data.ConnectionState.Connecting && state != System.Data.ConnectionState.Closed && state != System.Data.ConnectionState.Broken);
        }

        public bool CheckTable(string tablename)
        {
            foreach (string table in Tables)
            {
                if (String.Compare(table, tablename, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public object Scalar(string command)
        {
            return GetConnection().Scalar(command);
        }
        public int Execute(string command)
        {
            int affected = 0;
            var connection = GetConnection();
            affected = connection.Execute(command);
            return affected;
        }

        public SqlDataReader GetReader(string query)
        {
            var connection = GetConnection();
            return connection.Reader(query);
        }      

    }
}
