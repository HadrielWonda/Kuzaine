﻿using System;



namespace Kuzaine.Exceptions;

[Serializable]
public class InvalidEndpointException : Exception, IKuzaineException
{
    public InvalidEndpointException() : base($"The given endpoint was not recognized.")
    {

    }

    public InvalidEndpointException(string endpoint) : base($"The endpoint `{endpoint}` was not recognized.")
    {

    }
}
