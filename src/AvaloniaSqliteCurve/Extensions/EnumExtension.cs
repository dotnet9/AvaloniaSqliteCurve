using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AvaloniaSqliteCurve.Extensions;

public static class EnumExtension
{
    private static readonly ConcurrentDictionary<Type, Dictionary<int, string>> EnumNameValueDict =
        new ConcurrentDictionary<Type, Dictionary<int, string>>();

    public static Dictionary<int, string> GetDictionary(this Type enumType)
    {
        return enumType.IsEnum
            ? EnumExtension.EnumNameValueDict.GetOrAdd(enumType,
                (Func<Type, Dictionary<int, string>>)(_ => EnumExtension.GetDictionaryItems(enumType)))
            : throw new Exception("给定的类型不是枚举类型");
    }

    private static Dictionary<int, string> GetDictionaryItems(Type enumType)
    {
        FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
        Dictionary<int, string> dictionaryItems = new Dictionary<int, string>(fields.Length);
        foreach (FieldInfo fieldInfo in fields)
            dictionaryItems[(int)fieldInfo.GetValue((object)enumType)] = fieldInfo.Name;
        return dictionaryItems;
    }


    public static string GetDescription(this Enum value, params object[] args)
    {
        Type type = value.GetType();
        if (type.GetCustomAttribute<FlagsAttribute>() == null)
            return EnumExtension.GetDescriptionPrivate(value, args);
        List<string> values = new List<string>();
        foreach (Enum flag in Enum.GetValues(type))
        {
            if (Convert.ToInt64((object)flag) != 0L && value.HasFlag(flag))
                values.Add(EnumExtension.GetDescriptionPrivate(flag));
        }

        return values.Count <= 0
            ? EnumExtension.GetDescriptionPrivate(value)
            : string.Join(",", (IEnumerable<string>)values);
    }

    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        Type type = value.GetType();
        return !Enum.IsDefined(type, (object)value)
            ? Enum.GetValues(type).OfType<Enum>().Where<Enum>(new Func<Enum, bool>(value.HasFlag))
                .SelectMany<Enum, TAttribute>((Func<Enum, IEnumerable<TAttribute>>)(e =>
                    type.GetField(e.ToString()).GetCustomAttributes<TAttribute>(false)))
            : type.GetField(value.ToString()).GetCustomAttributes<TAttribute>(false);
    }

    public static IEnumerable<TEnum> Split<TEnum>(this TEnum value) where TEnum : Enum
    {
        Type enumType = typeof(TEnum);
        IEnumerable<TEnum> enums;
        if (!Enum.IsDefined(enumType, (object)value))
            enums = Enum.GetValues(enumType).Cast<TEnum>()
                .Where<TEnum>((Func<TEnum, bool>)(e => value.HasFlag((Enum)e)));
        else
            enums = (IEnumerable<TEnum>)new TEnum[1] { value };
        return enums;
    }


    public static string ToEnumString(this int value, Type enumType)
    {
        return EnumExtension.GetEnumStringFromEnumValue(enumType)[value.ToString()];
    }

    public static NameValueCollection GetEnumStringFromEnumValue(Type enumType)
    {
        NameValueCollection stringFromEnumValue = new NameValueCollection();
        foreach (FieldInfo field in enumType.GetFields())
        {
            if (field.FieldType.IsEnum)
            {
                string name = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, (Binder)null, (object)null,
                    (object[])null)).ToString();
                stringFromEnumValue.Add(name, field.Name);
            }
        }

        return stringFromEnumValue;
    }

    public static Dictionary<string, int> GetDescriptionAndValue(this Type enumType)
    {
        return Enum.GetValues(enumType).Cast<object>().ToDictionary<object, string, int>(
            (Func<object, string>)(e => (e as Enum).GetDescription()), (Func<object, int>)(e => (int)e));
    }

    private static string GetDescriptionPrivate(Enum value, params object[] args)
    {
        string description =
            (Attribute.GetCustomAttribute((MemberInfo)value.GetType().GetField(value.ToString()),
                typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
        return args.Length != 0 ? string.Format(description, args) : description;
    }
}