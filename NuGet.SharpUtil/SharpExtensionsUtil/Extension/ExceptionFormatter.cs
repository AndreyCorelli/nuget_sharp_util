using System;
using System.Collections.Generic;

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
            return ex.ExploreInnerException();
        }

        public static string ExploreInnerException(this Exception ex)
        {
            var errors = new List<string>();
            ExploreInnerException(ex, errors);
            return string.Join("> ", errors);
        }

        private static void ExploreInnerException(Exception ex, List<string> errors)
        {
            errors.Add(ex.GetType().Name + ": " + ex.Message);
            if (ex.InnerException != null)
                ExploreInnerException(ex.InnerException, errors);
        }
    }
}
