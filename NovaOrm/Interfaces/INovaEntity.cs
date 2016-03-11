using System.Collections.Generic;

namespace NovaOrm
{
    public interface INovaEntity
    {
        object this[string index] { get; set; }

        object Id { get; }

        void Delete();
        IEnumerator<NovaField> GetEnumerator();
        object Save();
        string ToCsv();
        string ToJson();
    }
}