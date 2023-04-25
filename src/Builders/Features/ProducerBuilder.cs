﻿namespace Kuzaine.Builders.Features;

using System;
using Domain;
using Helpers;
using Services;

public class ProducerBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ProducerBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateProducerFeature(string solutionDirectory, string srcDirectory, Producer producer, string projectBaseName)
    {
        var classPath = ClassPathHelper.ProducerFeaturesClassPath(srcDirectory, $"{producer.ProducerName}.cs", producer.DomainDirectory, projectBaseName);
        var fileText = GetProducerRegistration(classPath.ClassNamespace, producer, solutionDirectory, srcDirectory, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    public string GetProducerRegistration(string classNamespace, Producer producer, string solutionDirectory, string srcDirectory, string projectBaseName)
    {
        var context = _utilities.GetDbContext(srcDirectory, projectBaseName);
        var contextClassPath = ClassPathHelper.DbContextClassPath(srcDirectory, "", projectBaseName);
        var dbReadOnly = producer.UsesDb ? @$"{Environment.NewLine}        private readonly {context} _db;" : "";
        var dbProp = producer.UsesDb ? @$"{context} db, " : "";
        var assignDb = producer.UsesDb ? @$"{Environment.NewLine}            _db = db;" : "";
        var contextUsing = producer.UsesDb ? $@"
using {contextClassPath.ClassNamespace};" : "";

        var messagesClassPath = ClassPathHelper.MessagesClassPath(solutionDirectory, "");

        var propTypeToReturn = "bool";
        var commandName = $"{producer.ProducerName}Command";

        return @$"namespace {classNamespace};

using {messagesClassPath.ClassNamespace};
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;{contextUsing}

public static class {producer.ProducerName}
{{
    public sealed class {commandName} : IRequest<{propTypeToReturn}>
    {{
        public {commandName}()
        {{
        }}
    }}

    public sealed class Handler : IRequestHandler<{commandName}, {propTypeToReturn}>
    {{
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;{dbReadOnly}

        public Handler({dbProp}IMapper mapper, IPublishEndpoint publishEndpoint)
        {{
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;{assignDb}
        }}

        public async Task<{propTypeToReturn}> Handle({commandName} request, CancellationToken cancellationToken)
        {{
            var message = new {FileNames.MessageClassName(producer.MessageName)}
            {{
                // map content to message here or with mapster
            }};
            await _publishEndpoint.Publish<{FileNames.MessageInterfaceName(producer.MessageName)}>(message, cancellationToken);

            return true;
        }}
    }}
}}";
    }
}
