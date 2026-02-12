using Fcg.User.Common;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

namespace Fcg.User.Application.Requests
{
    public class BuyGamesRequest : IRequest<Response>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public IEnumerable<Guid> GamesId { get; set; } = null!;
    }

    public class BuyGamesRequestValidator : AbstractValidator<BuyGamesRequest>
    {
        public BuyGamesRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("O ID do usuário é obrigatório.");

            RuleFor(x => x.GamesId)
                .NotEmpty().WithMessage("A lista de IDs dos jogos é obrigatória.");
        }
    }
}
