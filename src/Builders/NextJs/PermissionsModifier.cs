﻿namespace Kuzaine.Builders.NextJs;

using System;
using System.IO.Abstractions;
using Kuzaine.Helpers;
using Kuzaine.Services;

public class PermissionsModifier
{
    private readonly IFileSystem _fileSystem;

    public PermissionsModifier(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public void AddPermission(string nextSrcDirectory, string permission)
    {
        var classPath = ClassPathHelper.NextJsPermissionTypesClassPath(nextSrcDirectory);

        if (!_fileSystem.Directory.Exists(classPath.ClassDirectory))
            _fileSystem.Directory.CreateDirectory(classPath.ClassDirectory);

        if (!_fileSystem.File.Exists(classPath.FullClassPath))
            return; // silently skip this. just want to add this as a convenience if the scaffolding set up is used.

        var fileText = _fileSystem.File.ReadAllText(classPath.FullClassPath);
        if (fileText.Contains(@$"""{permission}"","))
            return;
        
        var tempPath = $"{classPath.FullClassPath}temp";
        using (var input = _fileSystem.File.OpenText(classPath.FullClassPath))
        {
            using var output = _fileSystem.File.CreateText(tempPath);
            {
                string line;
                while (null != (line = input.ReadLine()))
                {
                    var newText = $"{line}";
                    if (line.Contains("permissions marker"))
                        newText += @$"{Environment.NewLine}  ""{permission}"",";

                    output.WriteLine(newText);
                }
            }
        }

        // delete the old file and set the name of the new one to the original name
        _fileSystem.File.Delete(classPath.FullClassPath);
        _fileSystem.File.Move(tempPath, classPath.FullClassPath);
    }
}
