using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaResult : NovaOrm.INovaResult, IEnumerable<NovaEntity>
    {
        List<NovaEntity> _rows = new List<NovaEntity>();
        string[] _columnNames;
        NovaSmoothing _smoothing;

        //List<object[]> _data;

        int _currentRow = -1;

        public NovaResult(string[] columnNames, NovaSmoothing smoothing = null)
        {
            _columnNames = columnNames;
            _smoothing = smoothing;
        }
        public NovaSmoothing Smoothing()
        {
            return _smoothing;
        }
        public void AddRow(NovaEntity row)
        {
            _rows.Add(row);
        }
        public object this[string column]
        {
            get
            {
                return this[_currentRow, column];
            }
        }
        public object this[int row, string column]
        {
            get{
                return _rows[row][column];
            }
        }
        private int GetColNum(string name)
        {
            int count = _columnNames.Count();
            for (int i = 0; i < count; i++)
            {
                if (_columnNames[i] == name)
                {
                    return i;
                }
            }
            throw new Exception("Unrecognised Column Name");
        }
        public IEnumerable<string> GetColumns()
        {
            return _columnNames;
        }
        public bool Read()
        {
            _currentRow++;
            return _currentRow < _rows.Count;
        }

        public int Max(string column)
        {
            int max = int.MinValue;
            _currentRow = -1;

            while (this.Read())
            {
                if ((int)this[column] > max)
                {
                    max = (int)this[column];
                }
            }

            return max;
        }

        public int Min(string column)
        {
            int min = int.MaxValue;
            _currentRow = -1;

            while (this.Read())
            {
                if ((int)this[column] < min)
                {
                    min = (int)this[column];
                }
            }

            return min;
        }

        public string ToCsv()
        {
            List<string> lines = new List<string>();
            lines.Add(StringHelpers.SeperateByComma(_columnNames));
            
            foreach(NovaEntity row in this){
                List<string> cells = new List<string>();
                foreach (NovaField cell in row)
                {
                    cells.Add(cell.ToCsv());
                }
                lines.Add(String.Join(",",cells));
            }

            return string.Join("\n",lines);
        }

        public string ToJson()
        {
            List<string> lines = new List<string>();
            foreach(NovaEntity entity in this)
            {                
                lines.Add(entity.ToJson());
            }
            return "[ " + String.Join(", ", lines) + " ]";
        }

        public int Count()
        {
            return _rows.Count();
        }

        public IEnumerator<NovaEntity> GetEnumerator()
        {
            foreach(NovaEntity entity in _rows)
            {
                if (false)
                {
                    continue; //skip 
                }

                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
