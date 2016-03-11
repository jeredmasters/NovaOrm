using System;
using System.Collections.Generic;
namespace NovaOrm
{
    public interface INovaDb
    {
        void Connect();
        INovaQuery Create(string tableName);
        INovaQuery Delete(string table);
        void Disconnect();
        INovaQuery Drop(string table);
        int Execute(string query, bool handleException = true);
        INovaQuery Insert(string tableName);
        System.Data.SqlClient.SqlDataReader Query(string query);
        object Scalar(string query);
        INovaQuery Select(INovaQuery table);
        INovaQuery Select(string table);
        INovaQuery Update(string table);
        INovaTable Table(string table, string identity = null);
        bool TableExists(string tableName);
        NovaOrm.NovaDb.ExceptionThrown ExceptionHandler { get; set; }
    }
}
