namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Rocks;

	public class ModuleWeaver
	{
		#region Constants

		private const string CacheAttributeName = "CacheAttribute";

		private const string CacheGetterName = "Cache";

		private const string CacheInterfaceContainsMethodName = "Contains";

		private const string CacheInterfaceName = "ICache";

		private const string CacheInterfaceRetrieveMethodName = "Retrieve";

		private const string CacheInterfaceStoreMethodName = "Store";

		#endregion

		#region Constructors and Destructors

		public ModuleWeaver()
		{
			LogInfo = m => { };
			LogWarning = m => { };
			LogWarningPoint = (m, p) => { };
			LogError = m => { };
			LogErrorPoint = (m, p) => { };

			DefineConstants = new List<string>();
		}

		#endregion

		#region Public Properties

		public IAssemblyResolver AssemblyResolver { get; set; }

		public List<string> DefineConstants { get; set; }

		public Action<string> LogError { get; set; }

		public Action<string, SequencePoint> LogErrorPoint { get; set; }

		public Action<string> LogInfo { get; set; }

		public Action<string> LogWarning { get; set; }

		public Action<string, SequencePoint> LogWarningPoint { get; set; }

		public ModuleDefinition ModuleDefinition { get; set; }

		#endregion

		#region Properties

		private TypeDefinition CacheAttribute { get; set; }

		private TypeDefinition CacheInterface { get; set; }

		private MethodDefinition CacheInterfaceContainsMethod { get; set; }

		private MethodDefinition CacheInterfaceRetrieveMethod { get; set; }

		private MethodDefinition CacheInterfaceStoreMethod { get; set; }

		private bool IsDebugBuild { get; set; }

		#endregion

		#region Public Methods and Operators

		public void Execute()
		{
			IsDebugBuild = DefineConstants.Any(x => x.ToLower() == "debug");

			// Check if CacheAttribute is existing
			if ((CacheAttribute = FindCacheAttribute(ModuleDefinition, CacheAttributeName)) == null)
			{
				return;
			}

			// Check if CacheInterface is existing
			if ((CacheInterface = FindCacheInterface(ModuleDefinition, CacheInterfaceName)) == null)
			{
				return;
			}

			// Check if CacheInterface methods are existing
			if (!CheckCacheInterfaceMethods())
			{
				return;
			}

			WeaveMethods();

			RemoveReference();
		}

		public void RemoveReference()
		{
			AssemblyNameReference referenceToRemove =
				ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "MethodCache");
			if (referenceToRemove == null)
			{
				LogInfo("\tNo reference to 'MethodCache.dll' found. References not modified.");
				return;
			}

			ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
			LogInfo("\tRemoving reference to 'MethodCache.dll'.");
		}

		#endregion

		#region Methods

		private MethodDefinition CacheInterfaceGetContainsMethod(TypeDefinition cacheInterface,
			string cacheInterfaceContainsMethodName)
		{
			return
				(cacheInterface.GetMethod(cacheInterfaceContainsMethodName, cacheInterface.Module.ImportType<bool>(),
					new[] { cacheInterface.Module.ImportType<string>() }));
		}

		private MethodDefinition CacheInterfaceGetRetrieveMethod(TypeDefinition cacheInterface,
			string cacheInterfaceRetrieveMethodName)
		{
			return
				(cacheInterface.GetMethod(cacheInterfaceRetrieveMethodName, new GenericParameter("T", cacheInterface),
					new[] { cacheInterface.Module.ImportType<string>() }));
		}

		private MethodDefinition CacheInterfaceGetStoreMethod(TypeDefinition cacheInterface,
			string cacheInterfaceStoreMethodName)
		{
			return
				(cacheInterface.GetMethod(cacheInterfaceStoreMethodName, cacheInterface.Module.ImportType(typeof(void)),
					new[] { cacheInterface.Module.ImportType<string>(), cacheInterface.Module.ImportType<object>() }));
		}

		private bool CheckCacheInterfaceMethods()
		{
			LogInfo(string.Format("Checking CacheInterface methods ({0}, {1}, {2}).", CacheInterfaceContainsMethodName,
				CacheInterfaceStoreMethodName, CacheInterfaceRetrieveMethodName));

			if (
				(CacheInterfaceContainsMethod = CacheInterfaceGetContainsMethod(CacheInterface, CacheInterfaceContainsMethodName)) ==
					null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheInterfaceContainsMethodName, CacheInterface.FullName));

				return false;
			}

			if ((CacheInterfaceStoreMethod = CacheInterfaceGetStoreMethod(CacheInterface, CacheInterfaceStoreMethodName)) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheInterfaceStoreMethodName, CacheInterface.FullName));

				return false;
			}

			if (
				(CacheInterfaceRetrieveMethod = CacheInterfaceGetRetrieveMethod(CacheInterface, CacheInterfaceRetrieveMethodName)) ==
					null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheInterfaceRetrieveMethodName, CacheInterface.FullName));

				return false;
			}

			LogInfo(string.Format("CacheInterface methods found."));

			return true;
		}

		private TypeDefinition FindCacheAttribute(ModuleDefinition moduleDefinition, string cacheAttributeName)
		{
			LogInfo(string.Format("Searching for CacheAttribute ({0}) in assembly ({1}).", cacheAttributeName,
				moduleDefinition.Name));

			TypeDefinition cacheAttribute = moduleDefinition.Types.FirstOrDefault(x => x.Name == cacheAttributeName && x.IsClass);

			if (cacheAttribute != null)
			{
				LogInfo(string.Format("{0} found ({1}).", cacheAttributeName, cacheAttribute.FullName));

				return cacheAttribute;
			}

			LogInfo(string.Format("{0} not found in assembly ({1}). Resolving assembly references.", cacheAttributeName,
				moduleDefinition.Name));

			foreach (AssemblyNameReference reference in moduleDefinition.AssemblyReferences)
			{
				ModuleDefinition mainModule = moduleDefinition.AssemblyResolver.Resolve(reference).MainModule;

				cacheAttribute = mainModule.Types.FirstOrDefault(x => x.Name == cacheAttributeName && x.IsClass);

				if (cacheAttribute != null)
				{
					LogInfo(string.Format("{0} found ({1}).", cacheAttributeName, cacheAttribute.FullName));

					return cacheAttribute;
				}
			}

			LogWarning(string.Format("{0} not found in assembly ({1}) and assembly references.", cacheAttributeName,
				moduleDefinition.Name));

			return null;
		}

		private TypeDefinition FindCacheInterface(ModuleDefinition moduleDefinition, string cacheInterfaceName)
		{
			LogInfo(string.Format("Searching for CacheInterface ({0}) in assembly ({1}).", cacheInterfaceName,
				moduleDefinition.Name));

			TypeDefinition cacheInterface =
				moduleDefinition.Types.FirstOrDefault(x => x.Name == cacheInterfaceName && x.IsInterface);

			if (cacheInterface != null)
			{
				LogInfo(string.Format("{0} found ({1}).", cacheInterfaceName, cacheInterface.FullName));

				return cacheInterface;
			}

			LogInfo(string.Format("{0} not found in assembly ({1}). Resolving assembly references.", cacheInterfaceName,
				moduleDefinition.Name));

			foreach (AssemblyNameReference reference in moduleDefinition.AssemblyReferences)
			{
				ModuleDefinition mainModule = moduleDefinition.AssemblyResolver.Resolve(reference).MainModule;

				cacheInterface = mainModule.Types.FirstOrDefault(x => x.Name == cacheInterfaceName && x.IsInterface);

				if (cacheInterface != null)
				{
					LogInfo(string.Format("{0} found ({1}).", cacheInterfaceName, cacheInterface.FullName));

					return cacheInterface;
				}
			}

			LogWarning(string.Format("{0} not found in assembly ({1}) and assembly references.", cacheInterfaceName,
				moduleDefinition.Name));

			return null;
		}

		private IEnumerable<MethodDefinition> SelectMethods(ModuleDefinition moduleDefinition, TypeDefinition cacheAttribute)
		{
			LogInfo(string.Format("Searching for Methods in assembly ({0}).", moduleDefinition.Name));

			HashSet<MethodDefinition> definitions = new HashSet<MethodDefinition>();

			definitions.UnionWith(
				moduleDefinition.Types.SelectMany(x => x.Methods.Where(y => y.ContainsAttribute(cacheAttribute))));
			definitions.UnionWith(
				moduleDefinition.Types.Where(x => x.IsClass && x.ContainsAttribute(cacheAttribute))
				                .SelectMany(x => x.Methods)
				                .Where(
					                x =>
						                !x.IsSpecialName && !x.IsGetter && !x.IsSetter && !x.IsConstructor &&
							                !x.ContainsAttribute(moduleDefinition.ImportType<CompilerGeneratedAttribute>())));

			return definitions;
		}

		private void WeaveMethod(MethodDefinition methodDefinition)
		{
			methodDefinition.Body.SimplifyMacros();

			Instruction firstInstruction = methodDefinition.Body.Instructions.First();

			ICollection<Instruction> returnInstructions =
				methodDefinition.Body.Instructions.ToList().Where(x => x.OpCode == OpCodes.Ret).ToList();

			if (returnInstructions.Count == 0)
			{
				LogWarning(string.Format("Method {0} does not contain any return statement. Skip weaving of method {0}.",
					methodDefinition.Name));
				return;
			}

			// Add local variables
			int cacheKeyIndex = methodDefinition.AddVariable<string>();
			int resultIndex = methodDefinition.AddVariable(methodDefinition.ReturnType);
			int objectArrayIndex = methodDefinition.AddVariable<object[]>();

			ILProcessor processor = methodDefinition.Body.GetILProcessor();

			// Generate CacheKeyTemplate
			StringBuilder builder = new StringBuilder();

			builder.Append(methodDefinition.DeclaringType.FullName);
			builder.Append(".");
			builder.Append(methodDefinition.Name);

			for (int i = 0; i < methodDefinition.Parameters.Count; i++)
			{
				builder.Append(string.Format("_{{{0}}}", i));
			}

			Instruction current = firstInstruction
				.Prepend(processor.Create(OpCodes.Ldstr, builder.ToString()), processor);

			// Create object[] for string.format
			current = current
				.AppendLdcI4(processor, methodDefinition.Parameters.Count)
				.Append(processor.Create(OpCodes.Newarr, methodDefinition.Module.ImportType<object>()), processor)
				.AppendStloc(processor, objectArrayIndex);

			// Set object[] values
			for (int i = 0; i < methodDefinition.Parameters.Count; i++)
			{
				current = current
					.AppendLdloc(processor, objectArrayIndex).AppendLdcI4(processor, i)
					.AppendLdarg(processor, i + 1)
					.AppendBoxIfNecessary(processor, methodDefinition.Parameters[i].ParameterType)
					.Append(processor.Create(OpCodes.Stelem_Ref), processor);
			}

			// Call string.format
			current = current
				.AppendLdloc(processor, objectArrayIndex)
				.Append(processor.Create(OpCodes.Call, methodDefinition.Module.ImportMethod<string>("Format", new[] { typeof(string), typeof(object[]) })), processor)
				.AppendStloc(processor, cacheKeyIndex);

			if (IsDebugBuild)
			{
				// Call Debug.WriteLine with CacheKey
				current = current
					.AppendLdstr(processor, "CacheKey created: {0}")
					.AppendLdloc(processor, cacheKeyIndex)
					.Append(processor.Create(OpCodes.Call, methodDefinition.Module.ImportMethod<string>("Format", new[] { typeof(string), typeof(object) })), processor)
					.Append(processor.Create(OpCodes.Call,
						methodDefinition.Module.ImportMethod(typeof(Debug), "WriteLine", new[] { typeof(string) })), processor);
			}

			// Cache Getter
			MethodDefinition propertyGet = methodDefinition.DeclaringType.GetPropertyGet(CacheGetterName);
			propertyGet = propertyGet ?? methodDefinition.DeclaringType.BaseType.Resolve().GetInheritedPropertyGet(CacheGetterName);

			current = current.Append(processor.Create(OpCodes.Ldarg_0), processor)
				.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
				.AppendLdloc(processor, cacheKeyIndex)
				.Append(processor.Create(OpCodes.Callvirt, methodDefinition.Module.Import(CacheInterfaceContainsMethod)), processor)
				.Append(processor.Create(OpCodes.Brfalse, firstInstruction), processor);

			// False branche (store value in cache of each return instruction)
			foreach (Instruction returnInstruction in returnInstructions)
			{
				returnInstruction.Previous
					.AppendStloc(processor, resultIndex);

				if (IsDebugBuild)
				{
					returnInstruction.Previous
						.AppendDebugWrite(processor, "Storing to cache.", methodDefinition.Module);
				}

				returnInstruction.Previous
					.Append(processor.Create(OpCodes.Ldarg_0), processor)
					.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
					.AppendLdloc(processor, cacheKeyIndex)
					.AppendLdloc(processor, resultIndex)
					.AppendBoxIfNecessary(processor, methodDefinition.ReturnType)
					.Append(processor.Create(OpCodes.Callvirt, methodDefinition.Module.Import(CacheInterfaceStoreMethod)), processor)
					.AppendLdloc(processor, resultIndex);
			}

			if (IsDebugBuild)
			{
				current = current
					.AppendDebugWrite(processor, "Loading from cache.", methodDefinition.Module);
			}

			// Start of branche true
			current = current
				.Append(processor.Create(OpCodes.Ldarg_0), processor)
				.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
				.AppendLdloc(processor, cacheKeyIndex)
				.Append(processor.Create(OpCodes.Callvirt, methodDefinition.Module.Import(CacheInterfaceRetrieveMethod).MakeGeneric(new[] { methodDefinition.ReturnType })), processor)
				.AppendStloc(processor, resultIndex).Append(processor.Create(OpCodes.Br, returnInstructions.Last().Previous), processor);

			methodDefinition.Body.OptimizeMacros();
		}

		private void WeaveMethods()
		{
			IEnumerable<MethodDefinition> methodDefinitions = SelectMethods(ModuleDefinition, CacheAttribute);

			foreach (MethodDefinition methodDefinition in methodDefinitions)
			{
				MethodDefinition propertyGet = methodDefinition.DeclaringType.GetPropertyGet(CacheGetterName);
				propertyGet = propertyGet ??
					methodDefinition.DeclaringType.BaseType.Resolve().GetInheritedPropertyGet(CacheGetterName);

				LogInfo(string.Format("Weaving method {0}::{1}.", methodDefinition.DeclaringType.Name, methodDefinition.Name));

				if (propertyGet == null)
				{
					LogWarning(string.Format("Class {0} does not contain or inherit Getter {1}. Skip weaving of method {2}.",
						methodDefinition.DeclaringType.Name, CacheGetterName, methodDefinition.Name));

					continue;
				}

				if (propertyGet.ReturnType.FullName != CacheInterface.FullName)
				{
					LogWarning(string.Format("Getter {0} of Class {1} is not of type {2}. Skip weaving of method {3}.", CacheGetterName,
						methodDefinition.DeclaringType.Name, CacheInterface.Name, methodDefinition.Name));

					continue;
				}

				if (methodDefinition.ReturnType.FullName == methodDefinition.Module.ImportType(typeof(void)).FullName)
				{
					LogWarning(string.Format("Method {0} returns void. Skip weaving of method {0}.", methodDefinition.Name));

					continue;
				}

				WeaveMethod(methodDefinition);
			}
		}

		#endregion
	}
}