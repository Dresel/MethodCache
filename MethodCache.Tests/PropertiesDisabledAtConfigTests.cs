namespace MethodCache.Tests
{
    using System.Xml.Linq;

    public class PropertiesDisabledAtConfigTests : ModuleWeaverTestsBase
    {
        protected override XElement WeaverConfig
        {
            get
            {
                return new XElement("MethodCache",
                                    new XAttribute("CacheProperties", false)
                    );
            }
        }
    }
}