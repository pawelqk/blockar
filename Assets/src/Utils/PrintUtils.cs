using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class PrintUtils
    {
        public static string PrintCollection<T>(ICollection<T> collection)
        {
            if (collection is null)
                return "";

            var sb = new StringBuilder("[");
            foreach (var element in collection)
                sb.Append($"{element}, ");

            if (sb.Length >= 3)
                sb.Remove(sb.Length - 2, 2);

            sb.Append("]");

            return sb.ToString();
        }
    }
}
