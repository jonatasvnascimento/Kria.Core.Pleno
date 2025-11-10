using FluentValidation;
using Kria.Core.Pleno.Lib.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Validators
{
    public class RegistroPedagioValidator : AbstractValidator<RegistroPedagio>
    {
        public RegistroPedagioValidator()
        {
            RuleFor(p => p.GUID)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.CodigoPracaPedagio)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.CodigoCabine)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.Instante)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.Sentido)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.TipoVeiculo)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.Isento)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.Evasao)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.TipoCobrancaEfetuada)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.ValorDevido)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.ValorArrecadado)
                .NotNull().NotEmpty().WithMessage("O campo é obrigatório.");
            RuleFor(p => p.MultiplicadorTarifa)
                .NotNull()
                .NotEmpty()
                .Must(x => x != decimal.MinValue)
                .WithMessage("O campo é obrigatório.");

        }
    }
}
