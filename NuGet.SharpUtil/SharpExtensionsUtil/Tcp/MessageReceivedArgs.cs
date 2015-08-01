using System;

namespace SharpExtensionsUtil.Tcp
{
    public class MessageReceivedArgs : EventArgs
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal MessageReceivedArgs() { }

        /// <summary>
        /// The received message.
        /// </summary>
        public ReceivedMessage ReceivedMessage { get; internal set; }
    }
}
