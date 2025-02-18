using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Views;

namespace EfCoreContext.Configurations;

public class RandomCityViewConfiguration: IEntityTypeConfiguration<RandomCityView>
{
    public void Configure(EntityTypeBuilder<RandomCityView> builder)
    {
        builder.ToView("RandomCityView").HasKey(x=>x.CityId);
    }
}