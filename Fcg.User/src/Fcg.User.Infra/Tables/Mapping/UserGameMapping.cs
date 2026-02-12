using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.User.Infra.Tables.Mapping
{
    public class UserGameMapping : IEntityTypeConfiguration<UserGame>
    {
        public void Configure(EntityTypeBuilder<UserGame> builder)
        {
            builder.ToTable("UserGames");

            builder.HasKey(ug => ug.Id);

            builder.Property(ug => ug.DateToPurchase)
                   .IsRequired();

            builder.Property(ug => ug.GameId)
                   .IsRequired();

            builder.HasOne(ug => ug.User)
                .WithMany()
                .HasForeignKey(ug => ug.UserId)
                .IsRequired();
        }
    }
}
