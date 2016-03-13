using System.Xml;

namespace UpdateNuGetDeps
{
    public class Package
    {
        public string Id { get; private set; }
        public string Version { get; private set; }
        public string TargetFramework { get; private set; }

        public Package(XmlNode node)
        {
            Id = XmlUtils.GetAttr(node, "id");
            Version = XmlUtils.GetAttr(node, "version");
            TargetFramework = XmlUtils.GetAttr(node, "targetFramework");
        }
    }
}
