using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public static class StringHelpers
    {
        public static T FixUp<T>(object value, object defaultValue = null)
        {

            if (defaultValue == null)
            {
                defaultValue = default(T);
            }
            if (value == null)
            {
                value = defaultValue;
            }
            else
            {
                if (typeof(T) == typeof(int))
                {
                    if (value.GetType() == typeof(string))
                    {
                        try
                        {
                            value = Convert.ToInt32(value as string);
                        }
                        catch
                        {
                            value = defaultValue;
                        }
                    }
                }

                if (typeof(T) == typeof(string))
                {
                    if (value.GetType() != typeof(string))
                    {
                        value = defaultValue;
                    }
                }

                if (typeof(T) == typeof(bool))
                {
                    value = (value as string) == "True";
                }
            }

            return (T)value;
        }

        public static Int32 Unix(DateTime? timein = null)
        {
            if (timein == null)
            {
                timein = DateTime.UtcNow;
            }
            TimeSpan span = (DateTime)timein - DateTime.Parse("00:00:00 01/01/1970");
            return Convert.ToInt32(span.TotalSeconds);
        }
        public static string FileTime(DateTime? timein = null)
        {
            if (timein == null)
            {
                timein = DateTime.Now;
            }
            return ((DateTime)timein).ToString("yyyyMMdd_HHmmss");
        }
        public static string SeperateByComma(int[] input)
        {
            List<string> list = new List<string>();
            foreach (int line in input)
            {
                list.Add(line.ToString());
            }
            return SeperateByComma(list.ToArray());
        }
        public static string FormatSqlVal(object val, bool quoteString = true)
        {
            if (val == null)
            {
                return "NULL";
            }
            Type t = val.GetType();
            if (val is Enum)
            {
                return FormatSqlVal(val.ToString());
            }
            if (t == typeof(DateTime))
            {
                return FormatSqlVal(((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss"), quoteString);
            }
            if (t == typeof(int) || t == typeof(long) || t == typeof(double))
            {
                return val.ToString();
            }
            if (t == typeof(string))
            {
                string retval = val.ToString().Replace("'", "''");
                if (quoteString)
                {
                    retval = "'" + retval + "'";
                }
                return retval;
            }

            return "Unrecognised type: " + t.ToString() + " (" + val.ToString() + ")";
        }
        public static string SeperateByComma(string[] input)
        {
            return SeperateBy(input, ",");
        }
        public static string SeperateBy(string[] input, string delimeter)
        {
            string retval = "";
            if (input.Count() > 0)
            {
                for (int i = 0; i < (input.Count() - 1); i++)
                {
                    string cell = input[i].ToString().Replace("\"", "\\\"");
                    if (cell.Contains(delimeter))
                    {
                        cell = '\"' + cell + '\"';
                    }
                    retval += cell + delimeter;
                }
                int lastint = input.Count() - 1;
                string lastcell = input[lastint].ToString().Replace("\"", "\\\""); ;
                if (lastcell.Contains(delimeter))
                {
                    lastcell = '\"' + lastcell + '\"';
                }
                retval += lastcell;
            }
            return retval;
        }
        public static string SeperateByComma(string input)
        {
            List<string> output = new List<string>();
            string extract;
            int NewLineAt = 0;
            while (input.Contains("\n"))
            {
                int indexofN = input.IndexOf("\n");
                int indexofR = input.IndexOf("\r");
                int indexofRN = input.IndexOf("\r\n");
                int size = 2;
                if (indexofN < indexofR)
                {
                    NewLineAt = indexofN;
                    size = 2;
                }
                if (indexofN > indexofR)
                {
                    NewLineAt = indexofR;
                    size = 2;
                    if (indexofRN == indexofR)
                    {
                        NewLineAt = indexofRN;
                        size = 4;
                    }
                    else
                    {
                        NewLineAt = indexofR;
                        size = 2;
                    }

                }


                extract = input.Substring(0, NewLineAt);
                input = input.Substring(NewLineAt + size);
                output.Add(extract);
            }
            output.Add(input);
            string[] celllist = output.ToArray();

            return SeperateByComma(celllist);

        }
    }
}
