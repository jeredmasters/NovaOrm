using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaResult
    {
        SqlDataReader _reader;
        public NovaResult(SqlDataReader reader)
        {
            _reader = reader;
        }

        public bool Any()
        {
            return _reader.HasRows;
        }

        public IEnumerable Row()
        {
            _reader.Read();
            return _reader;
        }
    }
}
