using AuthModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthContextEfCore.Configs;

public class RefreshTokenConfig:IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x=>new {x.User,x.Token});
        builder.HasIndex(x => x.User);
        builder.Property(x=>x.Token).IsRequired();
        builder.Property(x=>x.Expiration).IsRequired();
    }
}