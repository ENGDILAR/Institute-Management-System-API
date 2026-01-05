using FluentValidation;
using Lpgin2.DTOs.Request;

namespace Lpgin2.Validators.Admin_Validation
{
    public class AdminValidation:AbstractValidator<AdminDTO>
    {
        public AdminValidation()
        {
            RuleFor(a => a.Email)
        .NotEmpty().WithMessage("Email Cannot be Empy")
        .EmailAddress().WithMessage("Invalid Email Address Format");


            RuleFor(a => a.FirstName)
              .NotEmpty().WithMessage("First Name Cannot be Empy")
             .MinimumLength(3).WithMessage("First Name must contain at least 3 character");

            RuleFor(a => a.LastName)
             .NotEmpty().WithMessage("Last Name Cannot be Empy")
             .MinimumLength(3).WithMessage("Last Name must contain at least 3 character");

            RuleFor(a => a.Phone)
             .NotEmpty().WithMessage("Phone Cannot be Empy")
             .Matches(@"^[0-9]+$").WithMessage("Phone must contain only digits");
            RuleFor(a => a.Password)
                .NotEmpty().WithMessage("Password cant be Empty")
                .MinimumLength(8).WithMessage("Password must contain at least 8 characters long")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one UpperCase char")
                .Matches(@"\d").WithMessage("Password must contain at least one Number");



        }
    }
}
