using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MethodCache.Cache
{
	public interface ICache
	{
		bool Contains(string key);

		T Retrieve<T>(string key);

		void Store(string key, object data);
	}
}
