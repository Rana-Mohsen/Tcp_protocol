using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tcp_lap
{
    public partial class Server : Form
    {
        Socket server;
        Dictionary<string, Socket> Clients = new Dictionary<string, Socket>();

        public Server()
        {
            InitializeComponent();
        }

        private void UpdateGUI(bool isConnected)
        {
            btnStart.Enabled = !isConnected;
            btnSend.Enabled = isConnected;
            lblStatus.Text = isConnected ? "Connected" : "DisConnected";
            lblStatus.ForeColor = isConnected ? Color.Green : Color.Red;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            int portNum = int.Parse(txtPort.Text);
            server.Bind(new IPEndPoint(IPAddress.Any, portNum));
            server.Listen(5);

            UpdateGUI(true);

            server.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }
        private void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                
                Socket client = server.EndAccept(ar);
                //**update**
                SocketPacket Packet = new SocketPacket(client);
                client.BeginReceive(Packet.buffer, 0, Packet.buffer.Length, SocketFlags.None, new AsyncCallback(nameClient), Packet);

                //Console.WriteLine(name);*/

                ReadyForData(client);


            }
            catch
            {
                server.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            server.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }

        private void ReadyForData(Socket client) 
        {
            SocketPacket Packet = new SocketPacket(client);
            client.BeginReceive(Packet.buffer, 0, Packet.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), Packet);

        }
        private void OnReceive(IAsyncResult ar)
        {
            SocketPacket Packet = (SocketPacket)ar.AsyncState;
            Packet.client.EndReceive(ar);
            string msg = Encoding.Unicode.GetString(Packet.buffer);

            WriteLog(msg);

            ReadyForData(Packet.client);

        }
        //**update**
        private void nameClient(IAsyncResult ar)
        {
            SocketPacket Packet = (SocketPacket)ar.AsyncState;
            Packet.client.EndReceive(ar);
            string name = Encoding.Unicode.GetString(Packet.buffer);
            name = name.Replace("\0",""); // This line removes "?" from the name

            Clients.Add(name, Packet.client);

            Console.WriteLine(name);

        }
        private void WriteLog(string msg)
        {


            MethodInvoker invoke = 
                new MethodInvoker(delegate { txtLog.AppendText("Client Said : " + msg + Environment.NewLine); });
            this.BeginInvoke(invoke);

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //**update**
            string name = txtName.Text;
            string msg = txtMsg.Text;
            byte[] buffer = Encoding.Unicode.GetBytes(msg);

            if (name.Length>0 && Clients[name].Connected)
            {
                Clients[name].Send(buffer);
            }
            else
            {
                WriteLog("client is not connected");
            }
        }

        private void btnNewClient_Click(object sender, EventArgs e)
        {
            Client c = new Client();
            c.Show();
        }

      
    }
}