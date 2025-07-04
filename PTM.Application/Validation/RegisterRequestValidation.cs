using FluentValidation;
using PTM.Application.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.Validation
{
    public class RegisterRequestValidation : AbstractValidator<RegisterRequestDto>
    {
        private readonly IEntityCommiter commiter;
        public RegisterRequestValidation(IEntityCommiter commiter)
        {
            this.commiter = commiter;

            RuleFor(x => x.Username).NotEmpty().WithMessage("Username are required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password are required.");
            RuleFor(x => x.RoleName).NotEmpty().WithMessage("RoleName are required.");
            
            RuleFor(x => x.Username).MustAsync(async (u,c) =>
            {
                return !await commiter.Users.AnyAsync(x => x.Username.ToLower() == u.ToLower());
            }).WithMessage("Username already Taken");

            RuleFor(x => x.RoleName).MustAsync(async (u, c) =>
            {
                return await commiter.Users.AnyAsync(x => x.Role.Name.ToLower() == u.ToLower());
            }).WithMessage("role not found");


        }
    }
}
