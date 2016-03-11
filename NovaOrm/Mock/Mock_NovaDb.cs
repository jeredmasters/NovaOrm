using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class Mock_NovaDb : INovaDb
    {
        object _returlVal;
        public NovaOrm.NovaDb.ExceptionThrown ExceptionHandler { get; set; }
        Mock_NovaQuery _query = null;

        public Mock_NovaDb(object returnVal)
        {
            _returlVal = returnVal;
        }

        public void Connect()
        {
            
        }

        public string LastQueryString()
        {
            if (_query != null)
            {
                string retval = _query.BuildString();
                retval = retval.Replace(Environment.NewLine, " ");               

                while (retval.Contains("  "))
                {
                    retval = retval.Replace("  ", " ");
                }

                retval = retval.Replace("( ", "(");
                retval = retval.Replace(" )", ")");

                return retval.Trim();
            }
            return "No Query";
        }

        public Mock_NovaQuery LastQuery()
        {
            return _query;
        }

        public void UpdateReturnVal(object returnVal)
        {
            _returlVal = returnVal;
        }

        public INovaQuery Create(string tableName)
        {
            _query = new Mock_NovaQuery(_returlVal, tableName, "CREATE");
            return _query;
        }

        public INovaQuery Delete(string table)
        {
            _query = new Mock_NovaQuery(_returlVal, table, "DELETE");
            return _query;
        }

        public INovaQuery Update(string table)
        {
            _query = new Mock_NovaQuery(_returlVal, table, "UPDATE");
            return _query;
        }

        public void Disconnect()
        {
            
        }

        public INovaQuery Drop(string table)
        {
            _query = new Mock_NovaQuery(_returlVal, table, "DROP");
            return _query;
        }

        public INovaTable Table(string table, string identity)
        {
            throw new NotImplementedException();
        }

        public int Execute(string query, bool handleException)
        {
            return (int)_returlVal;
        }

        public INovaQuery Insert(string tableName)
        {
            _query = new Mock_NovaQuery(_returlVal, tableName, "INSERT");
            return _query;
        }

        public System.Data.SqlClient.SqlDataReader Query(string query)
        {
            throw new NotImplementedException();
        }

        public object Scalar(string query)
        {
            return _returlVal;
        }

        public INovaQuery Select(INovaQuery table)
        {
            _query = new Mock_NovaQuery(_returlVal, table, "SELECT");
            return _query;
        }

        public INovaQuery Select(string table)
        {
            _query = new Mock_NovaQuery(_returlVal, table, "SELECT");
            return _query;
        }

        public bool TableExists(string tableName)
        {
            return true;
        }
    }
}
