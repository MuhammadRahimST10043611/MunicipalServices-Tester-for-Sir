using MunicipalServices.Models;

namespace MunicipalServices.Models
{
    public static class ViewHelpers
    {
        // Helper method to convert CustomLinkedList to array for views
        public static T[] ToArray<T>(this CustomLinkedList<T> customList)
        {
            if (customList == null || customList.Count == 0)
                return new T[0];

            var array = new T[customList.Count];
            customList.CopyTo(array, 0);
            return array;
        }

        // Helper method to convert CustomArray to regular array for views
        public static T[] ToArray<T>(this CustomArray<T> customArray)
        {
            if (customArray == null || customArray.Count == 0)
                return new T[0];

            var array = new T[customArray.Count];
            for (int i = 0; i < customArray.Count; i++)
            {
                array[i] = customArray[i];
            }
            return array;
        }

        // Helper method to convert CustomCollection to List for view compatibility
        public static List<T> ToList<T>(this CustomLinkedList<T> customList)
        {
            var list = new List<T>();
            foreach (var item in customList)
            {
                list.Add(item);
            }
            return list;
        }

        // Helper method to check if custom collection has any items
        public static bool HasItems<T>(this CustomLinkedList<T> customList)
        {
            return customList != null && customList.Count > 0;
        }

        // Helper to get string representation of custom array
        public static string JoinToString<T>(this CustomLinkedList<T> customList, string separator = ", ")
        {
            if (customList == null || customList.Count == 0)
                return string.Empty;

            var items = new string[customList.Count];
            int index = 0;
            foreach (var item in customList)
            {
                items[index++] = item?.ToString() ?? "";
            }
            return string.Join(separator, items);
        }
    }
}