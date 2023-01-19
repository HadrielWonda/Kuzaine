﻿using Helpers;
using MediatR;
using Services;



namespace Kuzaine.Builders;

public static class EntityRepositoryBuilder
{
    public class Command : IRequest<bool>
    {
        public readonly string DbContextName;
        public readonly string EntityName;
        public readonly string EntityPlural;

        public Command(string dbContextName, string entityName, string entityPlural)
        {
            DbContextName = dbContextName;
            EntityPlural = entityPlural;
            EntityName = entityName;
        }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IKuzaineUtilities _utilities;
        private readonly IScaffoldingDirectoryStore _scaffoldingDirectoryStore;

        public Handler(IKuzaineUtilities utilities,
            IScaffoldingDirectoryStore scaffoldingDirectoryStore)
        {
            _utilities = utilities;
            _scaffoldingDirectoryStore = scaffoldingDirectoryStore;
        }

        public Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var classPath = ClassPathHelper.EntityServicesClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                $"{FileNames.EntityRepository(request.EntityName)}.cs", 
                request.EntityPlural, 
                _scaffoldingDirectoryStore.ProjectBaseName);
            var fileText = GetFileText(classPath.ClassNamespace, request.EntityName, request.EntityPlural, request.DbContextName);
            _utilities.CreateFile(classPath, fileText);
            return Task.FromResult(true);
        }

        private string GetFileText(string classNamespace, string entityName, string entityPlural, string dbContextName)
        {
            var entityClassPath = ClassPathHelper.EntityClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                "", 
                entityPlural, 
                _scaffoldingDirectoryStore.ProjectBaseName);
            var contextClassPath = ClassPathHelper.DbContextClassPath(_scaffoldingDirectoryStore.SrcDirectory, 
                "", 
                _scaffoldingDirectoryStore.ProjectBaseName);
            var servicesClassPath = ClassPathHelper.WebApiServicesClassPath(_scaffoldingDirectoryStore.SrcDirectory,
                "", 
                _scaffoldingDirectoryStore.ProjectBaseName);

            var genericRepositoryInterface = FileNames.GenericRepositoryInterface();
            var genericRepoName = FileNames.GenericRepository();
            var repoInterface = FileNames.EntityRepositoryInterface(entityName);
            var repoName = FileNames.EntityRepository(entityName);
            
            return @$"namespace {classNamespace};

using {entityClassPath.ClassNamespace};
using {contextClassPath.ClassNamespace};
using {servicesClassPath.ClassNamespace};

public interface {repoInterface} : {genericRepositoryInterface}<{entityName}>
{{
}}

public sealed class {repoName} : {genericRepoName}<{entityName}>, {repoInterface}
{{
    private readonly {dbContextName} _dbContext;

    public {repoName}({dbContextName} dbContext) : base(dbContext)
    {{
        _dbContext = dbContext;
    }}
}}
";
        }
    }
}
