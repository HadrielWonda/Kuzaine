using System.Collections.Generic;
using Domain;



namespace Kuzaine.Tests.Fakes;

    public static class CannedGenerator
    {
        public static Entity FakeBasicProduct()
        {
            return new Entity()
            {
                Name = "Product",
                Properties = new List<EntityProperty>()
                {
                    new EntityProperty()
                    {
                        Name = "Name",
                        Type = "string",
                        CanFilter = true,
                        CanSort = false,
                    },
                }
            };
        }

        public static ApiTemplate FakeBasicApiTemplate()
        {
            return new ApiTemplate()
            {
                ProjectName = "BespokedBikes.Api",
                DbContext = new DbContextConfig()
                {
                    ContextName = "BespokedBikesDbContext",
                    DatabaseName = "BespokedBikes",
                    Provider = "SqlServer"
                },
                Entities = new List<Entity>()
                {
                    FakeBasicProduct()
                }
            };
        }
    }

