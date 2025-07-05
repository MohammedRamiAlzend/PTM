using FluentValidation;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.Validation
{
    public class CreateTaskValidation : AbstractValidator<CreateTaskDto>
    {
        
        public CreateTaskValidation()
        {
            RuleFor(x => x.DueDate)
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("DueDate should be in the present or future.");

            RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(3)
                .WithMessage("title length should be at least 3 chars");
        }
    }
}
