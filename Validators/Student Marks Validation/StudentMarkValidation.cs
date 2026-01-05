using FluentValidation;
using Lpgin2.DTOs.Request.StudentMarkDTO;
using static Lpgin2.Models.Entities.Enums;

namespace Lpgin2.Validators.Student_Marks_Validation
{
    public class StudentMarkValidation:AbstractValidator<AddStudentMarkDTO>
    {
        public StudentMarkValidation()
        {
           
            RuleFor(s => s.EnrollmentId)
                .GreaterThan(0)
                .WithMessage("EnrollmentId must be a valid value");

            RuleFor(s => s.ExamTypeId)
                .IsInEnum()
                .WithMessage("Invalid exam type");

            RuleFor(s => s.Mark)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Mark cannot be negative")
                .Must((dto, mark) => IsValidMark(dto.ExamTypeId, mark))
                .WithMessage("Mark exceeds the allowed limit for this exam type");
        }

        private bool IsValidMark(enExamTypes examType, decimal mark)
        {
            return examType switch
            {
                enExamTypes.First => mark <= 20,
                enExamTypes.Secound => mark <= 20,
                enExamTypes.Final => mark <= 60,
                _ => false
            };
        }
    }
    
}
