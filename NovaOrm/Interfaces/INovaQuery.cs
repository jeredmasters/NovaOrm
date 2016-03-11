using System;
namespace NovaOrm
{
    public interface INovaQuery
    {
        bool Any();
        string BuildString(string forceAction = null);
        INovaQuery Column(string column);
        INovaQuery Column(string column, object data);
        INovaQuery Column(string column, object data, bool nullable);
        INovaQuery Columns(params string[] columns);
        int Count();
        INovaQuery Distinct(string column);
        int Execute(bool handleException = true);
        NovaEntity First();
        INovaResult Result();
        INovaQuery Join(string table, string main, string sub = null);
        INovaQuery Limit(int value);
        INovaQuery Order(string column, string direction = "ASC");
        INovaQuery ClearOrder();
        INovaQuery GroupBy(string groupBy);
        INovaQuery Where(NovaWhere where);
        INovaQuery Where(string column, object value);
        INovaQuery Where(string column, string eval, object value);
        INovaQuery Where(string where);
        INovaQuery Smoothing(NovaSmoothing smoothing);
        INovaQuery Smoothing(string column, int chunkSize, int sampleRate = 0, string name = null);
        int Max(string column);
        int Min(string column);
        object Scalar();
        T Scalar<T>();
    }
}
