using System;

namespace AutoBuyer.Logic.Domain
{
    public class Buyer
    {
        private readonly string _buyerName;
        private readonly int _maximumPrice;
        private readonly int _numberToBuy;
        public BuyerSnapshot Snapshot { get; private set; }

        public Buyer(string buyerName, int maximumPrice, int numberToBuy)
        {
            _buyerName = buyerName;
            _numberToBuy = numberToBuy;
            _maximumPrice = maximumPrice;
            Snapshot = BuyerSnapshot.Joining();
        }

        public StockCommand Process(StockEvent stockEvent)
        {
            if (Snapshot.State == BuyerState.Closed)
            {
                return StockCommand.None();
            }

            switch (stockEvent.Type)
            {
                case StockEventType.Price:
                    return ProcessPriceEvent(stockEvent.CurrentPrice, stockEvent.NumberInStock);

                case StockEventType.Purchase:
                    return ProcessPurchaseEvent(stockEvent.BuyerName, stockEvent.NumberSold);

                case StockEventType.Close:
                    return ProcessCloseEvent();

                default:
                    throw new InvalidOperationException();
            }
        }

        private StockCommand ProcessPriceEvent(int currentPrice, int numberInStock)
        {
            if (currentPrice > _maximumPrice)
            {
                Snapshot = Snapshot.Monitoring(currentPrice, numberInStock);
                return StockCommand.None();
            }
            else
            {
                Snapshot = Snapshot.Buying(currentPrice, numberInStock);
                int numberToBuy = Math.Min(numberInStock, _numberToBuy);
                return StockCommand.Buy(currentPrice, numberToBuy);
            }
        }

        private StockCommand ProcessPurchaseEvent(string buyerName, int numberSold)
        {
            if (buyerName == _buyerName)
            {
                Snapshot = Snapshot.Bought(numberSold);

                if (Snapshot.BoughtSoFar >= _numberToBuy)
                {
                    Snapshot = Snapshot.Closed();
                }
            }

            return StockCommand.None();
        }

        private StockCommand ProcessCloseEvent()
        {
            Snapshot = Snapshot.Closed();
            return StockCommand.None();
        }
    }
}
