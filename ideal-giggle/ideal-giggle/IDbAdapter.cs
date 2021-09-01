namespace ideal_giggle
{
    public interface IDbAdapter
    {
        public string Name { get; }
        public void InsertToTable<T>(T table);
  
    }
}
