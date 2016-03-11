using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaWhere
    {


        string _fullString = null;
        string _column;
        string _eval;
        object _value;
        
        List<NovaWhere> _orWhere = new List<NovaWhere>();

        public NovaWhere()
        {

        }

        public NovaWhere(string fullString)
        {
            _fullString = fullString;
        }

        public NovaWhere(string column, object value)
        {
            _column = column;
            _eval = "=";
            _value = value;
        }

        public NovaWhere(string column, string eval, object value)
        {
            _column = column;
            _eval = eval;
            _value = value;
        }

        public NovaWhere OrWhere(NovaWhere orWhere)
        {
            _orWhere.Add(orWhere);
            return this;
        }

        
        public string String(string table = null){
            string value;
            if (_value != null)
            {
                if (_value.GetType() == typeof(NovaQuery) || _value.GetType() == typeof(Mock_NovaQuery))
                {
                    value = '(' + ((NovaQuery)_value).BuildString() + ')';
                }
                else
                {
                    value = StringHelpers.FormatSqlVal(_value);
                }
                if (_fullString == null && _column != null && _eval != null && _value != null)
                {
                    if (table == null)
                    {
                        _fullString = _column + ' ' + _eval + ' ' + value;
                    }
                    else
                    {
                        _fullString = table + '.' + _column + ' ' + _eval + ' ' + value + ' ';
                    }
                }
            }

            foreach (NovaWhere orWhere in _orWhere)
            {
                if (!string.IsNullOrEmpty(_fullString))
                {
                    _fullString += " OR ";
                }
                _fullString += orWhere.String(table);
            }

            return '(' + _fullString + ')';
        }

        public static NovaWhere New(string fullString)
        {
            return new NovaWhere(fullString);
        }
        
        public static NovaWhere New(string column, string eval, string value)
        {
            return new NovaWhere(column, eval, value);
        }
    }
}
