using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSd3d
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>

        [STAThread]
        static void Main(string[] args)
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);*/

            new Lib.Thread_manager();
        }

        static void sendToServer(string senddata)
        {
            Socket sck;
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("Server"), 3939);

            try
            {
                sck.Connect(localEndPoint);
            }
            catch
            {
            }

            byte[] data = Encoding.UTF8.GetBytes(senddata);
            sck.Send(data);
            sck.Close();
        }
    }
}


//초기버전 스레드 관리자.
/*
using System;
using System.Threading;
using System.Windows.Forms;

namespace CSd3d
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new Form1());

            using (Form1 mainForm = new Form1())
            using (D3D_handler d3dHandler = new D3D_handler())
            {
                if (d3dHandler.InitallizeApplication(mainForm))
                {
                    mainForm.Show();
                    i
                    while (mainForm.Created)
                    {
                        Application.DoEvents();
                        d3dHandler.loop();
                        Thread.Sleep(D3D_handler.render_Delay);
                    }
                }
                else
                {
                    MessageBox.Show("초기화 에러");
                }
            }
        }
    }
}*/
