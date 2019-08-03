﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using LeagueSandbox.GameServer.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace LeagueSandbox.GameServer.Scripting.CSharp
{
    public class CSharpScriptEngine
    {
        private readonly ILog _logger;
        private List<Assembly> _scriptAssembly = new List<Assembly>();

        public CSharpScriptEngine()
        {
            _logger = LoggerProvider.GetLogger();
        }

        public bool LoadSubdirectoryScripts(string folder)
        {
            var basePath = Path.GetFullPath(folder);
            var allfiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories).Where(pathString =>
            {
                var fileBasePath = Path.GetFullPath(pathString);
                var trimmedPath = fileBasePath.Remove(0, basePath.Length);
                var directories = trimmedPath.ToLower().Split(Path.DirectorySeparatorChar);

                if (directories.Contains("bin") || directories.Contains("obj"))
                {
                    return false;
                }

                return true;
            });

            return !Load(new List<string>(allfiles));
        }

        //TODO: find out why this even works
        public bool LoadSubdirectoryScriptsZip(string zipLocation)
        {
            var allScriptFiles = new List<string>();

            using (var archive = ZipFile.OpenRead(zipLocation))
            {
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.Contains("bin") || entry.FullName.Contains("obj") || !entry.FullName.Contains(".cs"))
                    {
                        continue;
                    }

                    allScriptFiles.Add(entry.FullName);
                }
            }

            return LoadZip(allScriptFiles, zipLocation);
        }

        public bool LoadZip(List<string> scriptFileLocations, string zipLocation)
        {
            var treeList = new List<SyntaxTree>();

            Parallel.For(0, scriptFileLocations.Count, i =>
            {
                using (var archive = ZipFile.OpenRead(zipLocation))
                {
                    var scriptZipEntry = archive.GetEntry(scriptFileLocations[i]);

                    if (scriptZipEntry == null)
                    {
                        return;
                    }

                    var scriptFileStream = new StreamReader(scriptZipEntry.Open(), Encoding.Default).ReadToEnd();
                    var syntaxTree = CSharpSyntaxTree.ParseText(scriptFileStream, null, scriptFileLocations[i]);

                    lock (treeList)
                    {
                        treeList.Add(syntaxTree);
                    }
                }
            });

            var assemblyName = Path.GetRandomFileName();
            var references = new List<MetadataReference>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic || assembly.Location.Equals(""))
                {
                    continue;
                }

                var metaDataReference = MetadataReference.CreateFromFile(assembly.Location);

                references.Add(metaDataReference);
            }

            var gameMetaDataReference = MetadataReference.CreateFromFile(typeof(Game).Assembly.Location);

            references.Add(gameMetaDataReference);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Release).WithConcurrentBuild(true);

            var compilation = CSharpCompilation.Create(assemblyName, treeList, references, compilationOptions);

            while (true)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var compilationResult = compilation.Emit(memoryStream);

                    if (!compilationResult.Success)
                    {
                        var invalidSourceTrees = GetInvalidSourceTrees(compilationResult);

                        if (invalidSourceTrees.Count == 0)
                        {
                            _logger.Error("Script compilation failed.");
                            return false;
                        }

                        compilation = compilation.RemoveSyntaxTrees(invalidSourceTrees);
                    }

                    if (memoryStream.GetBuffer().Length == 0)
                    {
                        continue;
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    _scriptAssembly.Add(Assembly.Load(memoryStream.GetBuffer()));

                    return true;
                }
            }
        }

        //Takes about 300 milliseconds for a single script
        public bool Load(List<string> scriptLocations)
        {
            var treeList = new List<SyntaxTree>();
            Parallel.For(0, scriptLocations.Count, i =>
            {
                using (var sr = new StreamReader(scriptLocations[i]))
                {
                    // Read the stream to a string, and write the string to the console.
                    var syntaxTree = CSharpSyntaxTree.ParseText(sr.ReadToEnd(), null, scriptLocations[i]);
                    lock (treeList)
                    {
                        treeList.Add(syntaxTree);
                    }
                }
            });
            var assemblyName = Path.GetRandomFileName();

            var references = new List<MetadataReference>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                if (!a.IsDynamic && !a.Location.Equals(""))
                    references.Add(MetadataReference.CreateFromFile(a.Location));
            //Now add game reference
            references.Add(MetadataReference.CreateFromFile(typeof(Game).Assembly.Location));
            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(OptimizationLevel.Release).WithConcurrentBuild(true);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                treeList,
                references,
                op
            );

            var errored = false;
            while (true)
                using (var ms = new MemoryStream())
                {
                    var result = compilation.Emit(ms);

                    if (result.Success)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        _scriptAssembly.Add(Assembly.Load(ms.ToArray()));
                        return errored;
                    }

                    errored |= true;
                    var invalidSourceTrees = GetInvalidSourceTrees(result);

                    if (invalidSourceTrees.Count == 0)
                    {
                        // Shouldnt happen
                        _logger.Error("Script compilation failed");
                        return true;
                    }

                    compilation = compilation.RemoveSyntaxTrees(invalidSourceTrees);
                }
        }

        //TODO: find out why this throws errors
        private List<SyntaxTree> GetInvalidSourceTrees(EmitResult result)
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            return failures.Select(diagnostic =>
            {
                var loc = diagnostic.Location.SourceTree.GetLineSpan(diagnostic.Location.SourceSpan).Span;
                _logger.Error(
                    $"Script compilation error in script {diagnostic.Location.SourceTree.FilePath}: {diagnostic.Id}\n{diagnostic.GetMessage()} on " +
                    $"Line {loc.Start.Line} pos {loc.Start.Character} to Line {loc.End.Line} pos {loc.End.Character}");
                return diagnostic.Location.SourceTree;
            }).ToList();
        }

        public T GetStaticMethod<T>(string scriptNamespace, string scriptClass, string scriptFunction)
        {
            if (_scriptAssembly == null || _scriptAssembly.Count <= 0)
            {
                return default(T);
            }

            foreach (var scriptAssembly in _scriptAssembly)
            {
                var classType = scriptAssembly.GetType(scriptNamespace + "." + scriptClass, false);

                if (classType == null)
                {
                    continue;
                }

                var desiredFunction = classType.GetMethod(scriptFunction, BindingFlags.Public | BindingFlags.Static);

                if (desiredFunction != null)
                {
                    return (T)(object)Delegate.CreateDelegate(typeof(T), desiredFunction, false);
                }
            }

            return default(T);
        }

        public T CreateObject<T>(string scriptNamespace, string scriptClass)
        {
            if (_scriptAssembly == null || _scriptAssembly.Count <= 0)
            {
                return default(T);
            }

            scriptClass = scriptClass.Replace(" ", "_");

            foreach (var scriptAssembly in _scriptAssembly)
            {
                var classType = scriptAssembly.GetType(scriptNamespace + "." + scriptClass);

                if (classType == null)
                {
                    continue;
                }

                return (T)Activator.CreateInstance(classType);
            }

            _logger.Warn($"Failed to load script: {scriptNamespace}.{scriptClass}");
            return default(T);
        }

        public static object RunFunctionOnObject(object obj, string method, params object[] args)
        {
            return obj.GetType().InvokeMember(
                method,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                obj,
                args
            );
        }

        public static T GetObjectMethod<T>(object obj, string scriptFunction)
        {
            var classType = obj.GetType();
            var desiredFunction = classType.GetMethod(scriptFunction, BindingFlags.Public | BindingFlags.Instance);

            var typeParameterType = typeof(T);
            return (T) (object) Delegate.CreateDelegate(typeParameterType, obj, desiredFunction);
        }
    }
}