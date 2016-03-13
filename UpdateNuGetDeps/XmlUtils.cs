using System.Xml;

namespace UpdateNuGetDeps
{
    public static class XmlUtils
    {
        public static string GetAttr(XmlNode node, string attribute)
        {
            var attr = node.Attributes[attribute];
            return attr == null ? null : attr.Value;
        }
    }
}
