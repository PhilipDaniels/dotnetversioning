using System;
using System.Xml;

namespace UpdateNuGetDeps
{
    public class Package
    {
        public string Id { get; private set; }
        public string Version { get; private set; }
        public string TargetFramework { get; private set; }
        public bool DevelopmentDependency { get; private set; }

        public Package(XmlNode node)
        {
            Id = GetAttr(node, "id");
            Version = GetAttr(node, "version");
            TargetFramework = GetAttr(node, "targetFramework");
            DevelopmentDependency = Convert.ToBoolean(GetAttr(node, "developmentDependency"));
        }

        private static string GetAttr(XmlNode node, string attribute)
        {
            var attr = node.Attributes[attribute];
            return attr == null ? null : attr.Value;
        }
    }
}
