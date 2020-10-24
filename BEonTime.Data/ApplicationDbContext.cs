using BEonTime.Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BEonTime.Data
{
    public interface IAppDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }

    public class AppDbContext : IAppDbContext
    {
        private MongoClient MongoClient { get; set; }
        private IMongoDatabase Database { get; set; }

        public AppDbContext(IOptions<MongoDBOptions> settings)
        {
            MongoClient = new MongoClient(settings.Value.ConnectionString);
            Database = MongoClient?.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return Database.GetCollection<T>(name);
        }
    }
}
