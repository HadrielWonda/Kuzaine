namespace Kuzaine.Exceptions;

using System;

[Serializable]
public class DuplicateSingletonException : Exception, IKuzaineException
{
    public DuplicateSingletonException() : base($"This singleton has been instantiated more than once.")
    {

    }
}
