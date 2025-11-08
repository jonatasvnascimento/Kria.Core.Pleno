using Kria.Core.Pleno.Lib.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Interfaces.DAO
{
    public interface IPedagioDAO
    {
        public IQueryable<TabTransacoes> ObterTodos();
        public IEnumerable<TabTransacoes> ObterLote(DateTime? ultimaData, int tamanho);
    }
}
