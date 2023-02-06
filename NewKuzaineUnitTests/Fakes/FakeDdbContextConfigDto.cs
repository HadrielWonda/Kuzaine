namespace NewKuzaineUnitTests.Fakes;

using AutoBogus;
using NewKuzaine.Domain;
using NewKuzaine.Domain.DbContextConfigs.Dtos;

public class FakeDbContextConfigDto : AutoFaker<DbContextConfigDto>
{
    public FakeDbContextConfigDto()
    {
        RuleFor(e => e.Provider, 
            u => u.PickRandom<DbProvider>(DbProvider.List).Name);
        RuleFor(e => e.NamingConvention, 
            u => u.PickRandom<NamingConventionEnum>(NamingConventionEnum.List).Name);
    }
}