using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Entidades
{
    public class TabTransacoes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = string.Empty;
        public int IdTransacao { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DtCriacao { get; set; }
        public string CodigoPracaPedagio { get; set; } = string.Empty;
        public int CodigoCabine { get; set; }
        public string Instante { get; set; } = string.Empty;
        public int Sentido { get; set; }
        public int QuantidadeEixosVeiculo { get; set; }
        public int Rodagem { get; set; }
        public int Isento { get; set; }
        public int MotivoIsencao { get; set; }
        public int Evasao { get; set; }
        public int EixoSuspenso { get; set; }
        public int QuantidadeEixosSuspensos { get; set; }
        public int TipoCobranca { get; set; }
        public string Placa { get; set; } = string.Empty;
        public int LiberacaoCancela { get; set; }
        public decimal ValorDevido { get; set; }
        public decimal ValorArrecadado { get; set; }
        public string CnpjAmap { get; set; } = string.Empty;
        public float? MultiplicadorTarifa { get; set; }
        public int VeiculoCarregado { get; set; }
        public string IdTag { get; set; } = string.Empty;
    }
}
