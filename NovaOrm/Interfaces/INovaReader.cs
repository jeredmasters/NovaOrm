using System;
using System.Collections;
using System.Collections.Generic;

namespace NovaOrm
{
    public interface INovaResult
    {
        void AddRow(NovaEntity row);
        bool Read();
        object this[int row, string column] { get; }
        object this[string column] { get; }
        int Max(string column);
        int Min(string column);
        IEnumerable<string> GetColumns();
        string ToCsv();
        string ToJson();
        int Count();
        NovaSmoothing Smoothing();
        IEnumerator<NovaEntity> GetEnumerator();
    }
}
