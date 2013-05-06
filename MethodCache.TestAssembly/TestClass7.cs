using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.TestAssembly.Cache;

	[Cache]
	public class TestClass7
	{
		#region Constructors and Destructors

		public TestClass7(ICache cache)
		{
			Cache = cache;
		}

		#endregion

		#region Public Properties

		public ICache Cache { get; set; }

		#endregion

		#region Public Methods and Operators

		public int MethodOne(int x)
		{
			return x * x;
		}

		[NoCache]
		public MethodResult MethodTwo(string x)
		{
			return new MethodResult();
		}

		#endregion
	}
}
