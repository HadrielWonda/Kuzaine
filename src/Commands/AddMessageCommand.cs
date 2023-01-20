using System.IO.Abstractions;
using Builders;
using Builders.Bff;
using Builders.Bff.Components.Headers;
using Builders.Bff.Components.Layouts;
using Builders.Bff.Components.Navigation;
using Builders.Bff.Components.Notifications;
using Builders.Bff.Features.Auth;
using Builders.Bff.Features.Home;
using Builders.Bff.Src;
using Domain;
using Exceptions;
using Helpers;
using Services;
using Spectre.Console;
using Spectre.Console.Cli;
using Validators;



namespace Kuzaine.Commands;

public class AddMessageCommand : Command<AddMessageCommand.Settings>
{
    private readonly IFileSystem _fileSystem;
    private readonly IConsoleWriter _consoleWriter;
    private readonly IFileParsingHelper _fileParsingHelper;
    private readonly IAnsiConsole _console;
    private readonly IKuzaineUtilities _utilities;
    private readonly IScaffoldingDirectoryStore _scaffoldingDirectoryStore;

    public AddMessageCommand(IFileSystem fileSystem,
        IConsoleWriter consoleWriter,
        IKuzaineUtilities utilities,
        IScaffoldingDirectoryStore scaffoldingDirectoryStore,
        IAnsiConsole console, IFileParsingHelper fileParsingHelper)
    {
        _fileSystem = fileSystem;
        _consoleWriter = consoleWriter;
        _utilities = utilities;
        _scaffoldingDirectoryStore = scaffoldingDirectoryStore;
        _console = console;
        _fileParsingHelper = fileParsingHelper;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Filepath>")]
        public string Filepath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var potentialSolutionDir = _utilities.GetRootDir();

        _utilities.IsSolutionDirectoryGuard(potentialSolutionDir);
        _scaffoldingDirectoryStore.SetSolutionDirectory(potentialSolutionDir);

        _fileParsingHelper.RunInitialTemplateParsingGuards(settings.Filepath);
        var template = _fileParsingHelper.GetTemplateFromFile<MessageTemplate>(settings.Filepath);
        _consoleWriter.WriteHelpText($"Your template file was parsed successfully.");

        AddMessages(_scaffoldingDirectoryStore.SolutionDirectory, template.Messages);

        _consoleWriter.WriteHelpHeader($"{Environment.NewLine}Your feature has been successfully added. Keep up the good work! {Emoji.Known.Sparkles}");
        return 0;
    }

    public void AddMessages(string solutionDirectory, List<Message> messages)
    {
        var validator = new MessageValidator();
        foreach (var message in messages)
        {
            var results = validator.Validate(message);
            if (!results.IsValid)
                throw new DataValidationErrorException(results.Errors);
        }

        messages.ForEach(message => new MessageBuilder(_utilities).CreateMessage(solutionDirectory, message));
    }
}