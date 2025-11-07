using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Interfaces.DAO
{
    public interface IMongoDbContext
    {
        public IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
