using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Interfaces.DAO
{
    public interface IErroCollectorDAO
    {
        public Task SalvarEmDiscoAsync(string caminho);
        public void Add(IEnumerable<string> mensagens);
        public void CriarDiretorioLog();
        public string PathLog();
    }
}
