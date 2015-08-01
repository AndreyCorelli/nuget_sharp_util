using System;

namespace SharpExtensionsUtil.Tcp
{
    public class SocketErrorArgs : EventArgs
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal SocketErrorArgs() { }

        /// <summary>
        /// The exception.
        /// </summary>
        public Exception Exception { get; internal set; }
    }
}
