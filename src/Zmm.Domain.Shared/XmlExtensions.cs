using System.Xml;

namespace Zmm;

public static class XmlExtensions
{
    extension(XmlElement element)
    {
        public XmlNode? GetFirstTagOrDefault(string tagName)
        {
            var tags = element
                .GetElementsByTagName(tagName);

            return tags.Count > 0 ? tags.Item(0) : null;
        }

        public string? GetFirstTagTextOrDefault(string tagName)
        {
            return element.GetFirstTagOrDefault(tagName)?.InnerText;
        }
    }
}