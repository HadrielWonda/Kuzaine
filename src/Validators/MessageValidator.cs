using Domain;
using FluentValidation;



namespace Kuzaine.Validators;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Please specify a name for your message.");
    }
}
