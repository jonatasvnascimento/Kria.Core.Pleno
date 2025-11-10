using Kria.Core.Pleno.Lib.Entidades.DTO.DRegistroPedagioDTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Entidades.DTO.DPedagioDTO;

public static class PedagioDTOMapper
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static RegistroPedagioDTO Map(RegistroPedagio registroPedagio)
    {
        return new RegistroPedagioDTO
        {
            GUID = registroPedagio.GUID,
            CodigoPracaPedagio = registroPedagio.CodigoPracaPedagio.ToString(Inv),
            CodigoCabine = registroPedagio.CodigoCabine.ToString(Inv),
            Instante = registroPedagio.Instante,
            Sentido = registroPedagio.Sentido.ToString(Inv),
            TipoVeiculo = registroPedagio.TipoVeiculo.ToString(Inv),
            Isento = registroPedagio.Isento.ToString(Inv),
            Evasao = registroPedagio.Evasao.ToString(Inv),
            TipoCobrancaEfetuada = registroPedagio.TipoCobrancaEfetuada.ToString(Inv),
            ValorDevido = registroPedagio.ValorDevido.ToString(Inv),
            ValorArrecadado = registroPedagio.ValorArrecadado.ToString(Inv),
            MultiplicadorTarifa = registroPedagio.MultiplicadorTarifa.ToString(Inv)
        };
    }

    public static PedagioDTO Map(Pedagio pedagio)
    {
        return new PedagioDTO
        {
            Candidato = pedagio.Candidato,
            DataReferencia = pedagio.DataReferencia,
            NumeroArquivo = pedagio.NumeroArquivo,
            Registros = pedagio.Registros.Select(x => Map(x)).ToList()
        };
    }

}
