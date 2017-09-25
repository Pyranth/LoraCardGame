using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    
    public interface IClient
    {
        bool ConnectToServer();
        void StartListening();
        void Stop();
        void SendMessage(string message);

        event MessageRecievedHandler MessageRecieved;

    }
}
