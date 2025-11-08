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
            return _context.AsQueryable();
        }

        public IEnumerable<TabTransacoes> ObterLote(DateTime? ultimaData, int tamanho)
        {
            var filtro = Builders<TabTransacoes>.Filter.Empty;

            if (ultimaData.HasValue)
                filtro = Builders<TabTransacoes>.Filter.Gt(t => t.DtCriacao, ultimaData.Value);

            return _context
                .Find(filtro)
                .SortBy(t => t.DtCriacao)
                .Limit(tamanho)
                .ToList();
        }

    }
}
