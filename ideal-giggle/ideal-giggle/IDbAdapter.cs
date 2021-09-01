namespace ideal_giggle
{
    public interface IDbAdapter
    {
        public string Name { get; }
        public long InsertToTable<T>(T table);
  
    }
}
