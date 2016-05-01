namespace MethodCache.Tests
{
	using System;
	using System.Diagnostics;
	using Mono.Cecil;

	public class AssemblyResolverMock : IAssemblyResolver
	{
		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			throw new NotImplementedException();
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			throw new NotImplementedException();
		}

		public AssemblyDefinition Resolve(string fullName)
		{
			if (fullName == "System")
			{
				string codeBase = typeof(Debug).Assembly.CodeBase.Replace("file:///", "");
				return AssemblyDefinition.ReadAssembly(codeBase);
			}
			else
			{
				string codeBase = typeof(string).Assembly.CodeBase.Replace("file:///", "");
				return AssemblyDefinition.ReadAssembly(codeBase);
			}
		}

		public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
		{
			throw new NotImplementedException();
		}
	}
}