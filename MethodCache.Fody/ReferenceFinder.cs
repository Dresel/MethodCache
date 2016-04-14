namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;
	using Mono.Cecil.Rocks;

	public class References
	{
		public ModuleDefinition ModuleDefinition { get; set; }

		public IAssemblyResolver AssemblyResolver { get; set; }

		public MethodDefinition DebugWriteLineMethod { get; set; }

		public MethodDefinition StringFormatMethod { get; set; }

		public TypeDefinition CompilerGeneratedAttribute { get; set; }

		public void LoadReferences()
		{
			var coreTypes = new List<TypeDefinition>();

			AppendTypes("System.Runtime.Extensions", coreTypes);
			AppendTypes("System", coreTypes);
			AppendTypes("mscorlib", coreTypes);
			AppendTypes("System.Runtime", coreTypes);
			AppendTypes("System.Reflection", coreTypes);

			var debugType = GetDebugType(coreTypes);

			DebugWriteLineMethod =
				debugType.Methods.First(
					method =>
						method.Matches("WriteLine", ModuleDefinition.TypeSystem.Void, new[] { ModuleDefinition.TypeSystem.String }));

			StringFormatMethod =
				ModuleDefinition.TypeSystem.String.Resolve()
					.Methods.First(
						method =>
							method.Matches("Format", ModuleDefinition.TypeSystem.String,
								new[] { ModuleDefinition.TypeSystem.String, ModuleDefinition.TypeSystem.Object.MakeArrayType() }));

			CompilerGeneratedAttribute = coreTypes.First(t => t.Name == "CompilerGeneratedAttribute");
		}

		private void AppendTypes(string name, List<TypeDefinition> coreTypes)
		{
			var definition = AssemblyResolver.Resolve(name);
			if (definition != null)
			{
				coreTypes.AddRange(definition.MainModule.Types);
			}
		}

		private TypeDefinition GetDebugType(List<TypeDefinition> coreTypes)
		{
			var debugType = coreTypes.FirstOrDefault(x => x.Name == "Debug");

			if (debugType != null)
			{
				return debugType;
			}

			var systemDiagnosticsDebug = AssemblyResolver.Resolve("System.Diagnostics.Debug");

			if (systemDiagnosticsDebug != null)
			{
				debugType = systemDiagnosticsDebug.MainModule.Types.FirstOrDefault(x => x.Name == "Debug");

				if (debugType != null)
				{
					return debugType;
				}
			}

			throw new Exception("Could not find the 'Debug' type.");
		}
	}
}