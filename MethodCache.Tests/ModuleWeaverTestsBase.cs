namespace MethodCache.Tests
{
	using System.Reflection;
	using System.Xml.Linq;
	using NUnit.Framework;

	[TestFixture]
	public abstract class ModuleWeaverTestsBase
	{
		private Assembly assembly;

		protected Assembly Assembly
		{
			get { return this.assembly; }
		}

		protected virtual XElement WeaverConfig
		{
			get { return null; }
		}

		[SetUp]
		public void ClassInitialize()
		{
			if (this.assembly == null)
			{
				this.assembly = WeaverHelper.WeaveAssembly(GetType().Name, WeaverConfig);
			}
		}
	}
}