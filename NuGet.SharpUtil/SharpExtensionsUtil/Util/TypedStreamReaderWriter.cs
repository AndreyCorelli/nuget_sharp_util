using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpExtensionsUtil.Util
{
    public static class TypedStreamReaderWriter
    {
        public static List<T> ReadFromStream<T>(string fileName, Func<string, T> parser)
        {
            return ReadFromStream(fileName, parser, Encoding.UTF8);
        }

        public static List<T> ReadFromStream<T>(string fileName, Func<string, T> parser, Encoding encoding)
        {
            if (!File.Exists(fileName))
                return new List<T>();

            var items = new List<T>();
            using (var sr = new StreamReader(fileName, encoding))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    var obj = parser(line);
                    if (obj == null) continue;
                    items.Add(obj);
                }
            }
            return items;
        }

        public static void SaveInFile<T>(string fileName, IEnumerable<T> items, Func<T, string> serializer)
        {
            SaveInFile(fileName, items, serializer, Encoding.UTF8);
        }

        public static void SaveInFile<T>(string fileName, IEnumerable<T> items, Func<T, string> serializer, Encoding encoding)
        {
            using (var sw = new StreamWriter(fileName, false, encoding))
            {
                foreach (var item in items)
                    sw.WriteLine(serializer(item));
            }
        }
    }
}
