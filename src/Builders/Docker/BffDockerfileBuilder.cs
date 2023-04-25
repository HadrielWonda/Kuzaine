namespace Kuzaine.Builders.Docker;

using Helpers;
using Services;

public class BffDockerfileBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public BffDockerfileBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }


    public void CreateBffDotNetDockerfile(string projectDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.BffProjectRootClassPath(projectDirectory, $"Dockerfile");
        var fileText = GetDockerfileText(projectBaseName, true, false);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetDockerfileText(string projectBaseName, bool addNodeInstall, bool addSharedKernel)
    {
        var sharedKernelText = addSharedKernel
            ? $@"
COPY [""SharedKernel/SharedKernel.csproj"", ""./SharedKernel/""]"
            : "";

        // possibly below for yarn for bff
        // RUN npm install -g -s --no-progress yarn
        var nodeText = addNodeInstall
            ? $@"

RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt install -y nodejs"
            : "";
        return @$"FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app{nodeText}

# Copy csproj and restore as distinct layers
COPY [""{projectBaseName}/src/{projectBaseName}/{projectBaseName}.csproj"", ""./{projectBaseName}/src/{projectBaseName}/""]{sharedKernelText}
RUN dotnet restore ""./{projectBaseName}/src/{projectBaseName}/{projectBaseName}.csproj""

# Copy everything else and build
COPY . ./
RUN dotnet build ""{projectBaseName}/src/{projectBaseName}/{projectBaseName}.csproj"" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish ""{projectBaseName}/src/{projectBaseName}/{projectBaseName}.csproj"" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT [""dotnet"", ""/app/{projectBaseName}.dll""]
";
    }
}