using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StarmyKnife.Core.Helpers
{
    internal static class EnumExtensions
    {
        internal static string GetDisplayName(this Enum value)
        {
            if (!Enum.IsDefined(value.GetType(), value))
            {
                return "";
            }

            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)?.FirstOrDefault() is DisplayAttribute attribute && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                var nameProperty = attribute.ResourceType?.GetProperty(attribute.Name,
                                           BindingFlags.Static | BindingFlags.Public);
                var name = nameProperty?.GetValue(nameProperty.DeclaringType, null) as string;
                return name ?? attribute.Name;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
