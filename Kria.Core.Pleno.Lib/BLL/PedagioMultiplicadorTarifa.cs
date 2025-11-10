using Kria.Core.Pleno.Lib.Entidades.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.BLL
{
    public static class PedagioMultiplicadorTarifa
    {
        private static readonly Random _random = new();
        public static double Calcular(int tipoVeiculo, int isento) => (tipoVeiculo, isento) switch
        {
            ((int)ETipoVeiculo.Moto, (int)EIsento.Sim) => 0.0,
            ((int)ETipoVeiculo.Moto, (int)EIsento.Nao) => 0.5,
            ((int)ETipoVeiculo.Passeio, _) => new double[] { 1.0, 1.5, 2.0 }[_random.Next(3)],
            ((int)ETipoVeiculo.Comercial, _) => _random.Next(2, 21),
            _ => double.MinValue
        };
    }

}
