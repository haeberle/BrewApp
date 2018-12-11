using BrewApp.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BrewApp.Controls.Brewery.Logger
{

    public static class Logger
    {
        public static void Log(string fileName, string text)
        {
            if (!File.Exists(Path.Combine(Constants.ApplicationLogPath, fileName)))
            {
                Directory.CreateDirectory(Constants.ApplicationLogPath);
            }
           
            System.IO.File.AppendAllText(fileName, text + Environment.NewLine);
        }
        public static void Log(string fileName, LogItem item, bool header = false)
        {
            if (!File.Exists(Path.Combine(Constants.ApplicationLogPath, fileName)))
            {
                Directory.CreateDirectory(Constants.ApplicationLogPath);
            }

            if (header)
            {
                System.IO.File.AppendAllText(fileName, ToCsvHeader<LogItem>(item));
            }
            System.IO.File.AppendAllText(fileName, ToCsv<LogItem>(item));
        }

        public static void Header<T>(string fileName) where T: new()
        {
            if (!File.Exists(Path.Combine(Constants.ApplicationLogPath, fileName)))
            {
                Directory.CreateDirectory(Constants.ApplicationLogPath);
            }

            System.IO.File.AppendAllText(fileName, ToCsvHeader<T>(new T()));
        }

        public static string ToCsvHeader<T>(T item, string separator = ",")
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            return String.Join(separator, fields.Select(f => f.Name).Concat(properties.Select(p => p.Name)).ToArray()) + Environment.NewLine; ;
        }

        public static string ToCsv<T>(T item, string separator = ",")
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            string str2;
            //regex is to remove any misplaced returns or tabs that would
            //really mess up a csv conversion.
            str2 = string.Join(separator, fields.Select(f => (Regex.Replace(Convert.ToString(f.GetValue(item)), @"\t|\n|\r", "") ?? "").Trim())
               .Concat(properties.Select(p => (Regex.Replace(Convert.ToString(p.GetValue(item, null)), @"\t|\n|\r", "") ?? "").Trim())).ToArray());

            str2 = str2 + Environment.NewLine;
            return str2;

        }
        public static IEnumerable<string> ToCsv<T>(IEnumerable<T> objectlist, string separator = ",", bool header = true)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            string str1;
            string str2;

            if (header)
            {
                str1 = String.Join(separator, fields.Select(f => f.Name).Concat(properties.Select(p => p.Name)).ToArray());
                str1 = str1 + Environment.NewLine;
                yield return str1;
            }
            foreach (var o in objectlist)
            {
                //regex is to remove any misplaced returns or tabs that would
                //really mess up a csv conversion.
                str2 = string.Join(separator, fields.Select(f => (Regex.Replace(Convert.ToString(f.GetValue(o)), @"\t|\n|\r", "") ?? "").Trim())
                   .Concat(properties.Select(p => (Regex.Replace(Convert.ToString(p.GetValue(o, null)), @"\t|\n|\r", "") ?? "").Trim())).ToArray());

                str2 = str2 + Environment.NewLine;
                yield return str2;
            }
        }
        public static string ToCsv<T>(this IEnumerable<T> list, params Func<T, string>[] properties)
        {
            var columns = properties.Select(func => list.Select(func).ToList()).ToList();

            var stringBuilder = new StringBuilder();

            var rowsCount = columns.First().Count;

            for (var i = 0; i < rowsCount; i++)
            {
                var rowCells = columns.Select(column => column[i]);

                stringBuilder.AppendLine(string.Join(",", rowCells));
            }

            return stringBuilder.ToString();
        }



        //public string GetHeader<T>(string separator)
        //{
        //    Type t = typeof(T);
        //    FieldInfo[] fields = t.GetFields();

        //    string header = String.Join(separator, fields.Select(f => f.Name).ToArray());

        //    StringBuilder csvdata = new StringBuilder();
        //    csvdata.AppendLine(header);
        //    return csvdata.ToString();
        //}

        //public string GetLine<T>(string separator)
        //{
        //    Type t = typeof(T);
        //    FieldInfo[] fields = t.GetFields();

        //    string header = String.Join(separator, fields.Select(f => f.Name).ToArray());

        //    StringBuilder csvdata = new StringBuilder();
        //    csvdata.AppendLine(header);
        //    return csvdata.ToString();
        //}

        //public static string ToCsv<T>(string separator, IEnumerable<T> objectlist)
        //{
        //    Type t = typeof(T);
        //    FieldInfo[] fields = t.GetFields();

        //    string header = String.Join(separator, fields.Select(f => f.Name).ToArray());

        //    StringBuilder csvdata = new StringBuilder();
        //    csvdata.AppendLine(header);

        //    foreach (var o in objectlist)
        //        csvdata.AppendLine(ToCsvFields(separator, fields, o));

        //    return csvdata.ToString();
        //}

        //public static string ToCsvFields(string separator, FieldInfo[] fields, object o)
        //{
        //    StringBuilder linie = new StringBuilder();

        //    foreach (var f in fields)
        //    {
        //        if (linie.Length > 0)
        //            linie.Append(separator);

        //        var x = f.GetValue(o);

        //        if (x != null)
        //            linie.Append(x.ToString());
        //    }

        //    return linie.ToString();
        //}
    }
}
