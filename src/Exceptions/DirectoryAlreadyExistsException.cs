namespace Kuzaine.Exceptions;

using System;

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
