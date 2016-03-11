using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaQuery : NovaOrm.INovaQuery
    {
        INovaDb _db;
        NovaTable _table;
        protected string _tableName;
        protected string _action;
        protected int _limit = -1;
        string _distinct = null;
        string _groupBy = null;
        string _identity = null;
        NovaSmoothing _smoothing = null;

        protected List<NovaColumn> _columns = new List<NovaColumn>();
        protected List<NovaWhere> _where = new List<NovaWhere>();
        protected List<NovaJoin> _join = new List<NovaJoin>();
        protected List<NovaOrder> _order = new List<NovaOrder>();


        bool _queryTable = false; //only used when the passin in table(name) is a seperate query

        protected string FULLQUERY = "";

        protected NovaQuery(string table, string action)
        {
            _tableName = table;
            _action = action;
        }

        protected NovaQuery(INovaQuery table, string action)
        {
            _action = action;
            _tableName = table.BuildString();
            _queryTable = true;
        }

        public NovaQuery(INovaDb db, NovaTable table, INovaQuery tableQuery, string action, string identity = null)
        {
            _db = db;
            _table = table;
            _tableName = tableQuery.BuildString();
            _queryTable = true;
            _action = action;
            _identity = identity;
        }

        public NovaQuery(INovaDb db, NovaTable table, string tableName, string action, string identity = null)
        {
            _db = db;
            _table = table;
            _tableName = tableName;
            _action = action;
            _identity = identity;
        }

        public INovaQuery Columns(params string[] columns)
        {
            foreach (string col in columns)
            {
                Column(col);
            }
            return this;
        }

        public INovaQuery Column(string column)
        {
            _columns.Add(new NovaColumn { Name = column });
            return this;
        }

        public INovaQuery Column(string column, object data)
        {
            _columns.Add(new NovaColumn { Name = column, Data = data });
            return this;
        }

        public INovaQuery Column(string column, object data, bool nullable)
        {
            _columns.Add(new NovaColumn { Name = column, Data = data, Nullable = nullable });
            return this;
        }

        public INovaQuery Distinct(string column)
        {
            _distinct = column;
            return this;
        }

        public INovaQuery Where(NovaWhere where)
        {
            _where.Add(where);
            return this;
        }

        public INovaQuery Where(string where)
        {
            Where(new NovaWhere(where));
            return this;
        }

        public INovaQuery Where(string column, object value)
        {
            Where(column, "=", value);
            return this;
        }

        public INovaQuery Where(string column, string eval, object value)
        {
            Where(new NovaWhere(column, eval, value));
            return this;
        }

        public INovaQuery Limit(int value)
        {
            _limit = value;
            return this;
        }

        public INovaQuery Order(string column, string direction = "ASC")
        {
            _order.Add(new NovaOrder { Column = column, Direction = direction });
            return this;
        }

        public INovaQuery ClearOrder()
        {
            _order.Clear();
            return this;
        }

        public INovaQuery Join(string table, string main, string sub = null)
        {
            if (sub == null)
            {
                sub = main;
            }
            _join.Add(new NovaJoin
            {
                Table = table,
                ColMain = main,
                ColSub = sub
            });
            //"join [PointList] on ([" + tableidcol + "] = [" + pointlistcol + "]) "
            return this;
        }

        public INovaQuery Smoothing(string column, int chunkSize, int sampleRate = 0, string name = null)
        {
            Smoothing(new NovaSmoothing(column, chunkSize, sampleRate, name));
            return this;
        }

        public INovaQuery Smoothing(NovaSmoothing smoothing)
        {
            _smoothing = smoothing;
            return this;
        }

        public INovaQuery GroupBy(string groupBy)
        {
            _groupBy = groupBy;
            return this;
        }

        public string BuildString(string forceAction = null)
        {
            if (forceAction == null)
            {
                forceAction = _action;
            }
            FULLQUERY = "";
            bool first = true;
            int count = -1;
            if (_smoothing != null)
            {
                //count = Count();
            }
            switch (forceAction)
            {
                case "SELECT":

                    Append("SELECT");

                    if (_limit > 0)
                    {
                        Append("TOP");
                        Append(_limit.ToString());
                    }

                    if (_distinct == null)
                    {
                        if (_columns.Count > 0 || _smoothing != null)
                        {
                            first = true;
                            foreach (NovaColumn col in _columns)
                            {
                                if (!first)
                                {
                                    Append(",");
                                    NewLine();
                                }
                                first = false;
                                string colStr = "";
                                string func = _smoothing != null ? "avg" :  (string)col.Data;

                                if (func == null)
                                {
                                    colStr = col.Name;
                                }
                                else
                                {
                                    colStr = func + "(" + col.Name + ") as '"+ col.Name + "'";
                                }
                                if (!_queryTable)
                                {
                                    //colStr = _table + "." + colStr; // this causes issues with joins
                                }
                                Append(colStr, 1);
                            }
                            if (_smoothing != null && _smoothing.Name != null)
                            {
                                if (!first)
                                {
                                    Append(",");
                                    NewLine();
                                }

                                Append(_smoothing.Calculation + " * " + _smoothing.ChunkSize + " as '"+_smoothing.Name+"'", 1);                                
                            }
                        }
                        else
                        {
                            Append("*");
                        }
                    }
                    else
                    {
                        Append("DISTINCT");
                        Append(_distinct);
                    }

                    NewLine();
                    Append("FROM");
                    if (_queryTable)
                    {
                        Append("(");
                        NewLine();
                        Append(_tableName);
                        NewLine();
                        Append(")X");
                        
                    }
                    else
                    {
                        Append(_tableName,1);
                    }

                    NewLine();

                    if (_join.Count > 0)
                    {
                        foreach (NovaJoin join in _join)
                        {
                            Append("JOIN");
                            Append(join.Table);
                            Append("ON");
                            if (_queryTable)
                            {
                                // have to build this without table name, so no choice
                                Append("(" + join.ColMain + " = " + join.Table + "." + join.ColSub + ")");
                            }
                            else
                            {
                                Append("(" + _tableName + "." + join.ColMain + " = " + join.Table + "." + join.ColSub + ")");
                            }
                            
                        }
                    }

                    if (_where.Count > 0)
                    {
                        Append("WHERE");
                        Append(whereString());
                    }
                    

                    if (_smoothing != null)
                    {
                        _groupBy = _smoothing.Calculation;
                        _order.Clear();
                        _order.Add(new NovaOrder { Column = _smoothing.Calculation });
                    }

                    if (_groupBy != null)
                    {
                        Append("Group By");
                        Append(_groupBy);
                    }

                    if (_order.Count > 0)
                    {
                        NewLine();
                        Append("ORDER BY");
                        first = true;
                        foreach (NovaOrder order in _order)
                        {
                            if (!first)
                            {
                                Append(",");
                            }
                            first = false;
                            Append(order.Column);
                            if (!string.IsNullOrEmpty(order.Direction))
                            {
                                Append(order.Direction);
                            }
                        }
                    }


                    break;
                case "CREATE":
                    Append("CREATE");
                    Append("TABLE");
                    Append(_tableName);
                    Append("(");

                    first = true;
                    foreach (NovaColumn col in _columns)
                    {
                        if (!first)
                        {
                            Append(",");                            
                        }
                        first = false;
                        NewLine();
                        Append(col.Name + " " + col.Data + (col.Nullable ? "" : " NOT NULL"),1);
                    }
                    Append(")");

                    break;

                case "INSERT":
                    Append("INSERT");
                    Append("INTO");
                    Append(_tableName);

                    

                    List<string> cols = new List<string>();
                    List<object> values = new List<object>();
                    foreach (NovaColumn col in _columns)
                    {
                        cols.Add(col.Name);
                        values.Add(col.Data);
                    }
                    NewLine();
                    Append("(");
                    Append(String.Join(", ", cols));
                    Append(")");

                    if (_identity != null)
                    {
                        NewLine();
                        Append("OUTPUT Inserted." + _identity);
                    }

                    NewLine();
                    Append("VALUES");

                    NewLine();

                    Append("(");
                    for (int i = 0; i < values.Count; i++)
                    {
                        if (i != 0)
                        {
                            Append(",");
                        }
                        Append(StringHelpers.FormatSqlVal(values[i])); // used for loop so proper data types can be added down the track
                    }
                    Append(")");

                    break;

                case "DROP":
                    Append("DROP");
                    Append("TABLE");
                    Append(_tableName);
                    break;

                case "TRUNCATE":
                    Append("TRUNCATE");
                    Append("TABLE");
                    Append(_tableName);
                    break;

                case "DELETE":
                    Append("DELETE");
                    Append("FROM");
                    Append(_tableName);

                    if (_where.Count > 0)
                    {
                        Append("WHERE");
                        Append(whereString());
                    }

                    break;

                case "UPDATE":
                    Append("UPDATE");
                    Append(_tableName);
                    Append("SET");


                    first = true;
                    foreach (NovaColumn col in _columns)
                    {
                        if (!first)
                        {
                            Append(",");
                        }
                        first = false;
                        NewLine();
                        Append(col.Name,1);
                        Append("=",1);
                        Append(StringHelpers.FormatSqlVal(col.Data),1);
                    }

                    if (_identity != null)
                    {
                        NewLine();
                        Append("OUTPUT Inserted." + _identity);
                    }

                    if (_where.Count > 0)
                    {
                        Append("WHERE");
                        Append(whereString());
                    }

                    break;
            }

            return FULLQUERY;
        }
        private string whereString(){
            List<string> whereStrings = new List<string>();
            foreach (NovaWhere where in _where)
            {
                if (_queryTable)
                {
                    whereStrings.Add(where.String()); // have to build this without table name, so no choice
                }
                else
                {
                    whereStrings.Add(where.String(_tableName));
                }                            
            }
            if (_smoothing != null && _smoothing.Sample)
            {
                whereStrings.Add(_smoothing.Calculation + " % " + _smoothing.Mod + " = 0");
            }
            return String.Join(" AND ", whereStrings);
        }

        private void NewLine()
        {
            FULLQUERY += Environment.NewLine;
        }

        private void Append(string val, int indentation = 0, string seperator = " ")
        {
            for (int i = 0; i < indentation; i++)
            {
                FULLQUERY += "    ";
            }
            FULLQUERY += val + seperator;
        }

        public virtual int Execute(bool handleExceptions = true)
        {
            return _db.Execute(BuildString(), handleExceptions);
        }

        public virtual int Count()
        {
            string command = "SELECT COUNT(";
            if (_smoothing != null)
            {
                command += "DISTINCT " + _smoothing.Calculation;
            }
            else
            {
                command += "*";
            }
            command += ") FROM " + _tableName;

            if (_where.Count > 0)
            {
                command += " WHERE " + whereString();
            }

            return (int)_db.Scalar(command);
        }

        public virtual int Max(string column)
        {
            string command = "SELECT MAX(" + column + ") FROM " + _tableName;
            if (_where.Count > 0)
            {
                command += " WHERE " + whereString();
            }          

            return (int)_db.Scalar(command);
        }

        public virtual int Min(string column)
        {
            string command = "SELECT MIN(" + column + ") FROM " + _tableName;

            if (_where.Count > 0)
            {
                command += " WHERE " + whereString();
            }

            return (int)_db.Scalar(command);
        }


        public virtual object Scalar()
        {
            return _db.Scalar(BuildString());
        }
        public virtual T Scalar<T>()
        {
            object val = this.Scalar();

            return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
        }

        private SqlDataReader Get()
        {
            return _db.Query(BuildString());
        }

        public virtual INovaResult Result()
        {
            using (var reader = Get())
            {
                var cols = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    cols.Add(reader.GetName(i));
                }
                if (_smoothing != null)
                {
                    cols.Add(_smoothing.Name);
                }

                INovaResult table = new NovaResult(cols.ToArray(), _smoothing);

                while (reader.Read())
                {
                    table.AddRow(new NovaEntity(_table, reader));
                }
                return table;
            }            
        }

        public virtual bool Any()
        {
            using (var reader = Get())
            {
                bool any = reader.HasRows;
                reader.Close();
                return any;
            }
        }

        public virtual NovaEntity First()
        {
            NovaEntity item = null;

            using (var reader = Get())
            {

                if (reader.Read())
                {
                    item = new NovaEntity(_table, reader);
                }

                reader.Close();

                return item;
            }


        }
    }
}
