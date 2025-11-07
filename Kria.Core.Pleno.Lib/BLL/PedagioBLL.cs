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
    public class PedagioBLL(IPedagioDAO pedagioDAO, IConfigurationDao configurationDao) : IPedagioBLL
    {
        private readonly IPedagioDAO _pedagioDAO = pedagioDAO;

        public void ProcessarLotePedagio()
        {
            var transacoes = _pedagioDAO.ObterTodos().ToList();
            var numeroPacotes = int.TryParse(configurationDao.PegarChave("Configuracoes:Pacotes"), out var pacotes) ? pacotes : 1000;

            foreach (var pacote in transacoes.Chunk(numeroPacotes))
            {

            }

        }
    }
}
