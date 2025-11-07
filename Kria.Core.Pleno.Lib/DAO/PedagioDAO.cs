using Kria.Core.Pleno.Lib.Context;
using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.DAO
{
    public class PedagioDAO(MongoDbContext context) : IPedagioDAO
    {
        private readonly IMongoCollection<Pedagio> _context = context.GetCollection<Pedagio>("Pedagios");

        public List<Pedagio> ObterTodos()
        {
            return _context.AsQueryable().ToList();
        }
    }
}
