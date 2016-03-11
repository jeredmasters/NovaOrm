using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaEntity : IEnumerable<NovaField>
    {
        List<NovaField> _data = new List<NovaField>();
        NovaTable _table;
        string _identity;
        object _id;
        bool _exists = false;

        public NovaEntity(NovaTable table)
        {
            _table = table;
            _identity = table.Identity;
        }

        public NovaEntity(NovaTable table, SqlDataReader reader)
        {
            _table = table;
            _identity = table.Identity;
            _exists = true;
            int fieldcount = reader.FieldCount;
            for (int i = 0; i < fieldcount; i++)
            {
                object val = reader[i];
                if (val == DBNull.Value)
                {
                    val = null;
                }
                _data.Add(
                    new NovaField
                    {
                        Name = reader.GetName(i),
                        Value = val,
                        Edited = false
                    });
                if (reader.GetName(i) == _identity)
                {
                    _id = val;
                }
            }
        }

        public object this[string index]
        {
            get
            {
                NovaField field = _data.FirstOrDefault(t => t.Name == index);
                return field.Value;
            }
            set
            {
                if (index == _identity)
                {
                    if (_exists)
                    {
                        throw new Exception("Cannot change the identity of an object that exists");
                    }
                    _id = (object)value;
                }
                NovaField field = _data.FirstOrDefault(t => t.Name == index);
                if (field == default(NovaField))
                {
                    field = new NovaField
                    {
                        Name = index
                    };
                    _data.Add(field); //passed by reference
                }
                field.Value = value;
                field.Edited = true;
                
            }
        }

        public object Id
        {
            get
            {
                return _id;
            }
        }

        public object Save()
        {
            IEnumerable<NovaField> savable = _data.Where(t => t.Edited);
            if (savable.Count() > 0)
            {
                INovaQuery query;

                if (_exists)
                {
                    query = _table.Update()
                        .Where(_identity, _id);
                }
                else
                {
                    query = _table.Insert();
                    if (_id != null) {
                        query.Column(_identity, _id);
                    }
                }

                foreach (NovaField field in savable)
                {
                    if (field.Name != _identity)
                    {
                        query.Column(field.Name, field.Value);
                    }
                }

                _id = query.Scalar(); //should return the new ID 

                return _id;
            }
            return null;
        }

        public void Delete()
        {
            _table.Delete().Where(_identity, _id).Execute();
        }
        public string ToCsv()
        {
            List<string> cells = new List<string>();
            foreach (NovaField field in this)
            {
                cells.Add(field.ToCsv());
            }
            return String.Join(",", cells);
        }
        public string ToJson()
        {
            List<string> cells = new List<string>();
            foreach (NovaField field in this)
            {
                cells.Add(field.ToJson());
            }
            return "{ " + String.Join(",", cells) + " }";
        }

        public IEnumerator<NovaField> GetEnumerator()
        {
            foreach (NovaField entity in _data)
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
