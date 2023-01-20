﻿using System.Collections.Generic;
using System.Linq;
using Helpers;



namespace Kuzaine.Domain;

/// <summary>
/// This represents a database entity for your project
/// </summary>
public class Entity
{
    private string _plural;
    private string _lambda;
    private string _name;

    /// <summary>
    /// The name of the entity
    /// </summary>
    public string Name
    {
        get => _name.UppercaseFirstLetter();
        set => _name = value;
    }

    /// <summary>
    /// List of properties associated to the entity
    /// </summary>
    public List<EntityProperty> Properties { get; set; } = new List<EntityProperty>();

    /// <summary>
    /// The Plural name of the entity.
    /// </summary>
    public string Plural
    {
        get => _plural ?? $"{Name}s";
        set => _plural = value;
    }

    /// <summary>
    /// The value to use in lambda expressions for this entity. Will default to the first letter of the entity name if none is given.
    /// </summary>
    public string Lambda
    {
        get => _lambda ?? Name.Substring(0, 1).ToLower();
        set => _lambda = value;
    }

    /// <summary>
    /// The primary key property of the entity. Always a guid
    /// </summary>
    public static EntityProperty PrimaryKeyProperty => EntityProperty.GetPrimaryKey();


    /// <summary>
    /// The custom table name that will be used in the database. Optional and null if they want to use default value.
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// The database schema that will be used. Optional and null if they want to use default value
    /// </summary>
    public string Schema { get; set; }

    private List<Feature> _features = new List<Feature>();
    public List<Feature> Features
    {
        get => _features;
        set
        {
            var entitylessFeatures = value ?? new List<Feature>();
            _features = entitylessFeatures
                .Select(f =>
                {
                    f.EntityName = Name;
                    f.EntityPlural = Plural;
                    return f;
                })
                .ToList();
        }
    }
}
