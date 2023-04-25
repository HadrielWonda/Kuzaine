namespace Kuzaine.Builders;

using Helpers;
using MediatR;
using Services;

public static class UpdatedDomainEventBuilder
{
    public class UpdatedDomainEventBuilderCommand : IRequest<bool>
    {
        public string EntityName { get; set; }
        public string EntityPlural { get; set; }

        public UpdatedDomainEventBuilderCommand(string entityName, string entityPlural)
        {
            EntityName = entityName;
            EntityPlural = entityPlural;
        }
    }

    public class Handler : IRequestHandler<UpdatedDomainEventBuilderCommand, bool>
    {
        private readonly IKuzaineUtilities _utilities;
        private readonly IScaffoldingDirectoryStore _scaffoldingDirectoryStore;

        public Handler(IKuzaineUtilities utilities,
            IScaffoldingDirectoryStore scaffoldingDirectoryStore)
        {
            _utilities = utilities;
            _scaffoldingDirectoryStore = scaffoldingDirectoryStore;
        }

        public Task<bool> Handle(UpdatedDomainEventBuilderCommand request, CancellationToken cancellationToken)
        {
            var classPath = ClassPathHelper.DomainEventsClassPath(_scaffoldingDirectoryStore.SrcDirectory,
                $"{FileNames.EntityUpdatedDomainMessage(request.EntityName)}.cs",
                request.EntityPlural,
                _scaffoldingDirectoryStore.ProjectBaseName);
            var fileText = GetFileText(classPath.ClassNamespace, request.EntityName);
            _utilities.CreateFile(classPath, fileText);
            return Task.FromResult(true);
        }

        private static string GetFileText(string classNamespace, string entityName)
        {
            return @$"namespace {classNamespace};

public sealed class {FileNames.EntityUpdatedDomainMessage(entityName)} : DomainEvent
{{
    public Guid Id {{ get; set; }} 
}}
            ";
        }
    }
}