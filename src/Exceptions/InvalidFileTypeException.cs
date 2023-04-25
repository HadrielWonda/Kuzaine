namespace Kuzaine.Exceptions;

using System;

[Serializable]
class InvalidFileTypeException : Exception, IKuzaineException
{
    public InvalidFileTypeException() : base($"Invalid file type. You need to use a json or yml file.")
    {

    }
}
