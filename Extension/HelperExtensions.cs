using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;

namespace HelperExtensionNameSpace
{
    public static class HelperExtensions
    {
        public static IHtmlContent RawActionLink(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes["href"] = htmlHelper.ActionLink(actionName, controllerName, routeValues).ToString();
            tagBuilder.InnerHtml.AppendHtml(linkText);

            // Here's how you convert an anonymous object to html attributes without HtmlHelper
            var attributes = GetHtmlAttributes(htmlAttributes);
            tagBuilder.MergeAttributes(attributes);

            using (var writer = new System.IO.StringWriter())
            {
                tagBuilder.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return new HtmlString(writer.ToString().Replace(repID, linkText));
            }
        }

        // Helper method to convert an anonymous object to a dictionary
        private static IDictionary<string, object> GetHtmlAttributes(object htmlAttributes)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (htmlAttributes != null)
            {
                foreach (var property in htmlAttributes.GetType().GetProperties())
                {
                    dictionary.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
                }
            }

            return dictionary;
        }
    }
}
