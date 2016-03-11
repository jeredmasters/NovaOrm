using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaDb : NovaOrm.INovaDb
    {
        SqlManager _sqlManager;
        public delegate void ExceptionThrown(string sqlQuery, Exception exception);
        public ExceptionThrown ExceptionHandler {get; set;}

        long _ticksOffset;

        public NovaDb(string connectionString)
        {
            _sqlManager = new SqlManager(connectionString);
            _sqlManager.Connect();

            DateTime psudoNow = DateTime.Parse("2015-10-20 18:57:06.280");
            TimeSpan difference = psudoNow - DateTime.Now;

            _ticksOffset = difference.Ticks;
        }

        public void Connect()
        {
            _sqlManager.Connect();
        }

        public void Disconnect()
        {
            _sqlManager.Disconnect();
        }

        public SqlDataReader Query(string query)
        {
            try
            {
                return _sqlManager.GetReader(query);
            }
            catch (Exception ex)
            {
                ThrowExcpetion(query, ex);
                throw ex;
            }
        }

        public int Execute(string query, bool handleExceptions = true)
        {
            try
            {
                return _sqlManager.Execute(query);
            }
            catch (Exception ex)
            {
                if (handleExceptions)
                {
                    ThrowExcpetion(query, ex);
                }
                throw ex;
            }
        }

        public object Scalar(string query)
        {
            try
            {
                return _sqlManager.Scalar(query);
            }
            catch (Exception ex)
            {
                ThrowExcpetion(query, ex);
                throw ex;
            }
        }

        public INovaQuery Insert(string tableName)
        {
            return Table(tableName).Insert();
        }

        public INovaQuery Select(string tableName)
        {
            return Table(tableName).Select();
        }

        public INovaQuery Select(INovaQuery table)
        {
            return Table(table).Insert();
        }

        public INovaQuery Create(string tableName)
        {
            return Table(tableName).Create();
        }

        public INovaQuery Drop(string tableName)
        {
            return Table(tableName).Drop();
        }

        public INovaQuery Update(string tableName)
        {
            return Table(tableName).Update();
        }

        public INovaQuery Delete(string tableName)
        {
            return Table(tableName).Delete();
        }

        public INovaTable Table(string table, string identity = null)
        {
            return new NovaTable(this, table, identity);
        }

        public INovaTable Table(INovaQuery table, string identity = null)
        {
            return new NovaTable(this, table, identity);
        }

        public bool TableExists(string tableName)
        {
            var query = Select("[INFORMATION_SCHEMA].[TABLES]").Where("TABLE_TYPE", "BASE TABLE").Where("TABLE_NAME", tableName);

            bool result = query.Any();

            return result;
        }

        public static bool Test(string connectionString, bool throwEx = false) {
            bool success = true;

            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                con.Close();
            }
            catch(Exception ex)
            {
                success = false;
                if (throwEx)
                {
                    throw ex;
                }
            }

            return success;
        }

        public void ThrowExcpetion(string sqlQuery, Exception ex)
        {
            if (ExceptionHandler != null){
                ExceptionHandler(sqlQuery,ex);
            }
            
        }
    }


}
