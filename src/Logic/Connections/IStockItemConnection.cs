using System;

namespace AutoBuyer.Logic.Connections
{
    public interface IStockItemConnection
    {
        event Action<string> MessageReceived;
        string BuyerName { get; }
        void SendMessage(string message);
    }
}
