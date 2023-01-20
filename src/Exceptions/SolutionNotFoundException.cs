﻿using System;
using System.Runtime.Serialization;



namespace Kuzaine.Exceptions;

[Serializable]
internal class SolutionNotFoundException : Exception, IKuzaineException
{
    public SolutionNotFoundException() : base($"A solution file was not found in your current directory. Please make sure you are in the solution directory for your project.")
    {
    }

    public SolutionNotFoundException(string message) : base(message)
    {
    }

    public SolutionNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SolutionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
