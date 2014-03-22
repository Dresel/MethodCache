using System.Collections.Generic;
using Mono.Cecil;

namespace MethodCache.Fody
{
    internal class MethodsForWeaving
    {
        private readonly IList<MethodDefinition> _methods;
        private readonly IList<MethodDefinition> _setters;

        public MethodsForWeaving()
        {
            _methods = new List<MethodDefinition>();
            _setters = new List<MethodDefinition>();
        }

        public IEnumerable<MethodDefinition> Methods
        {
            get { return _methods; }
        }

        public IEnumerable<MethodDefinition> PropertySetters
        {
            get { return _setters; }
        }

        public void Add(MethodDefinition method)
        {
            _methods.Add(method);
        }

        public void Add(PropertyDefinition property)
        {
            MethodDefinition getter = property.GetMethod;
            MethodDefinition setter = property.SetMethod;

            if (getter == null)
            {
                return;
            }

            _methods.Add(getter);
            if (setter != null)
            {
                _setters.Add(setter);
            }
        }
    }
}