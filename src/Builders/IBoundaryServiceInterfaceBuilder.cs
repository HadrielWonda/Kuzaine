using Helpers;
using MediatR;
using Services;



namespace Kuzaine.Builders;

public static class IBoundaryServiceInterfaceBuilder
{
    public class IBoundaryServiceInterfaceBuilderCommand : IRequest<bool>
    {
        public IBoundaryServiceInterfaceBuilderCommand()
        {
        }
    }

    public class Handler : IRequestHandler<IBoundaryServiceInterfaceBuilderCommand, bool>
    {
        private readonly IKuzaineUtilities _utilities;
        private readonly IScaffoldingDirectoryStore _scaffoldingDirectoryStore;

        public Handler(IKuzaineUtilities utilities,
            IScaffoldingDirectoryStore scaffoldingDirectoryStore)
        {
            _utilities = utilities;
            _scaffoldingDirectoryStore = scaffoldingDirectoryStore;
        }

        public Task<bool> Handle(IBoundaryServiceInterfaceBuilderCommand request, CancellationToken cancellationToken)
        {
            var boundaryServiceName = FileNames.BoundaryServiceInterface(_scaffoldingDirectoryStore.ProjectBaseName);
            var classPath = ClassPathHelper.WebApiServicesClassPath(_scaffoldingDirectoryStore.SrcDirectory, $"{boundaryServiceName}.cs", _scaffoldingDirectoryStore.ProjectBaseName);
            var fileText = GetFileText(classPath.ClassNamespace);
            _utilities.CreateFile(classPath, fileText);
            return Task.FromResult(true);
        }

        private string GetFileText(string classNamespace)
        {
            var boundaryServiceName = FileNames.BoundaryServiceInterface(_scaffoldingDirectoryStore.ProjectBaseName);
            
            return @$"namespace {classNamespace};

public interface {boundaryServiceName}
{{
}}";
        }
    }
}
