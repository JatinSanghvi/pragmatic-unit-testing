using AutoBuyer.Logic.Connections;
using AutoBuyer.Logic.Domain;

namespace AutoBuyer.UI
{
    public class BuyerViewModel : ViewModel
    {
        public string ItemId { get; }
        public string CurrentPrice => _buyer.Snapshot.CurrentPrice.ToString();
        public string NumberInStock => _buyer.Snapshot.NumberInStock.ToString();
        public string BoughtSoFar => _buyer.Snapshot.BoughtSoFar.ToString();
        public string State => _buyer.Snapshot.State.ToString();

        private readonly Buyer _buyer;
        private readonly IStockItemConnection _connection;

        public BuyerViewModel(string itemId, int maximumPrice, int numberToBuy,
                string buyerName, IStockItemConnection connection)
        {
            ItemId = itemId;
            _buyer = new Buyer(buyerName, maximumPrice, numberToBuy);
            _connection = connection;
            _connection.MessageReceived += StockMessageReceived;
        }

        private void StockMessageReceived(string message)
        {
            StockEvent stockEvent = StockEvent.From(message);
            StockCommand stockCommand = _buyer.Process(stockEvent);

            if (stockCommand != StockCommand.None())
            {
                _connection.SendMessage(stockCommand.ToString());
            }

            Notify(nameof(CurrentPrice));
            Notify(nameof(NumberInStock));
            Notify(nameof(BoughtSoFar));
            Notify(nameof(State));
        }
    }
}
