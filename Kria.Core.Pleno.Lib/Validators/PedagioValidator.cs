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
            RuleFor(p => p.Candidato)
                .NotNull().NotEmpty().WithMessage("O campo Candidato é obrigatório.");
            RuleFor(p => p.DataReferencia)
                .NotNull().NotEmpty().WithMessage("O campo DataReferencia é obrigatório.");
            RuleFor(p => p.NumeroArquivo)
                .NotNull().NotEmpty().WithMessage("O campo NumeroArquivo é obrigatório.");
        }
    }
}
