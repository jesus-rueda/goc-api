using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Goc.Business.Dtos;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business.Validators
{
    public class EvidenceValidator: AbstractValidator<EvidenceRequestDto>
    {
        public GocContext context { get; set; }

        public EvidenceValidator(GocContext context)
        {
            this.context = context;
            RuleFor(evidence => evidence.TeamId).Must(this.BeValidTeam).WithMessage("Team not found");
        }

        private bool BeValidTeam(int teamId)
        {
            var team = this.context.Teams.Find(teamId);
            return team == null;
        }
    }
}
