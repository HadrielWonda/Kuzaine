﻿namespace Kuzaine.Builders.EntityModels;

using System;
using System.Collections.Generic;
using Domain;
using Domain.Enums;
using Helpers;
using Services;

public static class EntityModelFileTextGenerator
{
    public static string GetEntityModelText(IClassPath modelClassPath, Entity entity, EntityModel model)
    {
        var propString = string.Empty;
        propString += EntityModelPropBuilder(entity.Properties, model);

        return @$"namespace {modelClassPath.ClassNamespace};

public sealed class {model.GetClassName(entity.Name)}
{{
{propString}
}}
";
    }

    public static string EntityModelPropBuilder(List<EntityProperty> props, EntityModel model)
    {
        var propString = string.Empty;
        for (var eachProp = 0; eachProp < props.Count; eachProp++)
        {
            if (!props[eachProp].CanManipulate && (model == EntityModel.Creation || model == EntityModel.Update))
                continue;
            if (props[eachProp].IsForeignKey && props[eachProp].IsMany)
                continue;
            if (!props[eachProp].IsPrimitiveType)
                continue;

            string newLine = eachProp == props.Count - 1 ? "" : Environment.NewLine;
            propString += $@"    public {props[eachProp].Type} {props[eachProp].Name} {{ get; set; }}{newLine}";
        }

        return propString;
    }
}
