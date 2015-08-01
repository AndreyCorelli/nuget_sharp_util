using System;
using System.Text;

namespace SharpExtensionsUtil.Extension
{
    public static class ExceptionFormatter
    {
        public static string ToShortString(this Exception ex)
        {
            if (ex == null) return "null";
            return ex.GetType().Name + " (" + ex.Message + ")";
        }

        public static string FormatDatabaseException(this Exception ex)
        {
            var sb = new StringBuilder(ex.ToString());
            ExploreInnerException(sb, ex);
            return sb.ToString();
        }

        private static void ExploreInnerException(StringBuilder sb, Exception ex)
        {
            if (ex == null || ex.InnerException == null) return;
            sb.AppendLine("Inner exception ========================");
            sb.AppendLine();
            sb.Append(ex.InnerException);
            if (ex.InnerException != null)
                ExploreInnerException(sb, ex.InnerException.InnerException);
        }
    }
}
