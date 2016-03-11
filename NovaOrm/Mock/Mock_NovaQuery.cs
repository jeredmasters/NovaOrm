using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class Mock_NovaQuery : NovaQuery, INovaQuery
    {
        object _returnVal;
        INovaQuery _innerQuery;

        public Mock_NovaQuery(object returnVal, string table, string action) :
            base(table, action)
        {
            _returnVal = returnVal;
        }

        public Mock_NovaQuery(object returnVal, INovaQuery table, string action) :
            base(table, action)
        {
            _returnVal = returnVal;
            _innerQuery = table;
        }

        public bool Any()
        {
            return (bool)_returnVal;
        }

        public int Count()
        {
            return (int)_returnVal;
        }
        
        public int Execute(bool handleException)
        {
            return (int)_returnVal;
        }

        public Dictionary<string, object> First()
        {
            return (Dictionary<string, object>)_returnVal;
        }

        public INovaResult Result()
        {
            return (INovaResult)_returnVal;
        }

        public object Scalar()
        {
            return _returnVal;
        }

        public bool HasColumn(string column)
        {
            return _columns.Any(t => t.Name.Equals(column, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool HasWhereLike(string where)
        {
            return _where.Any(t => t.ToString().Contains(where));
        }

        public bool HasTable(string table)
        {
            return _tableName == table;
        }

        public bool HasAction(string action)
        {
            return _action == action;
        }

        public INovaQuery InnerQuery()
        {
            return _innerQuery;
        }


    }
}
