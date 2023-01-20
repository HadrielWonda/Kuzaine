using System.IO.Abstractions;
using Domain;
using Helpers;
using MediatR;
using Services;
using Spectre.Console;
using Spectre.Console.Cli;



namespace Kuzaine.Commands;

public class AddNextEntityCommand : Command<AddNextEntityCommand.Settings>
{
    private readonly IFileSystem _fileSystem;
    private readonly IConsoleWriter _consoleWriter;
    private readonly IKuzaineUtilities _utilities;
    private readonly IScaffoldingDirectoryStore _scaffoldingDirectoryStore;
    private readonly IFileParsingHelper _fileParsingHelper;
    private readonly IMediator _mediator;

    public AddNextEntityCommand(IFileSystem fileSystem,
        IConsoleWriter consoleWriter,
        IKuzaineUtilities utilities,
        IScaffoldingDirectoryStore scaffoldingDirectoryStore, IFileParsingHelper fileParsingHelper, IMediator mediator)
    {
        _fileSystem = fileSystem;
        _consoleWriter = consoleWriter;
        _utilities = utilities;
        _scaffoldingDirectoryStore = scaffoldingDirectoryStore;
        _fileParsingHelper = fileParsingHelper;
        _mediator = mediator;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Filepath>")]
        public string Filepath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var potentialNextRootDir = _utilities.GetRootDir();

        _utilities.IsNextJsRootDir(potentialNextRootDir);
        _scaffoldingDirectoryStore.SetNextJsDir(potentialNextRootDir);

        _fileParsingHelper.RunInitialTemplateParsingGuards(settings.Filepath);
        var template = _fileParsingHelper.GetTemplateFromFile<NextJsEntityTemplate>(settings.Filepath);
        _consoleWriter.WriteHelpText($"Your template file was parsed successfully.");
        
        new NextJsEntityScaffoldingService(_utilities, _fileSystem, _mediator).ScaffoldEntities(template, _scaffoldingDirectoryStore.SpaSrcDirectory);

        _consoleWriter.WriteHelpHeader($"{Environment.NewLine}Your entity scaffolding has been successfully added. Keep up the good work! {Emoji.Known.Sparkles}");
        return 0;
    }
}