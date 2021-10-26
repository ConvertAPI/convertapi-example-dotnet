using System;  
using System.IO;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  

public delegate void Notify(string request); 

class MyWebServer  
{  
    private TcpListener myListener;
    private const int Port = 5050;
    public event Notify WebhookReceived; // event

    public MyWebServer()  
    {  
        try  
        {  
            //start listing for webhook callback on the given port  
            myListener = new TcpListener(IPAddress.Any, Port);  
            myListener.Start();  
            Console.WriteLine("Web Server Running... Press ^C to Stop...");  
            //start the thread which calls the method 'StartListen'  
            var th = new Thread(new ThreadStart(StartListen));  
            th.Start();
        }  
        catch (Exception e)  
        {  
            Console.WriteLine("An Exception Occurred while Listening :" + e.ToString());  
        }  
    }

    private void StartListen()  
    {  
        while (true)  
        {  
            //Accept a new connection  
            var mySocket = myListener.AcceptSocket(); 
            if (mySocket.Connected)  
            {  
                //make a byte array and receive data from the client   
                var bReceive = new Byte[1024];  
                mySocket.Receive(bReceive, bReceive.Length, 0);  
                //Convert Byte to String  
                var sBuffer = Encoding.ASCII.GetString(bReceive);  
                //At present we will only deal with GET type  
                if (sBuffer.Substring(0, 3) != "GET")  
                {  
                    Console.WriteLine("Only Get Method is supported..");  
                    mySocket.Close();  
                    return;  
                }
                WebhookReceived?.Invoke(sBuffer);
            }  
        }  
    }    
}