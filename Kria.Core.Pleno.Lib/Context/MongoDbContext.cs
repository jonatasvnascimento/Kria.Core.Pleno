using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public MongoDbContext(IConfigurationDao configuration)
        {
            var connectionString = configuration.PegarChave("DefaultConnection");
            var databaseName = configuration.PegarChave("DatabaseName");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
            => _database.GetCollection<T>(collectionName);
    }
}
