namespace MethodCache.Tests
{
	using System.Diagnostics;
	using Mono.Cecil;

	public class AssemblyResolverMock : IAssemblyResolver
	{
		public void Dispose()
		{
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			return Resolve(name.FullName);
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			return Resolve(name.FullName);
		}

		public AssemblyDefinition Resolve(string fullName)
		{
			if (fullName.StartsWith("System"))
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
	}
}