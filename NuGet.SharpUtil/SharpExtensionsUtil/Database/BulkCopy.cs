using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace SharpExtensionsUtil.Database
{
    public interface IBulkCopy<T>
    {
        void Copy(IEnumerable<T> data, DbConnection connection, string destTableName);
    }

    public class BulkFieldMapping<T>
    {
        public Type fieldType;

        public string columnName;

        public Func<T, object> fieldAccessor;

        public Func<object, string> customFormatter;

        public static BulkFieldMapping<T> MakeMapping<TProperty>(Expression<Func<T, TProperty>> property)
        {
            return MakeMapping(property, string.Empty);
        }

        public static BulkFieldMapping<T> MakeMapping<TProperty>(Expression<Func<T, TProperty>> property, string columnName)
        {
            var memberExpression = property.Body as MemberExpression;
            var propName = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(columnName))
                columnName = propName;
            var accessor = property.Compile();
            return new BulkFieldMapping<T>
            {
                columnName = columnName,
                fieldAccessor = arg => accessor(arg),
                fieldType = accessor.Method.ReturnType
            };
        }
    }

    public abstract class BaseBulkCopy<T>
    {
        public List<BulkFieldMapping<T>> mappings;

        public T speciman;

        protected void ReadMappings()
        {
            mappings = new List<BulkFieldMapping<T>>();
            foreach (var pi in typeof(T).GetProperties())
            {
                var colAttr = pi.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                if (colAttr == null) continue;
                var propInf = pi;

                var colName = colAttr.Name;
                if (string.IsNullOrEmpty(colName))
                    colName = pi.Name;
                var map = new BulkFieldMapping<T>
                {
                    columnName = colName,
                    fieldType = pi.PropertyType,
                    fieldAccessor = arg => propInf.GetValue(arg)
                };
                mappings.Add(map);
            }
        }

        public void ClearMappings()
        {
            mappings.Clear();
        }

        public BulkFieldMapping<T> AddMapping<TProperty>(Expression<Func<T, TProperty>> property, string columnName)
        {
            var mapping = BulkFieldMapping<T>.MakeMapping(property, columnName);
            mappings.Add(mapping);
            return mapping;
        }

        public BulkFieldMapping<T> AddMapping<TProperty>(Expression<Func<T, TProperty>> property)
        {
            return AddMapping(property, string.Empty);
        }
    }
}
