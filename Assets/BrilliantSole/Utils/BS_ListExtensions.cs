using System.Collections.Generic;

public static class ListExtensions
{
    public static T GetOrDefault<T>(this List<T> list, int index, T defaultValue = default)
    {
        if (list == null || index < 0 || index >= list.Count)
            return defaultValue;

        return list[index];
    }
}