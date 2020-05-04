using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OpenBlog.Web.WebFramework
{
    public static class HtmlExtensions
    {
        public static IEnumerable<SelectListItem> GetEnumDescriptionSelectList<TEnum>(this IHtmlHelper htmlHelper) where TEnum : struct
        {
            var result = new List<SelectListItem>();

            var enumValues = Enum.GetValues(typeof(TEnum));
            foreach (var item in enumValues)
            {
                result.Add(new SelectListItem() { Text = GetDescription<TEnum>(item.ToString()), Value = item.ToString() });
            }

            return result;
        }

        public static string GetDescription<T>(string enumerationValue) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }
    }
}