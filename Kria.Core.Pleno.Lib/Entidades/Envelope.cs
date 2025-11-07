using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Entidades
{
    public class Envelope
    {
        public string Candidato { get; set; } = string.Empty;
        public string DataReferencia { get; set; } = string.Empty;
        public int NumeroArquivo { get; set; }
        public string CnpjConcessionaria { get; set; } = string.Empty;
        public int Motivo { get; set; }
        public List<RegistroPedagio> Registros { get; set; } = new List<RegistroPedagio>();
    }
}
