using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tcp_lap
{
    class SocketPacket
    {
        // property of type Socket & Name : Client 

        public Socket client { get; set; }

        // property of type byte array  & Name : buffer 
        public byte[] buffer { get; set; }

        //Constructor , parameter client of type socket 
        public SocketPacket(Socket client)
        {
            this.client = client;
            this.buffer = new byte[1024];
        }
    }
}
