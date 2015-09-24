using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SharpExtensionsUtil.Extension
{
    public static class EnumDescription
    {
        private static readonly Dictionary<Type, Dictionary<object, string>> enumNameDescription =
            new Dictionary<Type, Dictionary<object, string>>();

        public static string GetEnumDescription<TEnum>(this TEnum value)
        {
            Dictionary<object, string> items;
            if (enumNameDescription.TryGetValue(typeof(TEnum), out items))
                return items[value];

            items = new Dictionary<object, string>();
            enumNameDescription.Add(typeof(TEnum), items);
            foreach (var enumVal in Enum.GetValues(typeof(TEnum)))
                items.Add(enumVal, DeriveEnumDescription(enumVal));
            return items[value];
        }

        public static string DeriveEnumDescription<TEnum>(TEnum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}
