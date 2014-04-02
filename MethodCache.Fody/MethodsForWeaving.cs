namespace MethodCache.Fody
{
	using System.Collections.Generic;
	using Mono.Cecil;

	internal class MethodsForWeaving
	{
		private readonly IList<MethodDefinition> methods;

		private readonly IList<PropertyDefinition> properties;

		public MethodsForWeaving()
		{
			this.methods = new List<MethodDefinition>();
			this.properties = new List<PropertyDefinition>();
		}

		public IEnumerable<MethodDefinition> Methods
		{
			get { return this.methods; }
		}

		public IEnumerable<PropertyDefinition> Properties
		{
			get { return this.properties; }
		}

		public void Add(MethodDefinition method)
		{
			this.methods.Add(method);
		}

		public void Add(PropertyDefinition property)
		{
			if (property.GetMethod == null)
			{
				return;
			}

			this.properties.Add(property);
		}
	}
}