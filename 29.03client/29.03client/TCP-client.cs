using System;
using System.Net.Sockets;

using System.Text;

class Client
{
    static void Main()
    {
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            try
            {
                socket.Connect("127.0.0.1", 8888);
                bool exit = false;
                while (!(exit))
                {
                    string filename = "";
                    string data = "";
                    string operation = "NON";
                    string flag = "";
                    exit = false;
                    Console.Write("Enter action (1 - get a file, 2 - create a file, 3 - delete a file): > ");
                    flag = Console.ReadLine();
                    switch (flag)
                    {
                        case "2":
                            operation = "PUT";
                            Console.Write("Enter filename: >");
                            filename = Console.ReadLine();
                            Console.Write("Enter file content: >");
                            data = Console.ReadLine();
                            break;
                        case "3":
                            operation = "DEL";
                            Console.Write("Enter filename: >");
                            filename = Console.ReadLine();
                            break;
                        case "1":
                            operation = "GET";
                            Console.Write("Enter filename: >");
                            filename = Console.ReadLine();
                            break;
                        case "exit":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid operation");
                            break;
                    }

                    if (!exit)
                    {
                        byte[] sendData = Encoding.UTF8.GetBytes(operation + ";" + filename + ";" + data);
                        socket.Send(sendData);
                        byte[] receiveData = new byte[256];
                        int bytesRead = socket.Receive(receiveData);
                        string result = Encoding.UTF8.GetString(receiveData, 0, bytesRead);
                        if ((result[0] == 'G') && (result[1] == 'E') && (result[2] == 'T') && (result[3] == ';')) { result = result.Remove(0, 4); }
                        Console.WriteLine($"{result}");
                    }
                    else
                    {
                        socket.Close();
                    }

                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"Не удалось установить подключение");
            }
        }
    }
}
