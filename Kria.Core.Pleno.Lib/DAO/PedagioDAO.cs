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
    public class PedagioDAO(IMongoDbContext context) : IPedagioDAO
    {
        private readonly IMongoCollection<TabTransacoes> _context = context.GetCollection<TabTransacoes>("TabTransacoes");

        public IQueryable<TabTransacoes> ObterTodos()
        {
            throw new Exception("Teste");
            return _context.AsQueryable();
        }
    }
}
