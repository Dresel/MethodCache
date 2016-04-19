namespace MethodCache.Fody
{
	using System;
	using System.Collections.Generic;
	using Mono.Cecil;

	internal class MethodsForWeaving
	{
		private readonly IList<Tuple<MethodDefinition, CustomAttribute>> methods;

		private readonly IList<Tuple<PropertyDefinition, CustomAttribute>> properties;

		public MethodsForWeaving()
		{
			this.methods = new List<Tuple<MethodDefinition, CustomAttribute>>();
			this.properties = new List<Tuple<PropertyDefinition, CustomAttribute>>();
		}

		public IEnumerable<Tuple<MethodDefinition, CustomAttribute>> Methods
		{
			get { return this.methods; }
		}

		public IEnumerable<Tuple<PropertyDefinition, CustomAttribute>> Properties
		{
			get { return this.properties; }
		}

		public void Add(MethodDefinition method, CustomAttribute attribute)
		{
			this.methods.Add(new Tuple<MethodDefinition, CustomAttribute>(method, attribute));
		}

		public void Add(PropertyDefinition property, CustomAttribute attribute)
		{
			if (property.GetMethod == null)
			{
				return;
			}

			this.properties.Add(new Tuple<PropertyDefinition, CustomAttribute>(property, attribute));
		}
	}
}