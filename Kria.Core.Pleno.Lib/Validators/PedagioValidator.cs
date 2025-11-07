using FluentValidation;
using Kria.Core.Pleno.Lib.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Validators
{
    public class PedagioValidator : AbstractValidator<Pedagio>
    {
        public PedagioValidator()
        {
            //RuleFor(p => p.CodigoPracaPedagio)
            //    .NotEmpty().WithMessage("O campo CodigoPracaPedagio é obrigatório.")
            //    .MaximumLength(10).WithMessage("O campo CodigoPracaPedagio deve ter no máximo 10 caracteres.");
            //RuleFor(p => p.CodigoCabine)
            //    .GreaterThan(0).WithMessage("O campo CodigoCabine deve ser maior que zero.");
            //RuleFor(p => p.Instante)
            //    .NotEmpty().WithMessage("O campo Instante é obrigatório.");
            //RuleFor(p => p.Placa)
            //    .NotEmpty().WithMessage("O campo Placa é obrigatório.")
            //    .MaximumLength(15).WithMessage("O campo Placa deve ter no máximo 15 caracteres.");
            //RuleFor(p => p.ValorDevido)
            //    .GreaterThanOrEqualTo(0).WithMessage("O campo ValorDevido deve ser maior ou igual a zero.");
            //RuleFor(p => p.ValorArrecadado)
            //    .GreaterThanOrEqualTo(0).WithMessage("O campo ValorArrecadado deve ser maior ou igual a zero.");
        }
    }
}
