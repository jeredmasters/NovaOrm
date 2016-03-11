using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class Mock_NovaTable : INovaResult
    {
        public void AddRow(NovaEntity row)
        {
            
        }

        public NovaSmoothing Smoothing()
        {
            return null;
        }

        public bool Read()
        {
            return false;
        }

        public object this[int column]
        {
            get{
                return null;
            }            
        }

        public object this[int row, int col]
        {
            get
            {
                return null;
            }   
        }

        public object this[int row, string column]
        {
            get
            {
                return null;
            }   
        }

        public object this[string column]
        {
            get
            {
                return null;
            }   
        }

        public string ToCsv()
        {
            return "";
        }
        public string ToJson()
        {
            return "";
        }
        public int Count()
        {
            return 0;
        }
        public int Min(string col)
        {
            return 0;
        }
        public int Max(string col)
        {
            return 0;
        }
        public IEnumerable<string> GetColumns()
        {
            return new string[] { };
        }

        public IEnumerator<NovaEntity> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
