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
                .NotNull().NotEmpty().WithMessage("O campo GUID é obrigatório.");
            RuleFor(p => p.CodigoPracaPedagio)
                .NotNull().NotEmpty().WithMessage("O campo CodigoPracaPedagio é obrigatório.");
            RuleFor(p => p.CodigoCabine)
                .NotNull().NotEmpty().WithMessage("O campo CodigoCabine é obrigatório.");
            RuleFor(p => p.Instante)
                .NotNull().NotEmpty().WithMessage("O campo Instante é obrigatório.");
            RuleFor(p => p.Sentido)
                .NotNull().NotEmpty().WithMessage("O campo Sentido é obrigatório.");
            RuleFor(p => p.TipoVeiculo)
                .NotNull().NotEmpty().WithMessage("O campo TipoVeiculo é obrigatório.");
            RuleFor(p => p.Isento)
                .NotNull().NotEmpty().WithMessage("O campo Isento é obrigatório.");
            RuleFor(p => p.Evasao)
                .NotNull().NotEmpty().WithMessage("O campo Evasao é obrigatório.");
            RuleFor(p => p.TipoCobrancaEfetuada)
                .NotNull().NotEmpty().WithMessage("O campo TipoCobrancaEfetuada é obrigatório.");
            RuleFor(p => p.ValorDevido)
                .NotNull().NotEmpty().WithMessage("O campo ValorDevido é obrigatório.");
            RuleFor(p => p.ValorArrecadado)
                .NotNull().NotEmpty().WithMessage("O campo ValorArrecadado é obrigatório.");
        }
    }
}
