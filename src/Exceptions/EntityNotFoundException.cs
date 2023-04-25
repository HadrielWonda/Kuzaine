﻿namespace Kuzaine.Exceptions;

using System;

[Serializable]
class EntityNotFoundException : Exception, IKuzaineException
{
    public EntityNotFoundException() : base($"Invalid file type. You need to use a json or yml file.")
    {

    }

    public EntityNotFoundException(string entity) : base($"The entity `{entity}` was not recognized.")
    {

    }
}
