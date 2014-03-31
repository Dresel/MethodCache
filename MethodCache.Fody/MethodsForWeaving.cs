namespace MethodCache.Fody
{
	using System.Collections.Generic;
	using Mono.Cecil;

	internal class MethodsForWeaving
	{
		private readonly IList<MethodDefinition> _methods;

		private readonly IList<MethodDefinition> _setters;

		public MethodsForWeaving()
		{
			this._methods = new List<MethodDefinition>();
			this._setters = new List<MethodDefinition>();
		}

		public IEnumerable<MethodDefinition> Methods
		{
			get { return this._methods; }
		}

		public IEnumerable<MethodDefinition> PropertySetters
		{
			get { return this._setters; }
		}

		public void Add(MethodDefinition method)
		{
			this._methods.Add(method);
		}

		public void Add(PropertyDefinition property)
		{
			MethodDefinition getter = property.GetMethod;
			MethodDefinition setter = property.SetMethod;

			if (getter == null)
			{
				return;
			}

			this._methods.Add(getter);
			if (setter != null)
			{
				this._setters.Add(setter);
			}
		}
	}
}