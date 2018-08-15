using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//socket관련
using System.Net;
using System.Net.Sockets;

namespace ITKOO_s_Talk
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 소켓 세팅
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // user IP 얻기
            textLocalIP.Text = GetLocalIP();
            textRemoteIP.Text = GetLocalIP();
        } // end of  Form1_Load

        private string GetLocalIP() 
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "127.0.0.1";
        } // end of GetLocalIP

       

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // bind 
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIP.Text), Convert.ToInt32(textLocalPort.Text)); // 나의 info
            sck.Bind(epLocal);

            // remote 된 IP 와 Connect
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIP.Text), Convert.ToInt32(textRemotePort.Text)); // 상대방 info
            sck.Connect(epRemote);

            // listen 
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

        } // end of buttonConnect_Click

        private void MessageCallBack(IAsyncResult aResult) 
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;
                // byte -> string
                //ASCIIEncoding aEncoding = new ASCIIEncoding();
                UnicodeEncoding uEncoding = new UnicodeEncoding();
                string receivedMessage = uEncoding.GetString(receivedData);

                // listBox에 메세지 추가
                listMessage.Items.Add("친구 : " + receivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }

            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
           
        } // end of MessageCallBack

        private void buttonSend_Click(object sender, EventArgs e)
        {
            // string -> byte[]
            //ASCIIEncoding aEncoding = new ASCIIEncoding();
            UnicodeEncoding uEncoding = new UnicodeEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = uEncoding.GetBytes(textMessage.Text);

            // encoded message 보내기
            sck.Send(sendingMessage);

            // listBox에 추가
            listMessage.Items.Add("나 : " + textMessage.Text);
            textMessage.Text = "";

        } // end of buttonSend 

        private void title_Click(object sender, EventArgs e)
        {

        } 
    } // end of class
} // end of project
