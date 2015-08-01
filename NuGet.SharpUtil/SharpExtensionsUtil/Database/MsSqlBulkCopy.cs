using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using SharpExtensionsUtil.Logging;

namespace SharpExtensionsUtil.Database
{
    public class MsSqlBulkCopy<T> : BaseBulkCopy<T>, IBulkCopy<T>
    {
        public MsSqlBulkCopy()
        {
            ReadMappings();
        }

        public void Copy(IEnumerable<T> data, DbConnection connection, string destTableName)
        {
            // подготовить DataTable на основании mappings
            using (var table = new DataTable())
            {
                try
                {
                    foreach (var map in mappings)
                        table.Columns.Add(new DataColumn(map.columnName, map.fieldType));

                    foreach (var row in data)
                    {
                        var dataVector = mappings.Select(m => m.fieldAccessor(row)).ToArray();
                        table.Rows.Add(dataVector);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("BulkCopy() - error in mapping data", ex);
                    throw;
                }

                try
                {
                    DoBulkCopy(connection, destTableName, table);
                }
                catch (Exception ex)
                {
                    //Logger.Error("BulkCopy() - error in bulk copy", ex);
                    throw;
                }
            }
        }

        private static void DoBulkCopy(DbConnection connection, string destTableName, DataTable table)
        {
            using (var cpy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, null)
            {
                DestinationTableName = destTableName,
                BulkCopyTimeout = 10 * 60
            })
                cpy.WriteToServer(table);
        }
    }    
}
