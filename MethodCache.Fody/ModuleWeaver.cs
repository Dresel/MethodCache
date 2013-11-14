namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Xml.Linq;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Rocks;

	public class ModuleWeaver
	{
		public const string CacheAttributeName = "CacheAttribute";

		public const string CacheGetterName = "Cache";

		public const string CacheTypeContainsMethodName = "Contains";

		public const string CacheTypeRetrieveMethodName = "Retrieve";

		public const string CacheTypeStoreMethodName = "Store";

		public const string NoCacheAttributeName = "NoCacheAttribute";

		public IAssemblyResolver AssemblyResolver { get; set; }

		public XElement Config { get; set; }

		public List<string> DefineConstants { get; set; }

		public Action<string> LogError { get; set; }

		public Action<string, SequencePoint> LogErrorPoint { get; set; }

		public Action<string> LogInfo { get; set; }

		public Action<string> LogWarning { get; set; }

		public Action<string, SequencePoint> LogWarningPoint { get; set; }

		public ModuleDefinition ModuleDefinition { get; set; }

		private bool LogDebugOutput { get; set; }

		public ModuleWeaver()
		{
			LogInfo = m => { };
			LogWarning = m => { };
			LogWarningPoint = (m, p) => { };
			LogError = m => { };
			LogErrorPoint = (m, p) => { };

			Config = new XElement("MethodCache");
			DefineConstants = new List<string>();
		}

		public void Execute()
		{
			LogDebugOutput = DefineConstants.Any(x => x.ToLower() == "debug");

			if (Config.Attributes().Any(x => x.Name.ToString().ToLower() == "skipoutput" && x.Value.ToLower() == "true"))
			{
				LogWarning("Skipping Debug output.");
				LogDebugOutput = false;
			}

			WeaveMethods();

			RemoveReference();
		}

		private MethodDefinition CacheTypeGetContainsMethod(TypeDefinition cacheType, string cacheTypeContainsMethodName)
		{
			return
				(cacheType.GetMethod(cacheTypeContainsMethodName, cacheType.Module.ImportType<bool>(),
					new[] { cacheType.Module.ImportType<string>() }));
		}

		private MethodDefinition CacheTypeGetRetrieveMethod(TypeDefinition cacheType, string cacheTypeRetrieveMethodName)
		{
			return
				(cacheType.GetMethod(cacheTypeRetrieveMethodName, new GenericParameter("T", cacheType),
					new[] { cacheType.Module.ImportType<string>() }));
		}

		private MethodDefinition CacheTypeGetStoreMethod(TypeDefinition cacheInterface, string cacheTypeStoreMethodName)
		{
			return
				(cacheInterface.GetMethod(cacheTypeStoreMethodName, cacheInterface.Module.ImportType(typeof(void)),
					new[] { cacheInterface.Module.ImportType<string>(), cacheInterface.Module.ImportType<object>() }));
		}

		private bool CheckCacheTypeMethods(TypeDefinition cacheType)
		{
			LogInfo(string.Format("Checking CacheType methods ({0}, {1}, {2}).", CacheTypeContainsMethodName,
				CacheTypeStoreMethodName, CacheTypeRetrieveMethodName));

			if ((CacheTypeGetContainsMethod(cacheType, CacheTypeContainsMethodName)) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeContainsMethodName, cacheType.FullName));

				return false;
			}

			if ((CacheTypeGetStoreMethod(cacheType, CacheTypeStoreMethodName)) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeStoreMethodName, cacheType.FullName));

				return false;
			}

			if ((CacheTypeGetRetrieveMethod(cacheType, CacheTypeRetrieveMethodName)) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeRetrieveMethodName, cacheType.FullName));

				return false;
			}

			LogInfo(string.Format("CacheInterface methods found."));

			return true;
		}

		private void RemoveReference()
		{
			AssemblyNameReference referenceToRemove =
				ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "MethodCache.Attributes");

			if (referenceToRemove == null)
			{
				LogInfo("No reference to 'MethodCache.Attributes.dll' found. References not modified.");

				return;
			}

			ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);

			LogInfo("Removing reference to 'MethodCache.Attributes.dll'.");
		}

		private IEnumerable<MethodDefinition> SelectMethods(ModuleDefinition moduleDefinition, string cacheAttributeName,
			string noCacheAttributeName)
		{
			LogInfo(string.Format("Searching for Methods in assembly ({0}).", moduleDefinition.Name));

			HashSet<MethodDefinition> definitions = new HashSet<MethodDefinition>();

			definitions.UnionWith(
				moduleDefinition.Types.SelectMany(x => x.Methods.Where(y => y.ContainsAttribute(cacheAttributeName))));
			definitions.UnionWith(
				moduleDefinition.Types.Where(x => x.IsClass && x.ContainsAttribute(cacheAttributeName))
					.SelectMany(x => x.Methods)
					.Where(
						x =>
							!x.ContainsAttribute(noCacheAttributeName) &&
								(!x.IsSpecialName && !x.IsGetter && !x.IsSetter && !x.IsConstructor &&
									!x.ContainsAttribute(moduleDefinition.ImportType<CompilerGeneratedAttribute>()))));

			// Remove CacheAttributes and NoCacheAttributes
			definitions.ToList().ForEach(x => x.RemoveAttribute(cacheAttributeName));
			definitions.ToList().ForEach(x => x.DeclaringType.RemoveAttribute(cacheAttributeName));

			moduleDefinition.Types.SelectMany(x => x.Methods).ToList().ForEach(x => x.RemoveAttribute(NoCacheAttributeName));

			return definitions;
		}

		private void WeaveMethod(MethodDefinition methodDefinition)
		{
			methodDefinition.Body.InitLocals = true;

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

			Instruction current = firstInstruction.Prepend(processor.Create(OpCodes.Ldstr, builder.ToString()), processor);

			// Create object[] for string.format
			current =
				current.AppendLdcI4(processor, methodDefinition.Parameters.Count)
					.Append(processor.Create(OpCodes.Newarr, methodDefinition.Module.ImportType<object>()), processor)
					.AppendStloc(processor, objectArrayIndex);

			// Set object[] values
			for (int i = 0; i < methodDefinition.Parameters.Count; i++)
			{
				current =
					current.AppendLdloc(processor, objectArrayIndex)
						.AppendLdcI4(processor, i)
						.AppendLdarg(processor, (!methodDefinition.IsStatic ? i + 1 : i))
						.AppendBoxIfNecessary(processor, methodDefinition.Parameters[i].ParameterType)
						.Append(processor.Create(OpCodes.Stelem_Ref), processor);
			}

			// Call string.format
			current =
				current.AppendLdloc(processor, objectArrayIndex)
					.Append(
						processor.Create(OpCodes.Call,
							methodDefinition.Module.ImportMethod<string>("Format", new[] { typeof(string), typeof(object[]) })), processor)
					.AppendStloc(processor, cacheKeyIndex);

			if (LogDebugOutput)
			{
				// Call Debug.WriteLine with CacheKey
				current =
					current.AppendLdstr(processor, "CacheKey created: {0}")
						.AppendLdloc(processor, cacheKeyIndex)
						.Append(
							processor.Create(OpCodes.Call,
								methodDefinition.Module.ImportMethod<string>("Format", new[] { typeof(string), typeof(object) })), processor)
						.Append(
							processor.Create(OpCodes.Call,
								methodDefinition.Module.ImportMethod(typeof(Debug), "WriteLine", new[] { typeof(string) })), processor);
			}

			// Cache Getter
			MethodDefinition propertyGet = methodDefinition.DeclaringType.GetPropertyGet(CacheGetterName);
			propertyGet = propertyGet ??
				methodDefinition.DeclaringType.BaseType.Resolve().GetInheritedPropertyGet(CacheGetterName);

			TypeDefinition propertyGetReturnTypeDefinition = propertyGet.ReturnType.Resolve();

			if (!methodDefinition.IsStatic)
			{
				current = current.AppendLdarg(processor, 0);
			}

			current =
				current.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
					.AppendLdloc(processor, cacheKeyIndex)
					.Append(
						processor.Create(OpCodes.Callvirt,
							methodDefinition.Module.Import(CacheTypeGetContainsMethod(propertyGetReturnTypeDefinition,
								CacheTypeContainsMethodName))), processor)
					.Append(processor.Create(OpCodes.Brfalse, firstInstruction), processor);

			// False branche (store value in cache of each return instruction)
			foreach (Instruction returnInstruction in returnInstructions)
			{
				returnInstruction.Previous.AppendStloc(processor, resultIndex);

				if (LogDebugOutput)
				{
					returnInstruction.Previous.AppendDebugWrite(processor, "Storing to cache.", methodDefinition.Module);
				}

				if (!methodDefinition.IsStatic)
				{
					returnInstruction.Previous.AppendLdarg(processor, 0);
				}

				returnInstruction.Previous.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)),
					processor)
					.AppendLdloc(processor, cacheKeyIndex)
					.AppendLdloc(processor, resultIndex)
					.AppendBoxIfNecessary(processor, methodDefinition.ReturnType)
					.Append(
						processor.Create(OpCodes.Callvirt,
							methodDefinition.Module.Import(CacheTypeGetStoreMethod(propertyGetReturnTypeDefinition, CacheTypeStoreMethodName))),
						processor)
					.AppendLdloc(processor, resultIndex);
			}

			if (LogDebugOutput)
			{
				current = current.AppendDebugWrite(processor, "Loading from cache.", methodDefinition.Module);
			}

			if (!methodDefinition.IsStatic)
			{
				current = current.AppendLdarg(processor, 0);
			}

			// Start of branche true
			current.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
				.AppendLdloc(processor, cacheKeyIndex)
				.Append(
					processor.Create(OpCodes.Callvirt,
						methodDefinition.Module.Import(CacheTypeGetRetrieveMethod(propertyGetReturnTypeDefinition,
							CacheTypeRetrieveMethodName)).MakeGeneric(new[] { methodDefinition.ReturnType })), processor)
				.AppendStloc(processor, resultIndex)
				.Append(processor.Create(OpCodes.Br, returnInstructions.Last().Previous), processor);

			methodDefinition.Body.OptimizeMacros();
		}

		private void WeaveMethods()
		{
			IEnumerable<MethodDefinition> methodDefinitions = SelectMethods(ModuleDefinition, CacheAttributeName,
				NoCacheAttributeName);

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

				if (methodDefinition.IsStatic && !propertyGet.IsStatic)
				{
					LogWarning(string.Format("Method {2} of Class {0} is static, Getter {1} is not. Skip weaving of method {2}.",
						methodDefinition.DeclaringType.Name, CacheGetterName, methodDefinition.Name));

					continue;
				}

				if (!CheckCacheTypeMethods(propertyGet.ReturnType.Resolve()))
				{
					LogWarning(
						string.Format(
							"ReturnType {0} of Getter {1} of Class {2} does not implement all methods. Skip weaving of method {3}.",
							propertyGet.ReturnType.Name, CacheGetterName, methodDefinition.DeclaringType.Name, methodDefinition.Name));

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
	}
}