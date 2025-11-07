using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.BLL
{
    public class PedagioBLL(IPedagioDAO pedagioDAO) : IPedagioBLL
    {
        private readonly IPedagioDAO _pedagioDAO = pedagioDAO;

        public IQueryable<TabTransacoes> ObterTodos()
        {
            return _pedagioDAO.ObterTodos();
        }
    }
}
