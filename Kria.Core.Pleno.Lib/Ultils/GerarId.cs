using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Ultils
{
    public static class GerarId
    {
        private static Dictionary<string, int> contadorPorData = new();

        public static int ObterProximoNumeroArquivo(string data)
        {
            if (!contadorPorData.ContainsKey(data))
                contadorPorData[data] = 1;
            else
                contadorPorData[data]++;

            return contadorPorData[data];
        }
    }
}
