using System;



namespace Kuzaine.Exceptions;

[Serializable]
public class DuplicateSingletonException : Exception, IKuzaineException
{
    public DuplicateSingletonException() : base($"This singleton has been instantiated more than once. Impliment a different one")
    {

    }
}
