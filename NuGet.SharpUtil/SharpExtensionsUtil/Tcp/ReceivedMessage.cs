using System.Net.Sockets;

namespace SharpExtensionsUtil.Tcp
{
    public class ReceivedMessage
    {
        internal Socket Socket;
        internal int ThreadId;

        /// <summary>
        /// The message bytes.
        /// </summary>
        public byte[] Message;
    }
}
