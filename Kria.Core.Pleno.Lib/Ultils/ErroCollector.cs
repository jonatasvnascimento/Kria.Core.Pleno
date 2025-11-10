using Kria.Core.Pleno.Lib.Interfaces.DAO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Ultils
{
    public class ErroCollector(IConfigurationDAO configurationDao) : IErroCollectorDAO
    {
        private readonly ConcurrentBag<string> _erros = new();
        private IConfigurationDAO _configurationDao = configurationDao;

        public int Count => _erros.Count;
        private string pathCaminho {  get; set; } = string.Empty;
        public string PastaLog => "Logs";

        public void Add(string mensagem) => _erros.Add(mensagem);
        public void Add(IEnumerable<string> mensagens)
        {
            foreach (var m in mensagens) _erros.Add(m);
        }

        public async Task SalvarEmDiscoAsync(string caminho)
        {
            if (!_erros.Any()) return;
            var linhas = _erros.Select(e => $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {e}");
            pathCaminho = $"{PastaLog}/{caminho}";
            await File.AppendAllLinesAsync(pathCaminho, linhas);
        }

        public void CriarDiretorioLog()
        {
            if (!Directory.Exists(PastaLog))
            {
                Directory.CreateDirectory(PastaLog);
            }
        }

        public string PathLog() => pathCaminho;
        public int CountErros() => _erros.Count;
    }
}
