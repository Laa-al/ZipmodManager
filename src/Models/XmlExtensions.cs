using System.Xml;

namespace Laaal.Models;

public static class XmlExtensions
{
    public static XmlNode? GetFirstTagOrDefault(this XmlElement element, string tagName)
    {
        var tags = element
            .GetElementsByTagName(tagName);

        return tags.Count > 0 ? tags.Item(0) : null;
    }

    public static string? GetFirstTagTextOrDefault(this XmlElement element, string tagName)
    {
        return element.GetFirstTagOrDefault(tagName)?.InnerText;
    }
}