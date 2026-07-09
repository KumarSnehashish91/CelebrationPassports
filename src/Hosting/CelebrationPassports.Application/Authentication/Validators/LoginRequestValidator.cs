using CelebrationPassports.Application.Authentication.DTOs.RequestDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Authentication.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
