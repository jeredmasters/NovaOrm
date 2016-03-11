using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Edited { get; set; }

        public string ToJson()
        {
            object val = Value;

            if (val != null)
            {
                if (val.GetType() == typeof(DateTime))
                {
                    val = ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (val.GetType() == typeof(string))
                {
                    val = "\"" + val + "\"";
                }
            }

            if (val == null || val.GetType() == typeof(DBNull) || val == DBNull.Value)
            {
                val = "null";
            }

            return "\"" + Name + "\" : " + val;
        }
    }
}
