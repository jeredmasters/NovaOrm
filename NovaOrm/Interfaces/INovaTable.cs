namespace NovaOrm
{
    public interface INovaTable
    {
        string Identity { get; }

        INovaQuery Create();
        INovaQuery Delete();
        INovaQuery Drop();
        bool Exists();
        NovaEntity Find(object id);
        INovaQuery Insert();
        NovaEntity New();
        INovaQuery Select();
        INovaQuery Truncate();
        INovaQuery Update();
        INovaQuery Update(string id);
    }
}