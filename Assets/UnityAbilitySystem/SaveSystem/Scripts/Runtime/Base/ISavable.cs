namespace SaveSystem
{
    public interface ISavable
    {
        object data { get; }
        void Load(object _data);
    }
}