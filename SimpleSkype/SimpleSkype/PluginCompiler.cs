using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PluginInterface;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Diagnostics;

namespace SimpleSkype
{
    class Loader
    {
        public Plugin[] Load()
        {
            if (!(Directory.Exists("plugins")))
            {
                Directory.CreateDirectory("plugins");
            }
            if (!(Directory.Exists("plugins/source")))
            {
                Directory.CreateDirectory("plugins/source");
            }
            List<Plugin> loadedPlugins = new List<Plugin>();
            foreach (string singleFile in Directory.GetFiles("plugins/source", "*.cs"))
            {
                if (Path.GetFileName(singleFile).StartsWith("-"))
                {
                    Console.Write("[COMPILE]: Skipped [");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(singleFile);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("]");
                    Console.WriteLine();
                    continue;
                }
                Stopwatch compileTimer = new Stopwatch();
                compileTimer.Start();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[COMPILE]: Currently compiling [");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(singleFile);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("] > ");
                PluginCompiler pluginCompiler = new PluginCompiler(singleFile);
                bool compileSuccess = pluginCompiler.compilePlugin();
                compileTimer.Stop();
                Console.ForegroundColor = (compileSuccess) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
                Console.Write(string.Format("{0} [{1}ms]", ((compileSuccess) ? "OK" : "FAIL"), compileTimer.ElapsedMilliseconds.ToString()));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                if (!compileSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pluginCompiler.errorMessage);
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            foreach (string singleFile in Directory.GetFiles("plugins", "*.dll"))
            {
                foreach (Type singleType in Assembly.LoadFile(Path.GetFullPath(singleFile)).GetTypes())
                {
                    if (singleType.GetInterfaces().Contains(typeof(Plugin)))
                    {
                        Plugin newPlugin = (Plugin)Activator.CreateInstance(singleType);
                        loadedPlugins.Add(newPlugin);
                    }
                }
            }
            return loadedPlugins.ToArray();
        }
    }
    class PluginCompiler
    {
        string fullPath;
        public string finalPath;
        public string errorMessage;
        public PluginCompiler(string sourcePath)
        {
            fullPath = Path.GetFullPath(sourcePath);
        }
        public bool compilePlugin(string outputAssembly = null, string outputPath = "plugins/")
        {
            string toOutput = string.Format("{0}{1}.dll", outputPath, ((outputAssembly == null) ? Path.GetFileNameWithoutExtension(fullPath) : outputAssembly));
            if (File.Exists(toOutput))
            {
                File.Delete(toOutput);
            }
            List<string> referencedAssemblies = new List<string>();
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler codeCompiler = codeProvider.CreateCompiler();
            CompilerParameters compileParams = new CompilerParameters();
            compileParams.ReferencedAssemblies.Add(typeof(Plugin).Assembly.Location);
            compileParams.ReferencedAssemblies.Add(typeof(Skype4Sharp.Skype4Sharp).Assembly.Location);
            compileParams.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            compileParams.ReferencedAssemblies.Add("System.dll");
            foreach (string singleCodeLine in File.ReadAllLines(fullPath))
            {
                if (singleCodeLine.StartsWith("//ref "))
                {
                    try
                    {
                        compileParams.ReferencedAssemblies.Add(singleCodeLine.Split(' ')[1]);
                    }
                    catch { }
                }
            }
            compileParams.GenerateExecutable = false;
            compileParams.OutputAssembly = toOutput;
            CompilerResults compileResults = codeCompiler.CompileAssemblyFromSource(compileParams, File.ReadAllText(fullPath));
            if (compileResults.Errors.Count == 0)
            {
                finalPath = Path.GetFullPath(toOutput);
                return true;
            }
            else
            {
                errorMessage = string.Format("[COMPILE]: You have {0} error{1} in your code.", compileResults.Errors.Count.ToString(), (compileResults.Errors.Count == 1) ? "" : "s");
                foreach (CompilerError singleError in compileResults.Errors)
                {
                    errorMessage += string.Format("{0}    {1} [line {2}]", Environment.NewLine, singleError.ErrorText.TrimEnd(), singleError.Line.ToString());
                }
            }
            return false;
        }
    }
}
