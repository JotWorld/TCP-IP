using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.IO.Enumeration;
using static System.Net.Mime.MediaTypeNames;

class Server
{
    static void Main()
    {
        var listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключений...");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine($"Подключение от {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

            var clientHandler = new ClientHandler(client);
            clientHandler.HandleClient();
        }
    }
    class ClientHandler
    {
        private TcpClient client;

        public ClientHandler(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        async public void HandleClient()
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[256];
            int bytesRead = 0;
            string response = "";
            string name = "";

            while (true)
            {
                try { bytesRead = stream.Read(data, 0, data.Length); }
                catch { break; }

                string receivedData = Encoding.UTF8.GetString(data, 0, bytesRead);
                Console.WriteLine($"Получено от клиента: {receivedData}");
                string[] namedata = receivedData.Split(';');
                string operation = namedata[0];
                if (operation != "")
                {
                    name = namedata[1];
                }
                else { name = ""; }
                Console.WriteLine(name);
                string path = "C:/Files/";
                switch (operation) {
                    case "PUT":
                        FileStream? fstream = null;
                        try
                        {
                            string filedata = namedata[2];
                            Console.WriteLine(filedata);
                            fstream = new FileStream(path + name, FileMode.CreateNew);
                            byte[] buffer = Encoding.Default.GetBytes(filedata);
                            fstream.WriteAsync(buffer, 0, buffer.Length);
                            response = "200!The request was sent ";
                        }
                        catch (Exception ex)
                        {
                            response = "403!The request was not sent ";
                        }
                        finally
                        {
                            fstream?.Close();
                        }
                        break;
                    case "DEL":
                        FileInfo fileDel = new FileInfo(path + name);
                        if (fileDel.Exists)
                        {
                            fileDel.Delete();
                            response = "200!The response says that the file was successfully deleted!";
                        }
                        else
                        {
                            response = "403!The response says that the file was not found!";
                        }
                        break;
                    case "GET":
                        FileInfo fileGet = new FileInfo(path + name);
                        if (fileGet.Exists)
                        {
                            using (StreamReader reader = new StreamReader(path + name))
                            {
                                string text = await reader.ReadToEndAsync();
                                response = "GET;200!" + text;
                            }
                        }
                        else
                        {
                            response = "403!The response says that the file was not found!";
                        }
                        break;
                }
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);


            }

            client.Close();
        }
    }
}
