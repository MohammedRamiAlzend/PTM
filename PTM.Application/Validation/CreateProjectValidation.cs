using FluentValidation;

namespace PTM.Application.Validation
{
    public class CreateProjectValidation : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectValidation()
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage("Project name is required");
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("Project name must have at least 3 characters");
            RuleFor(x => x.Description).Must(x=> {
                if (string.IsNullOrWhiteSpace(x))
                    return true;
                if (x.Length < 3)
                    return false;
                return true;
            }).WithMessage("Description must have at least 3 characters");
        }
    }
}
