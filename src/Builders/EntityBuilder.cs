﻿namespace Kuzaine.Builders;

using System;
using System.Collections.Generic;
using Domain;
using Domain.Enums;
using Helpers;
using Services;

public class EntityBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public EntityBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateEntity(string solutionDirectory, string srcDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.EntityClassPath(srcDirectory, $"{entity.Name}.cs", entity.Plural, projectBaseName);
        var fileText = GetEntityFileText(classPath.ClassNamespace, solutionDirectory, srcDirectory, entity, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    public void CreateBaseEntity(string srcDirectory, string projectBaseName, bool useSoftDelete)
    {
        var classPath = ClassPathHelper.EntityClassPath(srcDirectory, $"BaseEntity.cs", "", projectBaseName);
        var fileText = GetBaseEntityFileText(classPath.ClassNamespace, useSoftDelete);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetEntityFileText(string classNamespace, string solutionDirectory, string srcDirectory, Entity entity, string projectBaseName)
    {
        var creationClassName = EntityModel.Creation.GetClassName(entity.Name);
        var updateClassName = EntityModel.Update.GetClassName(entity.Name);
        var propString = EntityPropBuilder(entity.Properties, entity.Name);
        var usingSieve = entity.Properties.Where(e => e.CanFilter || e.CanSort).ToList().Count > 0 ? @$"{Environment.NewLine}using Sieve.Attributes;" : "";
        var tableAnnotation = EntityAnnotationBuilder(entity);
        var entityCreatedDomainMessage = FileNames.EntityCreatedDomainMessage(entity.Name);
        var entityUpdatedDomainMessage = FileNames.EntityUpdatedDomainMessage(entity.Name);

        var foreignEntityUsings = "";
        var foreignProps = entity.Properties.Where(e => e.IsForeignKey).ToList();
        foreach (var entityProperty in foreignProps)
        {
            var classPath = ClassPathHelper.EntityClassPath(srcDirectory, $"", entityProperty.ForeignEntityPlural, projectBaseName);

            foreignEntityUsings += $@"
using {classPath.ClassNamespace};";
        }
        
        var exceptionClassPath = ClassPathHelper.ExceptionsClassPath(srcDirectory, "");
        var modelClassPath = ClassPathHelper.EntityModelClassPath(srcDirectory, entity.Name, entity.Plural, null, projectBaseName);
        var domainEventsClassPath = ClassPathHelper.DomainEventsClassPath(srcDirectory, "", entity.Plural, projectBaseName);

        var createEntityVar = $"new{entity.Name.UppercaseFirstLetter()}";
        var createPropsAssignment = string.Join($"{Environment.NewLine}", entity.Properties.Where(x => x.IsPrimitiveType).Select(property =>
            $"        {createEntityVar}.{property.Name} = {creationClassName.LowercaseFirstLetter()}.{property.Name};"));
        var updatePropsAssignment = string.Join($"{Environment.NewLine}", entity.Properties.Where(x => x.IsPrimitiveType).Select(property =>
            $"        {property.Name} = {updateClassName.LowercaseFirstLetter()}.{property.Name};"));
        
        return @$"namespace {classNamespace};

using {exceptionClassPath.ClassNamespace};
using {modelClassPath.ClassNamespace};
using {domainEventsClassPath.ClassNamespace};
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;{usingSieve}{foreignEntityUsings}

{tableAnnotation}
public class {entity.Name} : BaseEntity
{{
{propString}


    public static {entity.Name} Create({creationClassName} {creationClassName.LowercaseFirstLetter()})
    {{
        var {createEntityVar} = new {entity.Name}();

{createPropsAssignment}

        {createEntityVar}.QueueDomainEvent(new {entityCreatedDomainMessage}(){{ {entity.Name} = {createEntityVar} }});
        
        return {createEntityVar};
    }}

    public {entity.Name} Update({updateClassName} {updateClassName.LowercaseFirstLetter()})
    {{
{updatePropsAssignment}

        QueueDomainEvent(new {entityUpdatedDomainMessage}(){{ Id = Id }});
        return this;
    }}
    
    protected {entity.Name}() {{ }} // For EF + Mocking
}}";
    }

    public static string GetBaseEntityFileText(string classNamespace, bool useSoftDelete)
    {
        var isDeletedProp = useSoftDelete
            ? $@"
    public bool IsDeleted {{ get; private set; }}"
            : "";

        var isDeletedMethod = useSoftDelete
            ? $@"
    
    public void UpdateIsDeleted(bool isDeleted)
    {{
        IsDeleted = isDeleted;
    }}"
            : "";

        return @$"namespace {classNamespace};

using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity
{{
    [Key]
    [Sieve(CanFilter = true, CanSort = true)]
    public Guid Id {{ get; private set; }} = Guid.NewGuid();
    
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime CreatedOn {{ get; private set; }}
    
    [Sieve(CanFilter = true, CanSort = true)]
    public string CreatedBy {{ get; private set; }}
    
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? LastModifiedOn {{ get; private set; }}
    
    [Sieve(CanFilter = true, CanSort = true)]
    public string LastModifiedBy {{ get; private set; }}{isDeletedProp}
    
    [NotMapped]
    public List<DomainEvent> DomainEvents {{ get; }} = new List<DomainEvent>();

    public void UpdateCreationProperties(DateTime createdOn, string createdBy)
    {{
        CreatedOn = createdOn;
        CreatedBy = createdBy;
    }}
    
    public void UpdateModifiedProperties(DateTime? lastModifiedOn, string lastModifiedBy)
    {{
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }}{isDeletedMethod}
    
    public void QueueDomainEvent(DomainEvent @event)
    {{
        if(!DomainEvents.Contains(@event))
            DomainEvents.Add(@event);
    }}
}}";
    }

    public static string EntityAnnotationBuilder(Entity entity)
    {
        if (string.IsNullOrEmpty(entity.TableName))
            return null;

        // must have table name to have a schema :-(
        var tableName = entity.TableName;
        return entity.Schema != null ? @$"[Table(""{tableName}"", Schema=""{entity.Schema}"")]" : @$"[Table(""{tableName}"")]";
    }

    public static string EntityPropBuilder(List<EntityProperty> props, string entityName)
    {
        var propString = "";
        foreach (var property in props)
        {
            var attributes = AttributeBuilder(property);
            
            if (property.IsSmartEnum())
            {
                propString += $@"    private {property.SmartEnumPropName} _{property.Name.LowercaseFirstLetter()};
{attributes}    public string {property.Name}
    {{
        get => _{property.Name.LowercaseFirstLetter()}.Name;
        private set
        {{
            if (!{property.SmartEnumPropName}.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof({property.Name}), value);

            _{property.Name.LowercaseFirstLetter()} = parsed;
        }}
    }}

";
            }
            else
            {
                propString += attributes;
                var defaultValue = GetDefaultValueText(property.DefaultValue, property);
                var newLine = (property.IsForeignKey && !property.IsMany)
                    ? Environment.NewLine
                    : $"{Environment.NewLine}{Environment.NewLine}";

                if (property.IsPrimitiveType || property.IsMany)
                    propString += $@"    public {property.Type} {property.Name} {{ get; private set; }}{defaultValue}{newLine}";

                propString += GetForeignProp(property, entityName);
            }
        }

        return propString.RemoveLastNewLine().RemoveLastNewLine();
    }

    private static string GetDefaultValueText(string defaultValue, EntityProperty prop)
    {
        if (prop.Type == "string")
            return defaultValue == null ? "" : @$" = ""{defaultValue}"";";

        if ((prop.Type.IsGuidPropertyType() && !prop.Type.Contains("?") && !prop.IsForeignKey))
            return !string.IsNullOrEmpty(defaultValue) ? @$" = Guid.Parse(""{defaultValue}"");" : "";

        return string.IsNullOrEmpty(defaultValue) ? "" : $" = {defaultValue};";
    }

    private static string AttributeBuilder(EntityProperty entityProperty)
    {
        var attributeString = "";
        if (entityProperty.IsRequired)
            attributeString += @$"    [Required]{Environment.NewLine}";
        if (entityProperty.IsPrimativeForeignKey)
        {
            attributeString += @$"    [JsonIgnore, IgnoreDataMember]
    [ForeignKey(""{entityProperty.ForeignEntityName}"")]{Environment.NewLine}";
        }
    
        if (entityProperty.IsMany || !entityProperty.IsPrimitiveType)
            attributeString += $@"    [JsonIgnore, IgnoreDataMember]{Environment.NewLine}";
        if (entityProperty.CanFilter || entityProperty.CanSort)
            attributeString += @$"    [Sieve(CanFilter = {entityProperty.CanFilter.ToString().ToLower()}, CanSort = {entityProperty.CanSort.ToString().ToLower()})]{Environment.NewLine}";

        attributeString += ColumnAttributeBuilder(entityProperty);

        return attributeString;
    }

    private static string ColumnAttributeBuilder(EntityProperty entityProperty)
    {
        if (!string.IsNullOrEmpty(entityProperty.ColumnName) || !string.IsNullOrEmpty(entityProperty.ColumnType))
        {
            var columnString = "";
            if (!string.IsNullOrEmpty(entityProperty.ColumnName))
                columnString += @$"""{entityProperty.ColumnName}""";

            if (!string.IsNullOrEmpty(entityProperty.ColumnType))
            {
                if (!string.IsNullOrEmpty(entityProperty.ColumnName))
                    columnString += ",";

                columnString += @$"TypeName = ""{entityProperty.ColumnType}""";
            }

            return @$"    [Column({columnString})]{Environment.NewLine}";
        }

        return "";
    }

    private static string GetForeignProp(EntityProperty prop, string entityName)
    {
        var propName = !prop.IsPrimitiveType ? prop.Name : prop.ForeignEntityName;
        
        if (propName == entityName)
            propName = $"Parent{propName}";
        
        return !string.IsNullOrEmpty(prop.ForeignEntityName) && !prop.IsMany ? $@"    public {prop.ForeignEntityName} {propName} {{ get; private set; }}{Environment.NewLine}{Environment.NewLine}" : "";
    }

    public void CreateUserEntity(string srcDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.EntityClassPath(srcDirectory, $"{entity.Name}.cs", entity.Plural, projectBaseName);
        var fileText = GetUserEntityFileText(classPath.ClassNamespace, srcDirectory, entity, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetUserEntityFileText(string classNamespace, string srcDirectory, Entity entity, string projectBaseName)
    {
        var dtoClassPath = ClassPathHelper.DtoClassPath(srcDirectory, $"", entity.Plural, projectBaseName);
        var domainEventsClassPath = ClassPathHelper.DomainEventsClassPath(srcDirectory, "", entity.Plural, projectBaseName);
        var emailsClassPath = ClassPathHelper.EntityClassPath(srcDirectory, $"", "Emails", projectBaseName);
        var exceptionClassPath = ClassPathHelper.ExceptionsClassPath(srcDirectory, "");
        var modelClassPath = ClassPathHelper.EntityModelClassPath(srcDirectory, "User", "Users", null, projectBaseName);

        return @$"namespace {classNamespace};

using {exceptionClassPath.ClassNamespace};
using {dtoClassPath.ClassNamespace};
using {domainEventsClassPath.ClassNamespace};
using {emailsClassPath.ClassNamespace};
using {modelClassPath.ClassNamespace};
using Roles;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using Sieve.Attributes;

public class User : BaseEntity
{{
    [Sieve(CanFilter = true, CanSort = true)]
    public string Identifier {{ get; private set; }}

    [Sieve(CanFilter = true, CanSort = true)]
    public string FirstName {{ get; private set; }}

    [Sieve(CanFilter = true, CanSort = true)]
    public string LastName {{ get; private set; }}

    [Sieve(CanFilter = true, CanSort = true)]
    public Email Email {{ get; private set; }}

    [Sieve(CanFilter = true, CanSort = true)]
    public string Username {{ get; private set; }}

    [JsonIgnore]
    [IgnoreDataMember]
    public ICollection<UserRole> Roles {{ get; private set; }} = new List<UserRole>();


    public static User Create(UserForCreation userForCreation)
    {{
        ValidationException.ThrowWhenNullOrWhitespace(userForCreation.Identifier, 
            ""Please provide an identifier."");

        var newUser = new User();

        newUser.Identifier = userForCreation.Identifier;
        newUser.FirstName = userForCreation.FirstName;
        newUser.LastName = userForCreation.LastName;
        newUser.Email = new Email(userForCreation.Email);
        newUser.Username = userForCreation.Username;

        newUser.QueueDomainEvent(new UserCreated(){{ User = newUser }});
        
        return newUser;
    }}

    public User Update(UserForUpdate userForUpdate)
    {{
        ValidationException.ThrowWhenNullOrWhitespace(userForUpdate.Identifier, 
            ""Please provide an identifier."");

        Identifier = userForUpdate.Identifier;
        FirstName = userForUpdate.FirstName;
        LastName = userForUpdate.LastName;
        Email = new Email(userForUpdate.Email);
        Username = userForUpdate.Username;

        QueueDomainEvent(new UserUpdated(){{ Id = Id }});
        return this;
    }}

    public UserRole AddRole(Role role)
    {{
        var newList = Roles.ToList();
        var userRole = UserRole.Create(Id, role);
        newList.Add(userRole);
        UpdateRoles(newList);
        return userRole;
    }}

    public UserRole RemoveRole(Role role)
    {{
        var newList = Roles.ToList();
        var roleToRemove = Roles.FirstOrDefault(x => x.Role == role);
        newList.Remove(roleToRemove);
        UpdateRoles(newList);
        return roleToRemove;
    }}

    private void UpdateRoles(IList<UserRole> updates)
    {{
        var additions = updates.Where(userRole => Roles.All(x => x.Role != userRole.Role)).ToList();
        var removals = Roles.Where(userRole => updates.All(x => x.Role != userRole.Role)).ToList();
    
        var newList = Roles.ToList();
        removals.ForEach(toRemove => newList.Remove(toRemove));
        additions.ForEach(newRole => newList.Add(newRole));
        Roles = newList;
        QueueDomainEvent(new UserRolesUpdated(){{ UserId = Id }});
    }}
    
    protected User() {{ }} // For EF + Mocking
}}";
    }
    

    public void CreateUserRoleEntity(string srcDirectory, string projectBaseName)
    {
        var entityName = "UserRole";
        var classPath = ClassPathHelper.EntityClassPath(srcDirectory, $"{entityName}.cs", "Users", projectBaseName);
        var fileText = GetUserRoleEntityFileText(classPath.ClassNamespace, srcDirectory, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetUserRoleEntityFileText(string classNamespace, string srcDirectory, string projectBaseName)
    {
        var domainEventsClassPath = ClassPathHelper.DomainEventsClassPath(srcDirectory, "", "Users", projectBaseName);

        return @$"namespace {classNamespace};

using {domainEventsClassPath.ClassNamespace};
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Roles;

public class UserRole : BaseEntity
{{
    [JsonIgnore]
    [IgnoreDataMember]
    [ForeignKey(""User"")]
    public Guid UserId {{ get; private set; }}
    public User User {{ get; private set; }}

    public Role Role {{ get; private set; }}
    

    public static UserRole Create(Guid userId, Role role)
    {{
        var newUserRole = new UserRole
        {{
            UserId = userId,
            Role = role
        }};

        newUserRole.QueueDomainEvent(new UserRolesUpdated(){{ UserId = userId }});
        
        return newUserRole;
    }}
    
    protected UserRole() {{ }} // For EF + Mocking
}}";
    }
    

    public void CreateRolePermissionsEntity(string srcDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.EntityClassPath(srcDirectory, $"{entity.Name}.cs", entity.Plural, projectBaseName);
        var fileText = GetRolePermissionsEntityFileText(classPath.ClassNamespace, srcDirectory, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetRolePermissionsEntityFileText(string classNamespace, string srcDirectory, string projectBaseName)
    {
        var exceptionClassPath = ClassPathHelper.ExceptionsClassPath(srcDirectory, "");
        var modelClassPath = ClassPathHelper.EntityModelClassPath(srcDirectory, "RolePermission", "RolePermissions", null, projectBaseName);
        return @$"namespace {classNamespace};

using Dtos;
using DomainEvents;
using Roles;
using Domain;
using {exceptionClassPath.ClassNamespace};
using {modelClassPath.ClassNamespace};

public class RolePermission : BaseEntity
{{
    public Role Role {{ get; private set; }}
    public string Permission {{ get; private set; }}


    public static RolePermission Create(RolePermissionForCreation rolePermissionForCreation)
    {{
        ValidationException.Must(BeAnExistingPermission(rolePermissionForCreation.Permission), 
            ""Please use a valid permission."");

        var newRolePermission = new RolePermission();

        newRolePermission.Role = new Role(rolePermissionForCreation.Role);
        newRolePermission.Permission = rolePermissionForCreation.Permission;

        newRolePermission.QueueDomainEvent(new RolePermissionCreated(){{ RolePermission = newRolePermission }});
        
        return newRolePermission;
    }}

    public RolePermission Update(RolePermissionForUpdate rolePermissionForUpdate)
    {{
        ValidationException.Must(BeAnExistingPermission(rolePermissionForUpdate.Permission), 
            ""Please use a valid permission."");

        Role = new Role(rolePermissionForUpdate.Role);
        Permission = rolePermissionForUpdate.Permission;

        QueueDomainEvent(new RolePermissionUpdated(){{ Id = Id }});
        return this;
    }}
    
    private static bool BeAnExistingPermission(string permission)
    {{
        return Permissions.List().Contains(permission, StringComparer.InvariantCultureIgnoreCase);
    }}
    
    protected RolePermission() {{ }} // For EF + Mocking
}}";
    }
}
