using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Entidades
{
    public class RegistroPedagio
    {
        public string GUID { get; set; } = string.Empty;
        public string CodigoPracaPedagio { get; set; } = string.Empty;
        public string CodigoCabine { get; set; } = string.Empty;
        public string Instante { get; set; } = string.Empty;             
        public string Sentido { get; set; } = string.Empty;
        public string TipoVeiculo { get; set; } = string.Empty;
        public string Isento { get; set; } = string.Empty;
        public string Evasao { get; set; } = string.Empty;
        public string TipoCobrancaEfetuada { get; set; } = string.Empty;
        public string ValorDevido { get; set; } = string.Empty;
        public string ValorArrecadado { get; set; } = string.Empty;
        public string MultiplicadorTarifa { get; set; } = string.Empty;
    }
}
