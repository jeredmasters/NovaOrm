using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaTable
    {
        NovaDb _db;
        string _tableName;
        INovaQuery _tableQuery;
        string _identity = null;

        public NovaTable(NovaDb db, string table, string identity = null)
        {
            _db = db;
            _tableName = table;
            _identity = identity;
        }

        public NovaTable(NovaDb db, INovaQuery table, string identity = null)
        {
            _db = db;
            _tableQuery = table;
            _identity = identity;
        }

        public INovaQuery Insert()
        {
            return getQuery("INSERT");
        }

        public INovaQuery Select()
        {
            return getQuery("SELECT");
        }

        public INovaQuery Create()
        {
            return getQuery("CREATE");
        }

        public INovaQuery Drop()
        {
            return getQuery("DROP");
        }

        public INovaQuery Truncate()
        {
            return getQuery("TRUNCATE");
        }

        public INovaQuery Update()
        {
            return getQuery("UPDATE");
        }

        public INovaQuery Update(string id)
        {
            return getQuery("UPDATE", id);
        }

        public INovaQuery Delete()
        {
            return getQuery("DELETE");
        }

        public bool Exists()
        {
            if (_tableName == null)
            {
                throw new Exception("Cannot test temporary table");
            }
            return _db.TableExists(_tableName);
        }


        public void Set(object id, object something)
        {

            INovaQuery query;
            var obj = this.Find(id);

            if (obj == null)
            {
                query = Insert();
                foreach (var prop in something.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    query.Column(prop.Name, prop.GetValue(something, null));
                }
            }
            else
            {
                query = Update(id.ToString());
                foreach (var prop in something.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    query.Column(prop.Name, prop.GetValue(something, null));
                }
            }

            query.Execute();
        }

        public NovaEntity Find(object id)
        {
            return getQuery("SELECT", id).First();
        }

        public NovaEntity New()
        {
            return new NovaEntity(this);
        }

        private INovaQuery getQuery(string action, object id = null)
        {
            INovaQuery query;
            if (_tableName != null)
            {
                query = new NovaQuery(_db, this, _tableName, action, _identity);
            }
            else if(_tableQuery!=null) {
                query = new NovaQuery(_db, this, _tableQuery, action, _identity);
            }
            else
            {
                throw new ArgumentException("No table specified");
            }
            if (id != null)
            {
                query.Where(_identity, id);
            }
            return query;
        }

        public string Identity
        {
            get
            {
                return _identity;
            }
        }
    }
}
