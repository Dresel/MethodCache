namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;
	using Mono.Cecil.Rocks;

	public class References
	{
		public IAssemblyResolver AssemblyResolver { get; set; }

		public TypeDefinition CompilerGeneratedAttribute { get; set; }

		public MethodDefinition DebugWriteLineMethod { get; set; }

		public MethodReference DictionaryAddMethod { get; set; }

		public MethodReference DictionaryConstructor { get; set; }

		public GenericInstanceType DictionaryGenericInstanceType { get; set; }

		public TypeDefinition DictionaryInterface { get; set; }

		public TypeDefinition DictionaryType { get; set; }

		public ModuleDefinition ModuleDefinition { get; set; }

		public MethodDefinition StringFormatMethod { get; set; }

		public MethodDefinition SystemTypeGetTypeFromHandleMethod { get; set; }

		public TypeDefinition SystemTypeType { get; set; }

		public void LoadReferences()
		{
			List<TypeDefinition> coreTypes = new List<TypeDefinition>();

			AppendTypes("System.Runtime.Extensions", coreTypes);
			AppendTypes("System", coreTypes);
			AppendTypes("mscorlib", coreTypes);
			AppendTypes("System.Runtime", coreTypes);
			AppendTypes("System.Reflection", coreTypes);

			TypeDefinition debugType = GetDebugType(coreTypes);

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

			DictionaryInterface = GetDictionaryInterface(coreTypes);

			DictionaryType = GetDictionaryType(coreTypes);
			DictionaryGenericInstanceType = DictionaryType.MakeGenericInstanceType(ModuleDefinition.TypeSystem.String,
				ModuleDefinition.TypeSystem.Object);

			DictionaryConstructor =
				DictionaryGenericInstanceType.Resolve()
					.GetConstructors()
					.First(x => !x.Parameters.Any())
					.MakeHostInstanceGeneric(ModuleDefinition.TypeSystem.String, ModuleDefinition.TypeSystem.Object);

			DictionaryAddMethod =
				DictionaryGenericInstanceType.Resolve()
					.Methods.First(
						method =>
							method.Matches("Add", ModuleDefinition.TypeSystem.Void,
								new TypeReference[] { DictionaryType.GenericParameters[0], DictionaryType.GenericParameters[1] }))
					.MakeHostInstanceGeneric(ModuleDefinition.TypeSystem.String, ModuleDefinition.TypeSystem.Object);

			SystemTypeType = GetSystemTypeType(coreTypes);

			SystemTypeGetTypeFromHandleMethod =
				SystemTypeType.Resolve()
					.Methods.First(
						method =>
							method.Matches("GetTypeFromHandle", SystemTypeType,
								new TypeReference[] { GetSystemRuntimeTypeHandleType(coreTypes) }));
		}

		private void AppendTypes(string name, List<TypeDefinition> coreTypes)
		{
			AssemblyDefinition definition = AssemblyResolver.Resolve(name);
			if (definition != null)
			{
				coreTypes.AddRange(definition.MainModule.Types);
			}
		}

		private TypeDefinition GetDebugType(List<TypeDefinition> coreTypes)
		{
			TypeDefinition debugType = coreTypes.FirstOrDefault(x => x.Name == "Debug");

			if (debugType != null)
			{
				return debugType;
			}

			AssemblyDefinition systemDiagnosticsDebug = AssemblyResolver.Resolve("System.Diagnostics.Debug");

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

		private TypeDefinition GetDictionaryInterface(List<TypeDefinition> coreTypes)
		{
			TypeDefinition dictionaryType = coreTypes.FirstOrDefault(x => x.Name == "IDictionary`2");

			if (dictionaryType != null)
			{
				return dictionaryType;
			}

			throw new Exception("Could not find the 'IDictionary' interface.");
		}

		private TypeDefinition GetDictionaryType(List<TypeDefinition> coreTypes)
		{
			TypeDefinition dictionaryType = coreTypes.FirstOrDefault(x => x.Name == "Dictionary`2");

			if (dictionaryType != null)
			{
				return dictionaryType;
			}

			throw new Exception("Could not find the 'Dictionary' type.");
		}

		private TypeDefinition GetSystemRuntimeTypeHandleType(List<TypeDefinition> coreTypes)
		{
			TypeDefinition runtimeTypeHandle = coreTypes.FirstOrDefault(x => x.Name == "RuntimeTypeHandle");

			if (runtimeTypeHandle != null)
			{
				return runtimeTypeHandle;
			}

			throw new Exception("Could not find the 'RuntimeHandle' type.");
		}

		private TypeDefinition GetSystemTypeType(List<TypeDefinition> coreTypes)
		{
			TypeDefinition systemType = coreTypes.FirstOrDefault(x => x.Name == "Type");

			if (systemType != null)
			{
				return systemType;
			}

			throw new Exception("Could not find the 'SystemType' type.");
		}
	}
}