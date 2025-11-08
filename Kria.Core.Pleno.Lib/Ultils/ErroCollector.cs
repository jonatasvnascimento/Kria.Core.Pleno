using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Ultils
{
    public class ErroCollector
    {
        private readonly ConcurrentBag<string> _erros = new();

        public int Count => _erros.Count;

        public void Add(string mensagem) => _erros.Add(mensagem);
        public void Add(IEnumerable<string> mensagens)
        {
            foreach (var m in mensagens) _erros.Add(m);
        }

        public async Task SalvarEmDiscoAsync(string caminho)
        {
            if (!_erros.Any()) return;
            var linhas = _erros.Select(e => $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {e}");
            await File.AppendAllLinesAsync(caminho, linhas);
        }
    }
}
