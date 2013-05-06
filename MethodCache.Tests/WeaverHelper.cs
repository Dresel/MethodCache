namespace MethodCache.Tests
{
	using System;
	using System.IO;
	using System.Reflection;
	using MethodCache.Fody;
	using Mono.Cecil;

	public class WeaverHelper
	{
		#region Public Methods and Operators

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

		public static Assembly WeaveAssembly()
		{
			string projectPath =
				Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
					@"..\..\..\MethodCache.ReferenceAssembly\MethodCache.ReferenceAssembly.csproj"));
			string assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\MethodCache.ReferenceAssembly.dll");

#if (!DEBUG)
			assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

			string newAssembly = assemblyPath.Replace(".dll", "2.dll");
			File.Copy(assemblyPath, newAssembly, true);

			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(newAssembly);
			ModuleWeaver weavingTask = new ModuleWeaver { ModuleDefinition = moduleDefinition };

			weavingTask.Execute();
			moduleDefinition.Write(newAssembly);

			return Assembly.LoadFile(newAssembly);
		}

		#endregion
	}
}