using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using DataSet = System.Data.DataSet;

namespace server
{
	class Program
	{
		static private Socket socketBinder()
		{
			TcpClient client = new TcpClient();
			IPHostEntry host = Dns.GetHostEntry("mellorin.0pe.kr");
			string ip = host.AddressList[0].ToString();

			Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			sck.Bind(new IPEndPoint(IPAddress.Parse(ip), 3939));

			return sck;
		}

		static private string dbConnect(string uuid, string token)
		{
			string strDBConn = "Server=localhost;Database=capstoneserver;Uid=root;Pwd=rpavpdls3;";
			MySqlConnection dbConn = new MySqlConnection(strDBConn);
			dbConn.Open();

			string dbQuery = String.Format("select uuid from data where token = '{0}'", token);

			DataSet dbDataSet = new DataSet();
			MySqlDataAdapter dbAdapter = new MySqlDataAdapter(dbQuery, dbConn);
			dbAdapter.Fill(dbDataSet, "members");



			if (dbDataSet.Tables[0].Rows.Count != 0)
			{
				uuid = (string)dbDataSet.Tables[0].Rows[0]["uuid"];
				Console.WriteLine("값 찾음 uuid = {0}", uuid);
			}
			else
			{
				string insertQuery = String.Format("insert into data (uuid,token) values ('{0}','{1}');", uuid, token);
				MySqlCommand cmd = new MySqlCommand(insertQuery, dbConn);
				cmd.ExecuteNonQuery();
				Console.WriteLine("값 입력됨 uuid = {0}", uuid);
			}

			dbConn.Close();
			return uuid;
		}

		static void Main(string[] args)
		{
			try
			{
				Socket socket = socketBinder();
				while (true)
				{
					socket.Listen(100);

					Socket accepted = socket.Accept();

					byte[] Buffer = new byte[accepted.SendBufferSize];
					int bytesRead = accepted.Receive(Buffer);

					byte[] formatted = new byte[bytesRead];
					//string echo = "";

					for (int i = 0; i < bytesRead; ++i)
					{
						formatted[i] = Buffer[i];
					}

					string received = Encoding.UTF8.GetString(formatted);
					Console.WriteLine(received);
					/*
                    string uuid = received.Split('/')[0];
                    string token = received.Split('/')[1];

                    echo = dbConnect(uuid, token);

                    Console.WriteLine(received);
                    byte[] send = Encoding.UTF8.GetBytes(echo);

                    accepted.Send(send);
                    */
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}
