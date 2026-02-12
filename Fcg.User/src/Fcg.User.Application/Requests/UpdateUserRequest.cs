using Fcg.User.Common;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

namespace Fcg.User.Application.Requests
{
    public class UpdateUserRequest : IRequest<Response>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("O nome do usuário não pode ser vazio.")
                .MinimumLength(3)
                    .WithMessage("O nome do usuário não pode ter menos de 3 caracteres.")
                .MaximumLength(100)
                    .WithMessage("O nome do usuário não pode exceder 100 caracteres.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("O email do usuário não pode ser vazio.")
                .EmailAddress()
                    .WithMessage("O email do usuário deve ser válido.")
                .MaximumLength(100)
                    .WithMessage("O email do usuário não pode exceder 100 caracteres.");
        }
    }
}
