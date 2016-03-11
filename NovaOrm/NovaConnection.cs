using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaConnection
    {
        SqlConnection _connection;
        SqlDataReader _reader;

        Guid _connectionId;

        bool _connecting = false;

        public NovaConnection(string connecitonString)
        {
            _connection = new SqlConnection(connecitonString);
            _reader = null;
        }

        public bool Available()
        {
            if (_connection.State == System.Data.ConnectionState.Open && !_connecting)
            {
                if (_reader == null || _reader.IsClosed)
                {
                    return true;
                }
            }
            return false;
        }

        public void Open()
        {
            _connection.Open();
            _connectionId = _connection.ClientConnectionId;
        }

        public void Close()
        {
            if (_reader != null && !_reader.IsClosed)
            {
                _reader.Close();                
            }
            _connection.Close();
        }

        public int Execute(string command)
        {
            if (!Available())
            {
                throw new Exception("Connection in use");
            }
            _connecting = true;
            SqlCommand com = new SqlCommand(command,_connection);
            com.CommandTimeout = 3000;
            int affected = com.ExecuteNonQuery();
            _connecting = false;
            return affected;
        }

        public object Scalar(string command)
        {
            if (!Available())
            {
                throw new Exception("Connection in use");
            }
            _connecting = true;
            SqlCommand com = new SqlCommand(command, _connection);
            object scalar = com.ExecuteScalar();
            _connecting = false;
            return scalar;
        }

        public SqlDataReader Reader(string query) 
        {
            if (!Available())
            {
                throw new Exception("Connection in use");
            }

            _connecting = true;
            SqlCommand com = new SqlCommand(query, _connection);
            _reader = com.ExecuteReader();
            _connecting = false;

            return _reader;
        }
    }
}
