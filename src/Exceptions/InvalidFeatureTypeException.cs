using System;



namespace Kuzaine.Exceptions;

[Serializable]
public class InvalidFeatureTypeException : Exception, IKuzaineException
{
    public InvalidFeatureTypeException() : base($"The given feature type was not recognized.")
    {

    }

    public InvalidFeatureTypeException(string type) : base($"The feature type `{type}` was not recognized.")
    {

    }
}
