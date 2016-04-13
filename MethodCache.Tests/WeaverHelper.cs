namespace MethodCache.Tests
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Xml.Linq;
	using MethodCache.Fody;
	using Mono.Cecil;

	public class WeaverHelper
	{
		public static dynamic CreateInstance<T>(Assembly assembly)
		{
			Type type = assembly.GetType(typeof(T).FullName);

			return Activator.CreateInstance(type);
		}

		public static dynamic CreateInstance<T>(Assembly assembly, object parameter)
		{
			Type type = assembly.GetType(typeof(T).FullName);

			return Activator.CreateInstance(type, parameter);
		}

		public static dynamic CreateInstance<T>(Assembly assembly, object[] parameters)
		{
			Type type = assembly.GetType(typeof(T).FullName);

			return Activator.CreateInstance(type, parameters);
		}

		public static Assembly WeaveAssembly(string suffix, XElement config)
		{
			string projectPath =
				Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
					@"..\..\..\MethodCache.Tests.TestAssembly\MethodCache.Tests.TestAssembly.csproj"));
			string assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath),
				@"bin\Debug\MethodCache.Tests.TestAssembly.dll");

#if (!DEBUG)
			assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

			string newAssembly = assemblyPath.Replace(".dll", string.Format("{0}.dll", suffix));
			File.Copy(assemblyPath, newAssembly, true);

			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(newAssembly);
			ModuleWeaver weavingTask = new ModuleWeaver { ModuleDefinition = moduleDefinition };

			weavingTask.AssemblyResolver = new AssemblyResolverMock();

			weavingTask.LogInfo = (message) => Debug.WriteLine(message);
			weavingTask.LogWarning = (message) => Debug.WriteLine(message);
			weavingTask.LogError = (message) => new Exception(message);

			if (config != null)
			{
				weavingTask.Config = config;
			}

#if (DEBUG)
			weavingTask.DefineConstants.Add("DEBUG");
#endif

			weavingTask.Execute();
			moduleDefinition.Write(newAssembly);

			return Assembly.LoadFile(newAssembly);
		}
	}
}