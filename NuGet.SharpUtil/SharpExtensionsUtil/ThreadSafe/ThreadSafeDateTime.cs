using System;
using System.Threading;

namespace SharpExtensionsUtil.ThreadSafe
{
    public class ThreadSafeDateTime
    {
        private DateTime? time;
        private readonly ReaderWriterLock locker = new ReaderWriterLock();

        public DateTime? Value
        {
            get
            {
                try
                {
                    locker.AcquireReaderLock(200);
                }
                catch
                {
                    return null;
                }
                try
                {
                    return time;
                }
                finally
                {
                    locker.ReleaseReaderLock();
                }
            }
            set
            {
                try
                {
                    locker.AcquireWriterLock(200);
                }
                catch
                {
                    return;
                }
                try
                {
                    time = value;
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            }
        }
    }
}
