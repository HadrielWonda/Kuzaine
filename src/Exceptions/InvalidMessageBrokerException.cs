using System;



namespace Kuzaine.Exceptions;

[Serializable]
public class InvalidMessageBrokerException : Exception, IKuzaineException
{
    public InvalidMessageBrokerException() : base($"The given message broker was not recognized.")
    {
    }

    public InvalidMessageBrokerException(string broker) : base($"The message broker `{broker}` was not recognized.")
    {
    }
}
