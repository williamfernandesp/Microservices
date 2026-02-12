namespace Fcg.User.Domain
{
    public class User
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public decimal Walllet { get; private set; }
        public List<UserGame> Library { get; private set; } = [];

        public User(Guid id, string name)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio ou nulo.", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio ou nulo.", nameof(name));

            Id = id;
            Name = name;
            Library = [];
            Walllet = 0;
        }

        public User(Guid id, string name, decimal walllet, List<UserGame> library)
        {
            Id = id;
            Name = name;
            Walllet = walllet;
            Library = library;
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void AddGameToLibrary(Guid gameId)
        {
            if (gameId == Guid.Empty)
                throw new ArgumentException("ID do game inválido.", nameof(gameId));

            if (Library.Select(x => x.GameId).Contains(gameId))
                return;

            Library.Add(new UserGame(gameId, DateTimeOffset.Now));
        }

        public void AddFunds(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor a ser adicionado deve ser maior que zero.", nameof(amount));

            Walllet += amount;
        }

        public void RemoveFunds(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor a ser removido deve ser maior que zero.", nameof(amount));

            if (amount > Walllet)
                throw new InvalidOperationException("Saldo insuficiente na carteira do usuário.");

            Walllet -= amount;
        }
    }
}
