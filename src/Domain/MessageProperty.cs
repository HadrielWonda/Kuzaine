﻿using Helpers;



namespace Kuzaine.Domain;

public class MessageProperty
{
    private string _name;
    private string _type = "string";

    /// <summary>
    /// Name of the property
    /// </summary>
    public string Name
    {
        get => _name.UppercaseFirstLetter();
        set => _name = value;
    }

    /// <summary>
    /// Type of property (e.g. string, int, DateTime?, etc.)
    /// </summary>
    public string Type
    {
        get => _type;
        set => _type = KuzaineUtilities.PropTypeCleanupDotNet(value);
    }
}
