﻿using System;
using System.Runtime.Serialization;



namespace Kuzaine.Exceptions;

[Serializable]
internal class NextConfigNotFoundException : Exception, IKuzaineException
{
    public NextConfigNotFoundException() : base($"A NextJS config was not found in your current directory. Please make sure you are in the root of your NextJs project.")
    {
    }

    public NextConfigNotFoundException(string message) : base(message)
    {
    }

    public NextConfigNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected NextConfigNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
