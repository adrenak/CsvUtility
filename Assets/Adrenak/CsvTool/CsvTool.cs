using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Adrenak {
    public class CsvData<T> {
        public List<string> schema;
        public List<T> rows;
    }

    public class CsvTool {
        public static List<string[]> Read(string _path) {
            List<string[]> result = new List<string[]>();

            string[] contents = File.ReadAllLines(_path);

            foreach(var line in contents) {
                var values = line.Split(',');
                result.Add(values.ToArray());
            }

            return result;
        }

        public static CsvData<T> Read<T>(string _path, List<string> _schema = null) where T : new(){
            List<T> rows = new List<T>();

            List<string> schema;
            string[] contents = File.ReadAllLines(_path);
            if (_schema == null)
                schema = contents[0].Split(',').ToList();
            else
                schema = _schema;

            for(int i = 1; i < contents.Length; i++) {
                var line = contents[i];
                var values = line.Split(',');
                var dict = ZipLists(schema, values.ToList());
                rows.Add(Deserialize<T>(dict));
            }

            var res = new CsvData<T>();
            res.schema = schema;
            res.rows = rows;
            return res;
        }

        static Dictionary<T, K> ZipLists<T, K>(List<T> list1, List<K> list2) {
            var data = new Dictionary<T, K>();
            var count = Math.Min(list1.Count, list2.Count);

            for (int i = 0; i < count; i++)
                data.Add(list1[i], list2[i]);

            return data;
        }
    
        public static T Deserialize<T>(Dictionary<string, string> data) where T : new() {
            var result = new T();
            Type type = typeof(T);

            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++) {
                var field = fields[i];

                foreach (var attribute in field.GetCustomAttributes(typeof(CsvHelperAttribute), true)) {
                    var csvAtt = (CsvHelperAttribute)attribute;
                    var attName = csvAtt.name;
                    if (!data.ContainsKey(attName)) continue;

                    var val = data[attName];

                    if (csvAtt is CsvInt)
                        field.SetValue(result, int.Parse(val));
                    else if (csvAtt is CsvFloat)
                        field.SetValue(result, float.Parse(val.Replace("f", "").Replace("F", "")));
                    else if (csvAtt is CsvString)
                        field.SetValue(result, val);
                    else if (csvAtt is CsvLong)
                        field.SetValue(result, long.Parse(val));
                    else if (csvAtt is CsvDouble)
                        field.SetValue(result, double.Parse(val));
                }
            }

            return result;
        }
    }
}
