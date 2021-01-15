using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Util
{
    public static class ExtensionMethods
    {
        public static string[] Wrap(this string text, int max)
        {
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }
    }
}
