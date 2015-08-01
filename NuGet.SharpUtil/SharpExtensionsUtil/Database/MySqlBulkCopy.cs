using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SharpExtensionsUtil.Converter;
using SharpExtensionsUtil.Util;

namespace SharpExtensionsUtil.Database
{
    public class MySqlBulkCopy<T> : BaseBulkCopy<T>, IBulkCopy<T>
    {
        public string tempFileName = "temp_file.txt";

        private string charSet = "utf8";

        public string CharSet
        {
            get { return charSet; }
            set { charSet = value; }
        }

        public bool ContinueOnErrors { get; set; }

        public MySqlBulkCopy()
        {
            ReadMappings();
        }

        public void Copy(IEnumerable<T> data, DbConnection connection, string destTableName)
        {
            var fileName = ExecutablePath.Combine(tempFileName);
            // файл без BOM !!
            using (var sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                foreach (var row in data)
                    WriteFileLine(row, sw);
            }

            var cpy = new MySqlBulkLoader((MySqlConnection)connection)
            {
                TableName = destTableName,
                FieldTerminator = ";",
                LineTerminator = "\r\n",
                FileName = fileName,
                NumberOfLinesToSkip = 0,
                CharacterSet = CharSet,
                Timeout = 60 * 10
            };
            if (ContinueOnErrors)
                cpy.ConflictOption = MySqlBulkLoaderConflictOption.Ignore;
            cpy.Load();

            try
            {
                File.Delete(fileName);
            }
            catch
            {
            }
        }

        private void WriteFileLine(T row, StreamWriter sw)
        {
            var values = (from mapping in mappings
                          let value = mapping.fieldAccessor(row)
                          select StringifyFileValue(value, mapping)).ToList();
            var line = string.Join(";", values);
            sw.WriteLine(line);
        }

        private string StringifyFileValue(object val, BulkFieldMapping<T> mapping)
        {
            if (mapping.customFormatter != null)
                return mapping.customFormatter(val);

            if (val == null) return "NULL";

            if (val is string || val is int || val is long ||
                val is bool || val is uint || val is ulong || val is short ||
                val is ushort) return "" + val + "";

            if (val is float) return "" + ((float)val).ToStringUniform() + "";
            if (val is double) return "" + ((double)val).ToStringUniform() + "";
            if (val is decimal) return "" + ((decimal)val).ToStringUniform() + "";
            if (val.GetType().IsEnum) return "" + ((int)val) + "";
            if (val is DateTime)
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", (DateTime)val);

            return "" + val + "";
        }
    }
}
