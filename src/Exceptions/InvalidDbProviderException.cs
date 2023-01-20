﻿using System;



namespace Kuzaine.Exceptions;

[Serializable]
public class InvalidDbProviderException : Exception, IKuzaineException
{
    public InvalidDbProviderException() : base($"The given database provider was not recognized.")
    {

    }

    public InvalidDbProviderException(string dbProvider) : base($"The database provider `{dbProvider}` was not recognized.")
    {

    }
}
