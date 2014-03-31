namespace MethodCache.Tests
{
	using System.Reflection;
	using System.Xml.Linq;
	using NUnit.Framework;

	[TestFixture]
	public abstract class ModuleWeaverTestsBase
	{
		private Assembly _assembly;

		protected Assembly Assembly
		{
			get { return this._assembly; }
		}

		protected virtual XElement WeaverConfig
		{
			get { return null; }
		}

		[SetUp]
		public void ClassInitialize()
		{
			if (this._assembly == null)
			{
				this._assembly = WeaverHelper.WeaveAssembly(GetType().Name, WeaverConfig);
			}
		}
	}
}