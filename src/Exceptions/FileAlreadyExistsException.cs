﻿namespace Kuzaine.Exceptions;

using System;

[Serializable]
public class FileAlreadyExistsException : Exception, IKuzaineException
{
    public FileAlreadyExistsException() : base($"This file already exists.")
    {

    }

    public FileAlreadyExistsException(string file) : base($"The file `{file}` already exists.")
    {

    }
}
