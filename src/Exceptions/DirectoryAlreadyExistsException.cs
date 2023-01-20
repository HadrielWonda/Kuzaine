using System;



namespace Kuzaine.Exceptions;

[Serializable]
class DirectoryAlreadyExistsException : Exception, IKuzaineException
{
    public DirectoryAlreadyExistsException() : base($"This directory already exists.")
    {

    }

    public DirectoryAlreadyExistsException(string directory) : base($"The directory `{directory}` already exists.")
    {

    }
}
