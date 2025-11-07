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
        public int CodigoPracaPedagio { get; set; }      
        public int CodigoCabine { get; set; }            
        public string Instante { get; set; } = string.Empty;             
        public int Sentido { get; set; }                 
        public int TipoVeiculo { get; set; }             
        public int Isento { get; set; }                  
        public int Evasao { get; set; }                  
        public int TipoCobrancaEfetuada { get; set; }    
        public decimal ValorDevido { get; set; }         
        public decimal ValorArrecadado { get; set; }     
        public decimal MultiplicadorTarifa { get; set; } 
    }
}
