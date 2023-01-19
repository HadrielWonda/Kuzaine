﻿using Domain;
using Domain.Enums;
using Helpers;
using MediatR;
using Services;



namespace Kuzaine.Builders;

public static class ValueObjectMappingsBuilder
{
    public class ValueObjectMappingsBuilderCommand : IRequest<bool>
    {
        public readonly bool HasAuth;

        public ValueObjectMappingsBuilderCommand(bool hasAuth)
        {
            HasAuth = hasAuth;
        }
    }

    public class Handler : IRequestHandler<ValueObjectMappingsBuilderCommand, bool>
    {
        private readonly IKuzaineUtilities _utilities;
        private readonly IScaffoldingDirectoryStore _scaffoldingDirectoryStore;

        public Handler(IKuzaineUtilities utilities,
            IScaffoldingDirectoryStore scaffoldingDirectoryStore)
        {
            _utilities = utilities;
            _scaffoldingDirectoryStore = scaffoldingDirectoryStore;
        }

        public Task<bool> Handle(ValueObjectMappingsBuilderCommand request, CancellationToken cancellationToken)
        {
            var percentClassPath = ClassPathHelper.WebApiValueObjectMappingsClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                ValueObjectEnum.Percent,
                _scaffoldingDirectoryStore.ProjectBaseName);
            var percentFileText = GetPercentFileText(percentClassPath.ClassNamespace);
            _utilities.CreateFile(percentClassPath, percentFileText);
            
            var addressClassPath = ClassPathHelper.WebApiValueObjectMappingsClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                ValueObjectEnum.Address,
                _scaffoldingDirectoryStore.ProjectBaseName);
            var addressFileText = GetAddressFileText(addressClassPath.ClassNamespace);
            _utilities.CreateFile(addressClassPath, addressFileText);
            
            var monetaryAmountClassPath = ClassPathHelper.WebApiValueObjectMappingsClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                ValueObjectEnum.MonetaryAmount,
                _scaffoldingDirectoryStore.ProjectBaseName);
            var monetaryAmountFileText = GetMonetaryAmountFileText(monetaryAmountClassPath.ClassNamespace);
            _utilities.CreateFile(monetaryAmountClassPath, monetaryAmountFileText);
            
            var emailClassPath = ClassPathHelper.WebApiValueObjectMappingsClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                ValueObjectEnum.Email,
                _scaffoldingDirectoryStore.ProjectBaseName);
            var emailFileText = GetEmailFileText(emailClassPath.ClassNamespace);
            _utilities.CreateFile(emailClassPath, emailFileText);

            if (request.HasAuth)
            {
                var roleClassPath = ClassPathHelper.WebApiValueObjectMappingsClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                    ValueObjectEnum.Role,
                    _scaffoldingDirectoryStore.ProjectBaseName);
                var roleFileText = GetRoleFileText(roleClassPath.ClassNamespace);
                _utilities.CreateFile(roleClassPath, roleFileText);
            }
            
            return Task.FromResult(true);
        }
        
        private string GetPercentFileText(string classNamespace)
        {
            var mappingName = FileNames.GetMappingName(ValueObjectEnum.Percent.Name);
            var voClassPath = ClassPathHelper.SharedKernelDomainClassPath(_scaffoldingDirectoryStore.SolutionDirectory, "");
            
            return @$"namespace {classNamespace};

using {voClassPath.ClassNamespace};
using Mapster;

public sealed class {mappingName} : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{
        config.NewConfig<decimal, Percent>()
            .MapWith(value => new Percent(value));
        config.NewConfig<Percent, decimal>()
            .MapWith(percent => percent.Value);
    }}
}}";
        }
        
        private string GetAddressFileText(string classNamespace)
        {
            var mappingName = FileNames.GetMappingName(ValueObjectEnum.Address.Name);
            var voClassPath = ClassPathHelper.SharedKernelDomainClassPath(_scaffoldingDirectoryStore.SolutionDirectory, "");
            var dtoClassPath = ClassPathHelper.WebApiValueObjectDtosClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                ValueObjectEnum.Address,
                Dto.Creation, // dto type doesn't actually matter here
                _scaffoldingDirectoryStore.ProjectBaseName);
            
            return @$"namespace {classNamespace};

using {dtoClassPath.ClassNamespace};
using {voClassPath.ClassNamespace};
using Mapster;

public sealed class {mappingName} : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{
        config.NewConfig<string, PostalCode>()
            .MapWith(value => new PostalCode(value));
        config.NewConfig<PostalCode, string>()
            .MapWith(postalCode => postalCode.Value);

        config.NewConfig<AddressDto, Address>()
            .MapWith(address => new Address(address.Line1, address.Line2, address.City, address.State, address.PostalCode, address.Country))
            .TwoWays();
        config.NewConfig<AddressForCreationDto, Address>()
            .MapWith(address => new Address(address.Line1, address.Line2, address.City, address.State, address.PostalCode, address.Country))
            .TwoWays();
        config.NewConfig<AddressForUpdateDto, Address>()
            .MapWith(address => new Address(address.Line1, address.Line2, address.City, address.State, address.PostalCode, address.Country))
            .TwoWays();
    }}
}}";
        }
        
        private string GetMonetaryAmountFileText(string classNamespace)
        {
            var mappingName = FileNames.GetMappingName(ValueObjectEnum.MonetaryAmount.Name);
            var voClassPath = ClassPathHelper.SharedKernelDomainClassPath(_scaffoldingDirectoryStore.SolutionDirectory, "");
            
            return @$"namespace {classNamespace};

using {voClassPath.ClassNamespace};
using Mapster;

public sealed class {mappingName} : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{
        config.NewConfig<decimal, MonetaryAmount>()
            .MapWith(value => new MonetaryAmount(value));
        config.NewConfig<MonetaryAmount, decimal>()
            .MapWith(monetaryAmount => monetaryAmount.Amount);
    }}
}}";
        }
        
        private string GetRoleFileText(string classNamespace)
        {
            var mappingName = FileNames.GetMappingName(ValueObjectEnum.Role.Name);
            var voClassPath = ClassPathHelper.SharedKernelDomainClassPath(_scaffoldingDirectoryStore.SolutionDirectory, "");
            
            return @$"namespace {classNamespace};

using {voClassPath.ClassNamespace};
using Mapster;

public sealed class {mappingName} : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{
        config.NewConfig<string, Role>()
            .MapWith(value => new Role(value));
        config.NewConfig<Role, string>()
            .MapWith(role => role.Value);
    }}
}}";
        }
        
        private string GetEmailFileText(string classNamespace)
        {
            var mappingName = FileNames.GetMappingName(ValueObjectEnum.Email.Name);
            var voClassPath = ClassPathHelper.SharedKernelDomainClassPath(_scaffoldingDirectoryStore.SolutionDirectory, "");
            
            return @$"namespace {classNamespace};

using {voClassPath.ClassNamespace};
using Mapster;

public sealed class {mappingName} : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{
        config.NewConfig<string, Email>()
            .MapWith(value => new Email(value));
        config.NewConfig<Email, string>()
            .MapWith(email => email.Value);
    }}
}}";
        }
    }
}
