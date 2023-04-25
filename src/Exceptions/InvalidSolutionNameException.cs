namespace Kuzaine.Exceptions;

using System;

[Serializable]
class InvalidSolutionNameException : Exception, IKuzaineException
{
    public InvalidSolutionNameException() : base($"Invalid template file. Please enter a valid solution name.")
    {

    }
}
