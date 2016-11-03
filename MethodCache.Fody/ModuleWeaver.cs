namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;
	using MethodCache.Attributes;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Rocks;

	public class ModuleWeaver
	{
		public const string CacheAttributeName = "CacheAttribute";

		public const string CacheGetterName = "Cache";

		public const string CacheTypeContainsMethodName = "Contains";

		public const string CacheTypeRemoveMethodName = "Remove";

		public const string CacheTypeRetrieveMethodName = "Retrieve";

		public const string CacheTypeStoreMethodName = "Store";

		public const string NoCacheAttributeName = "NoCacheAttribute";

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

		public IAssemblyResolver AssemblyResolver { get; set; }

		public XElement Config { get; set; }

		public List<string> DefineConstants { get; set; }

		public Action<string> LogError { get; set; }

		public Action<string, SequencePoint> LogErrorPoint { get; set; }

		public Action<string> LogInfo { get; set; }

		public Action<string> LogWarning { get; set; }

		public Action<string, SequencePoint> LogWarningPoint { get; set; }

		public ModuleDefinition ModuleDefinition { get; set; }

		public References References { get; set; }

		private bool LogDebugOutput { get; set; }

		private bool MethodCacheEnabledByDefault { get; set; }

		private bool PropertyCacheEnabledByDefault { get; set; }

		public void Execute()
		{
			LogDebugOutput = DefineConstants.Any(x => x.ToLower() == "debug");

			References = new References() { AssemblyResolver = AssemblyResolver, ModuleDefinition = ModuleDefinition };
			References.LoadReferences();

			MethodCacheEnabledByDefault = true;
			PropertyCacheEnabledByDefault = true;

			ReadConfiguration();

			MethodsForWeaving methodToWeave = SelectMethods();

			WeaveMethods(methodToWeave.Methods);
			WeaveProperties(methodToWeave.Properties);

			RemoveReference();
		}

		private static string CreateCacheKeyString(MethodDefinition methodDefinition)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(methodDefinition.DeclaringType.FullName);
			builder.Append(".");

			if (methodDefinition.IsSpecialName && (methodDefinition.IsSetter || methodDefinition.IsGetter))
			{
				builder.Append(Regex.Replace(methodDefinition.Name, "[gs]et_", string.Empty));
			}
			else
			{
				builder.Append(methodDefinition.Name);

				for (int i = 0; i < methodDefinition.Parameters.Count + methodDefinition.GenericParameters.Count; i++)
				{
					builder.Append(string.Format("_{{{0}}}", i));
				}
			}

			return builder.ToString();
		}

		private static MethodDefinition GetCacheGetter(MethodDefinition methodDefinition)
		{
			MethodDefinition propertyGet = methodDefinition.DeclaringType.GetPropertyGet(CacheGetterName);

			propertyGet = propertyGet ??
				methodDefinition.DeclaringType.BaseType.Resolve().GetInheritedPropertyGet(CacheGetterName);

			return propertyGet;
		}

		private Instruction AppendDebugWrite(Instruction instruction, ILProcessor processor, string message)
		{
			return instruction
				.AppendLdstr(processor, message)
				.Append(processor.Create(OpCodes.Call, ModuleDefinition.ImportMethod(References.DebugWriteLineMethod)), processor);
		}

		private bool CacheAttributeConstructedWithParam(CustomAttribute attribute, Members cachedMembers)
		{
			if (attribute == null || attribute.ConstructorArguments.Count == 0)
			{
				return false;
			}

			return Equals(attribute.ConstructorArguments.Single().Value, (int)cachedMembers);
		}

		private bool CacheAttributeExcludesMethods(CustomAttribute attribute)
		{
			return CacheAttributeConstructedWithParam(attribute, Members.Properties);
		}

		private bool CacheAttributeExcludesProperties(CustomAttribute attribute)
		{
			return CacheAttributeConstructedWithParam(attribute, Members.Methods);
		}

		private bool CacheAttributeMembersExplicitly(CustomAttribute attribute, Members requiredFlag)
		{
			if (attribute == null || attribute.ConstructorArguments.Count == 0)
			{
				return false;
			}

			Members constructorArgument = (Members)attribute.ConstructorArguments.Single().Value;

			return constructorArgument.HasFlag(requiredFlag);
		}

		private MethodDefinition CacheTypeGetContainsMethod(TypeDefinition cacheType, string cacheTypeContainsMethodName)
		{
			return cacheType.GetMethod(cacheTypeContainsMethodName, ModuleDefinition.TypeSystem.Boolean,
				new[] { ModuleDefinition.TypeSystem.String });
		}

		private MethodDefinition CacheTypeGetRemoveMethod(TypeDefinition cacheType, string cacheTypeRemoveMethodName)
		{
			return cacheType.GetMethod(cacheTypeRemoveMethodName, ModuleDefinition.TypeSystem.Void,
				new[] { ModuleDefinition.TypeSystem.String });
		}

		private MethodDefinition CacheTypeGetRetrieveMethod(TypeDefinition cacheType, string cacheTypeRetrieveMethodName)
		{
			return cacheType.GetMethod(cacheTypeRetrieveMethodName, new GenericParameter("T", cacheType),
				new[] { ModuleDefinition.TypeSystem.String });
		}

		private MethodDefinition CacheTypeGetStoreMethod(TypeDefinition cacheInterface, string cacheTypeStoreMethodName)
		{
			// Prioritize Store methods with parameters Dictionary
			MethodDefinition methodDefinition = cacheInterface.GetMethod(cacheTypeStoreMethodName, ModuleDefinition.TypeSystem.Void,
				new[]
				{
					ModuleDefinition.TypeSystem.String, ModuleDefinition.TypeSystem.Object,
					References.DictionaryInterface.MakeGenericInstanceType(ModuleDefinition.TypeSystem.String,
						ModuleDefinition.TypeSystem.Object)
				});

			if (methodDefinition != null)
			{
				return methodDefinition;
			}

			return cacheInterface.GetMethod(cacheTypeStoreMethodName, ModuleDefinition.TypeSystem.Void,
				new[] { ModuleDefinition.TypeSystem.String, ModuleDefinition.TypeSystem.Object });
		}

		private bool CheckCacheTypeMethods(TypeDefinition cacheType)
		{
			LogInfo(string.Format("Checking CacheType methods ({0}, {1}, {2}).", CacheTypeContainsMethodName,
				CacheTypeStoreMethodName, CacheTypeRetrieveMethodName));

			if (CacheTypeGetContainsMethod(cacheType, CacheTypeContainsMethodName) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeContainsMethodName, cacheType.FullName));

				return false;
			}

			if (CacheTypeGetStoreMethod(cacheType, CacheTypeStoreMethodName) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeStoreMethodName, cacheType.FullName));

				return false;
			}

			if (CacheTypeGetRetrieveMethod(cacheType, CacheTypeRetrieveMethodName) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeRetrieveMethodName, cacheType.FullName));

				return false;
			}

			LogInfo(string.Format("CacheInterface methods found."));

			return true;
		}

		private bool ConfigHasAttribute(string name, string value)
		{
			return
				Config.Attributes().Any(x => x.Name.ToString().ToLower() == name.ToLower() && x.Value.ToLower() == value.ToLower());
		}

		private IDictionary<string, CustomAttributeArgument> GetCacheAttributeConstructorFields(CustomAttribute attribute)
		{
			return attribute.Fields.ToDictionary(field => field.Name, field => field.Argument);
		}

		private IDictionary<string, CustomAttributeArgument> GetCacheAttributeConstructorParameters(CustomAttribute attribute)
		{
			return attribute.Properties.ToDictionary(property => property.Name, property => property.Argument);
		}

		private Instruction InjectCacheKeyCreatedCode(MethodDefinition methodDefinition, Instruction current,
			ILProcessor processor, int cacheKeyIndex)
		{
			if (LogDebugOutput)
			{
				// Call Debug.WriteLine with CacheKey
				current = current
					.AppendLdstr(processor, "CacheKey created: {0}")
					.AppendLdcI4(processor, 1)
					.Append(processor.Create(OpCodes.Newarr, ModuleDefinition.TypeSystem.Object), processor)
					.AppendDup(processor)
					.AppendLdcI4(processor, 0)
					.AppendLdloc(processor, cacheKeyIndex)
					.Append(processor.Create(OpCodes.Stelem_Ref), processor)
					.Append(processor.Create(OpCodes.Call, methodDefinition.Module.ImportMethod(References.StringFormatMethod)),
						processor)
					.Append(processor.Create(OpCodes.Call, methodDefinition.Module.ImportMethod(References.DebugWriteLineMethod)),
						processor);
			}

			return current;
		}

		private bool IsMethodValidForWeaving(MethodDefinition propertyGet, MethodDefinition methodDefinition)
		{
			if (propertyGet == null)
			{
				LogWarning(string.Format("Class {0} does not contain or inherit Getter {1}. Skip weaving of method {2}.",
					methodDefinition.DeclaringType.Name, CacheGetterName, methodDefinition.Name));

				return false;
			}

			if (methodDefinition.IsStatic && !propertyGet.IsStatic)
			{
				LogWarning(string.Format("Method {2} of Class {0} is static, Getter {1} is not. Skip weaving of method {2}.",
					methodDefinition.DeclaringType.Name, CacheGetterName, methodDefinition.Name));

				return false;
			}

			if (!CheckCacheTypeMethods(propertyGet.ReturnType.Resolve()))
			{
				LogWarning(
					string.Format(
						"ReturnType {0} of Getter {1} of Class {2} does not implement all methods. Skip weaving of method {3}.",
						propertyGet.ReturnType.Name, CacheGetterName, methodDefinition.DeclaringType.Name, methodDefinition.Name));

				return false;
			}

			return true;
		}

		private bool IsPropertySetterValidForWeaving(MethodDefinition propertyGet, MethodDefinition methodDefinition)
		{
			if (!IsMethodValidForWeaving(propertyGet, methodDefinition))
			{
				return false;
			}

			if (CacheTypeGetRemoveMethod(propertyGet.ReturnType.Resolve(), CacheTypeRemoveMethodName) == null)
			{
				LogWarning(string.Format("Method {0} missing in {1}.", CacheTypeRemoveMethodName,
					propertyGet.ReturnType.Resolve().FullName));

				LogWarning(
					string.Format(
						"ReturnType {0} of Getter {1} of Class {2} does not implement all methods. Skip weaving of method {3}.",
						propertyGet.ReturnType.Name, CacheGetterName, methodDefinition.DeclaringType.Name, methodDefinition.Name));

				return false;
			}

			return true;
		}

		private void ReadConfiguration()
		{
			if (ConfigHasAttribute("SkipDebugOutput", "true"))
			{
				LogWarning("Skipping Debug Output.");
				LogDebugOutput = false;
			}

			if (ConfigHasAttribute("CacheProperties", "false"))
			{
				LogWarning("Disabling property weaving.");
				PropertyCacheEnabledByDefault = false;
			}

			if (ConfigHasAttribute("CacheMethods", "false"))
			{
				LogWarning("Disabling method weaving.");
				MethodCacheEnabledByDefault = false;
			}
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

		private MethodsForWeaving SelectMethods()
		{
			LogInfo(string.Format("Searching for Methods and Properties in assembly ({0}).", ModuleDefinition.Name));

			MethodsForWeaving result = new MethodsForWeaving();

			foreach (TypeDefinition type in ModuleDefinition.Types)
			{
				foreach (MethodDefinition method in type.Methods)
				{
					if (ShouldWeaveMethod(method))
					{
						// Store Cache attribute, method attribute takes precedence over class attributes
						CustomAttribute attribute =
							method.CustomAttributes.SingleOrDefault(x => x.Constructor.DeclaringType.Name == CacheAttributeName) ??
								method.DeclaringType.CustomAttributes.SingleOrDefault(
									x => x.Constructor.DeclaringType.Name == CacheAttributeName);

						result.Add(method, attribute);
					}

					method.RemoveAttribute(CacheAttributeName);
					method.RemoveAttribute(NoCacheAttributeName);
				}

				foreach (PropertyDefinition property in type.Properties)
				{
					if (ShouldWeaveProperty(property))
					{
						// Store Cache attribute, property attribute takes precedence over class attributes
						CustomAttribute attribute =
							property.CustomAttributes.SingleOrDefault(x => x.Constructor.DeclaringType.Name == CacheAttributeName) ??
								property.DeclaringType.CustomAttributes.SingleOrDefault(
									x => x.Constructor.DeclaringType.Name == CacheAttributeName);

						result.Add(property, attribute);
					}

					property.RemoveAttribute(CacheAttributeName);
					property.RemoveAttribute(NoCacheAttributeName);
				}

				type.RemoveAttribute(CacheAttributeName);
			}

			return result;
		}

		private Instruction SetCacheKeyLocalVariable(Instruction current, MethodDefinition methodDefinition,
			ILProcessor processor, int cacheKeyIndex, int objectArrayIndex)
		{
			if (methodDefinition.IsSetter || methodDefinition.IsGetter)
			{
				return current.AppendStloc(processor, cacheKeyIndex);
			}
			else
			{
				// Create object[] for string.format
				int parameterCount = methodDefinition.Parameters.Count + methodDefinition.GenericParameters.Count;

				current = current
					.AppendLdcI4(processor, parameterCount)
					.Append(processor.Create(OpCodes.Newarr, ModuleDefinition.TypeSystem.Object), processor)
					.AppendStloc(processor, objectArrayIndex);


				// Set object[] values
				for (int i = 0; i < methodDefinition.GenericParameters.Count; i++)
				{
					current = current
						.AppendLdloc(processor, objectArrayIndex)
						.AppendLdcI4(processor, i)
						.Append(processor.Create(OpCodes.Ldtoken, methodDefinition.GenericParameters[i]), processor)
						.Append(processor.Create(OpCodes.Call, methodDefinition.Module.ImportMethod(References.SystemTypeGetTypeFromHandleMethod)),
						processor)
						.Append(processor.Create(OpCodes.Stelem_Ref), processor);
				}

				for (int i = 0; i < methodDefinition.Parameters.Count; i++)
				{
					current = current
						.AppendLdloc(processor, objectArrayIndex)
						.AppendLdcI4(processor, methodDefinition.GenericParameters.Count + i)
						.AppendLdarg(processor, !methodDefinition.IsStatic ? i + 1 : i)
						.AppendBoxIfNecessary(processor, methodDefinition.Parameters[i].ParameterType)
						.Append(processor.Create(OpCodes.Stelem_Ref), processor);
				}

				// Call string.format
				return current
					.AppendLdloc(processor, objectArrayIndex)
					.Append(processor.Create(OpCodes.Call, methodDefinition.Module.ImportMethod(References.StringFormatMethod)),
						processor)
					.AppendStloc(processor, cacheKeyIndex);
			}
		}

		private bool ShouldWeaveMethod(MethodDefinition method)
		{
			CustomAttribute classLevelCacheAttribute =
				method.DeclaringType.CustomAttributes.SingleOrDefault(x => x.Constructor.DeclaringType.Name == CacheAttributeName);

			bool hasClassLevelCache = classLevelCacheAttribute != null &&
				!CacheAttributeExcludesMethods(classLevelCacheAttribute);
			bool hasMethodLevelCache = method.ContainsAttribute(CacheAttributeName);
			bool hasNoCacheAttribute = method.ContainsAttribute(NoCacheAttributeName);
			bool isSpecialName = method.IsSpecialName || method.IsGetter || method.IsSetter || method.IsConstructor;
			bool isCompilerGenerated = method.ContainsAttribute(References.CompilerGeneratedAttribute);

			if (hasNoCacheAttribute || isSpecialName || isCompilerGenerated)
			{
				// Never weave property accessors, special methods and compiler generated methods
				return false;
			}

			if (hasMethodLevelCache)
			{
				// Always weave methods explicitly marked for cache
				return true;
			}

			if (hasClassLevelCache && !CacheAttributeExcludesMethods(classLevelCacheAttribute))
			{
				// Otherwise weave if marked at class level
				return MethodCacheEnabledByDefault || CacheAttributeMembersExplicitly(classLevelCacheAttribute, Members.Methods);
			}

			return false;
		}

		private bool ShouldWeaveProperty(PropertyDefinition property)
		{
			CustomAttribute classLevelCacheAttribute =
				property.DeclaringType.CustomAttributes.SingleOrDefault(x => x.Constructor.DeclaringType.Name == CacheAttributeName);

			bool hasClassLevelCache = classLevelCacheAttribute != null;
			bool hasPropertyLevelCache = property.ContainsAttribute(CacheAttributeName);
			bool hasNoCacheAttribute = property.ContainsAttribute(NoCacheAttributeName);
			bool isCacheGetter = property.Name == CacheGetterName;
			bool hasGetAccessor = property.GetMethod != null;
			bool isAutoProperty = hasGetAccessor && property.GetMethod.ContainsAttribute(References.CompilerGeneratedAttribute);

			if (hasNoCacheAttribute || isCacheGetter || isAutoProperty || !hasGetAccessor)
			{
				// Never weave Cache property, auto-properties, write-only properties and properties explicitly excluded
				return false;
			}

			if (hasPropertyLevelCache)
			{
				// Always weave properties explicitly marked for cache
				return true;
			}

			if (hasClassLevelCache && !CacheAttributeExcludesProperties(classLevelCacheAttribute))
			{
				// Otherwise weave if marked at class level
				return PropertyCacheEnabledByDefault ||
					CacheAttributeMembersExplicitly(classLevelCacheAttribute, Members.Properties);
			}

			return false;
		}

		private void WeaveMethod(MethodDefinition methodDefinition, CustomAttribute attribute, MethodReference propertyGet)
		{
			methodDefinition.Body.InitLocals = true;

			methodDefinition.Body.SimplifyMacros();

			if (propertyGet.DeclaringType.HasGenericParameters)
			{
				propertyGet = propertyGet.MakeHostInstanceGeneric(propertyGet.DeclaringType.GenericParameters.Cast<TypeReference>().ToArray());
			}

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
			int cacheKeyIndex = methodDefinition.AddVariable(ModuleDefinition.TypeSystem.String);
			int resultIndex = methodDefinition.AddVariable(methodDefinition.ReturnType);
			int objectArrayIndex = methodDefinition.AddVariable(ModuleDefinition.TypeSystem.Object.MakeArrayType());

			ILProcessor processor = methodDefinition.Body.GetILProcessor();

			// Generate CacheKeyTemplate
			string cacheKey = CreateCacheKeyString(methodDefinition);

			Instruction current = firstInstruction.Prepend(processor.Create(OpCodes.Ldstr, cacheKey), processor);

			current = SetCacheKeyLocalVariable(current, methodDefinition, processor, cacheKeyIndex, objectArrayIndex);

			current = InjectCacheKeyCreatedCode(methodDefinition, current, processor, cacheKeyIndex);

			TypeDefinition propertyGetReturnTypeDefinition = propertyGet.ReturnType.Resolve();

			if (!propertyGet.Resolve().IsStatic)
			{
				current = current.AppendLdarg(processor, 0);
			}

			MethodReference methodReferenceContain =
				methodDefinition.Module.Import(CacheTypeGetContainsMethod(propertyGetReturnTypeDefinition,
					CacheTypeContainsMethodName));

			current = current
				.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
				.AppendLdloc(processor, cacheKeyIndex)
				.Append(processor.Create(OpCodes.Callvirt, methodReferenceContain), processor)
				.Append(processor.Create(OpCodes.Brfalse, firstInstruction), processor);

			// False branche (store value in cache of each return instruction)
			foreach (Instruction returnInstruction in returnInstructions)
			{
				returnInstruction.Previous.AppendStloc(processor, resultIndex);

				if (LogDebugOutput)
				{
					AppendDebugWrite(returnInstruction.Previous, processor, "Storing to cache.");
				}

				if (!propertyGet.Resolve().IsStatic)
				{
					returnInstruction.Previous.AppendLdarg(processor, 0);
				}

				MethodReference methodReferenceStore =
					methodDefinition.Module.Import(CacheTypeGetStoreMethod(propertyGetReturnTypeDefinition, CacheTypeStoreMethodName));

				returnInstruction.Previous
					.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
					.AppendLdloc(processor, cacheKeyIndex)
					.AppendLdloc(processor, resultIndex)
					.AppendBoxIfNecessary(processor, methodDefinition.ReturnType);

				// Pass parameters to Store method if supported
				if (methodReferenceStore.Parameters.Count == 3)
				{
					returnInstruction.Previous
						.Append(processor.Create(OpCodes.Newobj,
							methodDefinition.Module.Import(References.DictionaryConstructor)), processor);

					foreach (CustomAttributeNamedArgument property in attribute.Properties.Union(attribute.Fields))
					{
						returnInstruction.Previous
							.AppendDup(processor)
							.AppendLdstr(processor, property.Name)
							.AppendLd(processor, property.Argument, References)
							.AppendBoxIfNecessary(processor,
								property.Argument.Type != ModuleDefinition.TypeSystem.Object
									? property.Argument.Type : ((CustomAttributeArgument)property.Argument.Value).Type)
							.Append(processor.Create(OpCodes.Callvirt, methodDefinition.Module.Import(References.DictionaryAddMethod)),
								processor);
					}
				}

				returnInstruction.Previous
					.Append(processor.Create(OpCodes.Callvirt, methodReferenceStore), processor)
					.AppendLdloc(processor, resultIndex);
			}

			if (LogDebugOutput)
			{
				current = AppendDebugWrite(current, processor, "Loading from cache.");
			}

			if (!propertyGet.Resolve().IsStatic)
			{
				current = current.AppendLdarg(processor, 0);
			}

			// Start of branche true
			MethodReference methodReferenceRetrieve =
				methodDefinition.Module.Import(CacheTypeGetRetrieveMethod(propertyGetReturnTypeDefinition,
					CacheTypeRetrieveMethodName)).MakeGeneric(new[] { methodDefinition.ReturnType });

			current.Append(processor.Create(OpCodes.Call, methodDefinition.Module.Import(propertyGet)), processor)
				.AppendLdloc(processor, cacheKeyIndex)
				.Append(processor.Create(OpCodes.Callvirt, methodReferenceRetrieve), processor)
				.AppendStloc(processor, resultIndex)
				.Append(processor.Create(OpCodes.Br, returnInstructions.Last().Previous), processor);

			methodDefinition.Body.OptimizeMacros();
		}

		private void WeaveMethod(MethodDefinition methodDefinition, CustomAttribute attribute)
		{
			MethodDefinition propertyGet = GetCacheGetter(methodDefinition);

			if (!IsMethodValidForWeaving(propertyGet, methodDefinition))
			{
				return;
			}

			if (methodDefinition.ReturnType == methodDefinition.Module.TypeSystem.Void)
			{
				LogWarning(string.Format("Method {0} returns void. Skip weaving of method {0}.", methodDefinition.Name));

				return;
			}

			LogInfo(string.Format("Weaving method {0}::{1}.", methodDefinition.DeclaringType.Name, methodDefinition.Name));

			WeaveMethod(methodDefinition, attribute, propertyGet);
		}

		private void WeaveMethods(IEnumerable<Tuple<MethodDefinition, CustomAttribute>> methodDefinitions)
		{
			foreach (Tuple<MethodDefinition, CustomAttribute> methodDefinition in methodDefinitions)
			{
				WeaveMethod(methodDefinition.Item1, methodDefinition.Item2);
			}
		}

		private void WeaveProperties(IEnumerable<Tuple<PropertyDefinition, CustomAttribute>> properties)
		{
			foreach (Tuple<PropertyDefinition, CustomAttribute> propertyTuple in properties)
			{
				PropertyDefinition property = propertyTuple.Item1;
				CustomAttribute attribute = propertyTuple.Item2;

				// Get-Only Property, weave like normal methods
				if (property.SetMethod == null)
				{
					WeaveMethod(property.GetMethod, attribute);
				}
				else
				{
					MethodDefinition propertyGet = GetCacheGetter(property.SetMethod);

					if (!IsPropertySetterValidForWeaving(propertyGet, property.SetMethod))
					{
						continue;
					}

					LogInfo(string.Format("Weaving property {0}::{1}.", property.DeclaringType.Name, property.Name));

					WeaveMethod(property.GetMethod, attribute, propertyGet);
					WeavePropertySetter(property.SetMethod, propertyGet);
				}
			}
		}

		private void WeavePropertySetter(MethodDefinition setter, MethodReference propertyGet)
		{
			setter.Body.InitLocals = true;
			setter.Body.SimplifyMacros();

			if (propertyGet.DeclaringType.HasGenericParameters)
			{
				propertyGet = propertyGet.MakeHostInstanceGeneric(propertyGet.DeclaringType.GenericParameters.Cast<TypeReference>().ToArray());
			}

			Instruction firstInstruction = setter.Body.Instructions.First();
			ILProcessor processor = setter.Body.GetILProcessor();

			// Add local variables
			int cacheKeyIndex = setter.AddVariable(ModuleDefinition.TypeSystem.String);

			// Generate CacheKeyTemplate
			string cacheKey = CreateCacheKeyString(setter);

			Instruction current = firstInstruction.Prepend(processor.Create(OpCodes.Ldstr, cacheKey), processor);

			// Create set cache key
			current = current.AppendStloc(processor, cacheKeyIndex);

			current = InjectCacheKeyCreatedCode(setter, current, processor, cacheKeyIndex);

			if (LogDebugOutput)
			{
				current = AppendDebugWrite(current, processor, "Clearing cache.");
			}

			if (!propertyGet.Resolve().IsStatic)
			{
				current = current.AppendLdarg(processor, 0);
			}

			current
				.Append(processor.Create(OpCodes.Call, setter.Module.Import(propertyGet)), processor)
				.AppendLdloc(processor, cacheKeyIndex)
				.Append(processor.Create(OpCodes.Callvirt, setter.Module.Import(
					CacheTypeGetRemoveMethod(propertyGet.ReturnType.Resolve(), CacheTypeRemoveMethodName))), processor);

			setter.Body.OptimizeMacros();
		}
	}
}