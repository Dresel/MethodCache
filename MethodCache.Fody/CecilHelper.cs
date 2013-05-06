namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using Mono.Cecil;
	using Mono.Cecil.Cil;

	public static class CecilHelper
	{
		#region Public Methods and Operators

		public static int AddVariable<T>(this MethodDefinition method)
		{
			method.Body.Variables.Add(method.Module.ImportVariable<T>());

			return method.Body.Variables.Count - 1;
		}

		public static int AddVariable(this MethodDefinition method, TypeReference typeReference)
		{
			method.Body.Variables.Add(method.Module.ImportVariable(typeReference));

			return method.Body.Variables.Count - 1;
		}

		public static Instruction Append(this Instruction instruction, Instruction instructionAfter, ILProcessor processor)
		{
			processor.InsertAfter(instruction, instructionAfter);

			return instructionAfter;
		}

		public static Instruction AppendBoxIfNecessary(this Instruction instruction, ILProcessor processor,
			TypeReference typeReference)
		{
			if (typeReference.IsValueType)
			{
				return instruction.Append(processor.Create(OpCodes.Box, typeReference), processor);
			}

			return instruction;
		}

		public static Instruction AppendDebugWrite(this Instruction instruction, ILProcessor processor, string message,
			ModuleDefinition module)
		{
			return instruction
				.AppendLdstr(processor, message).Append(
					processor.Create(OpCodes.Call, module.ImportMethod(typeof(Debug), "WriteLine", new[] { typeof(string) })), processor);
		}

		public static Instruction AppendLdarg(this Instruction instruction, ILProcessor processor, int index)
		{
			return instruction.Append(processor.Create(OpCodes.Ldarg, index), processor);
		}

		public static Instruction AppendLdcI4(this Instruction instruction, ILProcessor processor, int value)
		{
			return instruction.Append(processor.Create(OpCodes.Ldc_I4, value), processor);
		}

		public static Instruction AppendLdloc(this Instruction instruction, ILProcessor processor, int index)
		{
			return instruction.Append(processor.Create(OpCodes.Ldloc, index), processor);
		}

		public static Instruction AppendLdstr(this Instruction instruction, ILProcessor processor, string str)
		{
			return instruction.Append(processor.Create(OpCodes.Ldstr, str), processor);
		}

		public static Instruction AppendStloc(this Instruction instruction, ILProcessor processor, int index)
		{
			return instruction.Append(processor.Create(OpCodes.Stloc, index), processor);
		}

		public static bool ContainsAttribute(this MethodDefinition methodDefinition, MemberReference attributeType)
		{
			var attribute = methodDefinition.CustomAttributes.FirstOrDefault(x => x.Constructor.DeclaringType.FullName == attributeType.FullName);
			if (attribute != null)
			{
				methodDefinition.CustomAttributes.Remove(attribute);
				return true;
			}
			return false;
		}

		public static bool ContainsAttribute(this TypeDefinition typeDefinition, MemberReference attributeType)
		{
			var attribute = typeDefinition.CustomAttributes.FirstOrDefault(x => x.Constructor.DeclaringType.FullName == attributeType.FullName);
			if (attribute != null)
			{
				typeDefinition.CustomAttributes.Remove(attribute);
				return true;
			}
			return false;
		}

		public static MethodDefinition GetInheritedPropertyGet(this TypeDefinition baseType, string propertyName)
		{
			MethodDefinition methodDefinition = baseType.GetPropertyGet(propertyName);

			if (methodDefinition == null && baseType.BaseType != null)
			{
				return baseType.BaseType.Resolve().GetInheritedPropertyGet(propertyName);
			}

			if (methodDefinition == null && baseType.BaseType == null)
			{
				return null;
			}

			if (methodDefinition.IsPrivate)
			{
				return null;
			}

			return methodDefinition;
		}

		public static MethodDefinition GetMethod(this TypeDefinition typeDefinition, string methodName,
			MemberReference returnType, ICollection<MemberReference> parameterTypes)
		{
			return
				typeDefinition.Methods.SingleOrDefault(
					x =>
						x.Name == methodName && x.ReturnType.FullName == returnType.FullName &&
							x.Parameters.ToList().Select(y => y.ParameterType.FullName).IsEqualTo(parameterTypes.Select(y => y.FullName)));
		}

		public static MethodDefinition GetPropertyGet(this TypeDefinition typeDefinition, string propertyName)
		{
			return typeDefinition.Properties.Where(x => x.Name == propertyName).Select(x => x.GetMethod).SingleOrDefault();
		}

		public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName)
		{
			return module.ImportMethod(typeof(T), methodName);
		}

		public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName, Type[] types)
		{
			return module.ImportMethod(typeof(T), methodName, types);
		}

		public static MethodReference ImportMethod(this ModuleDefinition module, Type type, string methodName)
		{
			MethodInfo methodInfo = type.GetMethod(methodName);

			return module.Import(methodInfo);
		}

		public static MethodReference ImportMethod(this ModuleDefinition module, Type type, string methodName, Type[] types)
		{
			MethodInfo methodInfo = type.GetMethod(methodName, types);

			return module.Import(methodInfo);
		}

		public static TypeReference ImportType<T>(this ModuleDefinition module)
		{
			return module.ImportType(typeof(T));
		}

		public static TypeReference ImportType(this ModuleDefinition module, Type type)
		{
			return module.Import(type);
		}

		public static VariableDefinition ImportVariable<T>(this ModuleDefinition module)
		{
			return new VariableDefinition(module.Import(typeof(T)));
		}

		public static VariableDefinition ImportVariable(this ModuleDefinition module, TypeReference typeReference)
		{
			return new VariableDefinition(module.Import(typeReference));
		}

		public static bool IsEqualTo<T>(this IEnumerable<T> collection, IEnumerable<T> collectionToCompare)
		{
			int index = 0;

			IEnumerable<T> list = collection as IList<T> ?? collection.ToList();
			IEnumerable<T> listToCompare = collectionToCompare as IList<T> ?? collectionToCompare.ToList();

			if (list.Count() != listToCompare.Count())
			{
				return false;
			}

			foreach (T item in list)
			{
				if (!EqualityComparer<T>.Default.Equals(item, listToCompare.ElementAt(index)))
				{
					return false;
				}

				index++;
			}

			return true;
		}

		public static MethodReference MakeGeneric(this MethodReference method, params TypeReference[] arguments)
		{
			if (method.GenericParameters.Count != arguments.Length)
			{
				throw new ArgumentException("Invalid number of generic type arguments supplied");
			}

			if (arguments.Length == 0)
			{
				return method;
			}

			GenericInstanceMethod genericTypeReference = new GenericInstanceMethod(method);

			foreach (TypeReference argument in arguments)
			{
				genericTypeReference.GenericArguments.Add(argument);
			}

			return genericTypeReference;
		}

		public static Instruction Prepend(this Instruction instruction, Instruction instructionBefore, ILProcessor processor)
		{
			processor.InsertBefore(instruction, instructionBefore);

			return instructionBefore;
		}

		#endregion
	}
}
