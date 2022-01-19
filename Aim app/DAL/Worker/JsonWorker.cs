using System.IO;
using System.Text;
using Newtonsoft.Json;
using DAL.Abstractions.Interfaces;

namespace DAL.Worker
{
    public class JsonWorker : IWorker
    {
        public void SaveToFile<T>(string filePath, T obj)
        {
            var writeStream = File.OpenWrite(filePath);
            var json = JsonConvert.SerializeObject(obj);
            writeStream.Write(Encoding.UTF8.GetBytes(json));
        }


        public T LoadFromFile<T>(string filePath)
        {
            var writeStream = File.OpenRead(filePath);
            var json = JsonConvert.DeserializeObject<T>(filePath);
            return json;
        }
    }
}