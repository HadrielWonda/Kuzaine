using System;



namespace Kuzaine.Exceptions;

[Serializable]
class InvalidFileTypeException : Exception, IKuzaineException
{
    public InvalidFileTypeException() : base($"Invalid file type. You need to use a json or yml file.")
    {

    }
}
