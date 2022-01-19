namespace DAL.Abstractions.Interfaces
{
    public interface IWorker
    {
        public void SaveToFile<T>(string filePath, T obj);
        public T LoadFromFile<T>(string filePath);
    }
}