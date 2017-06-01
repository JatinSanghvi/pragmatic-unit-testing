using AutoBuyer.Logic.Domain;
using Should;
using Xunit;

namespace Tests.Unit
{
    public class BuyerTests
    {
        [Fact]
        public void New_buyer_is_in_joining_state()
        {
            var buyer = CreateJoiningBuyer();

            buyer.SnapshotShouldEqual(BuyerState.Joining, 0, 0, 0);
        }

        [Fact]
        public void Closes_when_item_closes()
        {
            var buyer = CreateJoiningBuyer();

            StockCommand command = buyer.Process(StockEvent.Close());

            command.ShouldEqual(StockCommand.None());
            buyer.SnapshotShouldEqual(BuyerState.Closed, 0, 0, 0);
        }

        [Fact]
        public void Buyer_does_not_buy_when_price_event_with_too_high_price_arrives()
        {
            var buyer = CreateJoiningBuyer();

            StockCommand command = buyer.Process(StockEvent.Price(200, 10));

            command.ShouldEqual(StockCommand.None());
            buyer.SnapshotShouldEqual(BuyerState.Monitoring, 200, 10, 0);
        }

        [Fact]
        public void Buyer_buys_when_price_event_with_appropriate_price_arrives()
        {
            var buyer = CreateJoiningBuyer();

            StockCommand command = buyer.Process(StockEvent.Price(50, 20));

            command.ShouldEqual(StockCommand.Buy(50, 10));
            buyer.SnapshotShouldEqual(BuyerState.Buying, 50, 20, 0);
        }

        [Fact]
        public void Buyer_attempts_to_buy_maximum_amount_available()
        {
            var buyer = CreateJoiningBuyer();

            StockCommand command = buyer.Process(StockEvent.Price(50, 5));

            command.ShouldEqual(StockCommand.Buy(50, 5));
            buyer.SnapshotShouldEqual(BuyerState.Buying, 50, 5, 0);
        }

        [Fact]
        public void Buyer_does_not_react_to_a_purchase_event_related_to_another_buyer()
        {
            var buyer = CreateMonitoringBuyer("Buyer name");

            StockCommand command = buyer.Process(StockEvent.Purchase("Different buyer name", 5));

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot.State.ShouldEqual(BuyerState.Monitoring);
            buyer.Snapshot.BoughtSoFar.ShouldEqual(0);
        }

        [Fact]
        public void Buyer_updates_items_bought_so_far_when_purchase_event_with_the_same_user_name_arrives()
        {
            var buyer = CreateMonitoringBuyer("Buyer name", numberInStock: 5);

            StockCommand command = buyer.Process(StockEvent.Purchase("Buyer name", 1));

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot.State.ShouldEqual(BuyerState.Monitoring);
            buyer.Snapshot.BoughtSoFar.ShouldEqual(1);
            buyer.Snapshot.NumberInStock.ShouldEqual(4);
        }

        [Fact]
        public void Buyer_closes_when_it_buys_enough_items()
        {
            var buyer = CreateMonitoringBuyer("Buyer name", numberToBuy: 10);

            StockCommand command = buyer.Process(StockEvent.Purchase("Buyer name", 20));

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot.State.ShouldEqual(BuyerState.Closed);
        }

        [Fact]
        public void Closed_buyer_does_not_react_to_further_messages_()
        {
            var buyer = CreateClosedBuyer();

            StockCommand command = buyer.Process(StockEvent.Price(50, 5));

            command.ShouldEqual(StockCommand.None());
        }

        private static Buyer CreateClosedBuyer()
        {
            var buyer = new Buyer("Buyer Name", 100, 10);
            buyer.Process(StockEvent.Close());
            return buyer;
        }

        private static Buyer CreateJoiningBuyer(int maximumPrice = 100, int numberToBuy = 10)
        {
            return new Buyer("Buyer Name", maximumPrice, numberToBuy);
        }

        private static Buyer CreateMonitoringBuyer(string buyerName = "Buyer Name", int numberInStock = 5, int numberToBuy = 10)
        {
            var buyer = new Buyer(buyerName, 100, numberToBuy);
            buyer.Process(StockEvent.Price(200, numberInStock));
            return buyer;
        }
    }

    internal static class BuyerExtensions
    {
        public static void SnapshotShouldEqual(this Buyer buyer, BuyerState state, int currentPrice,
            int numberInStock, int boughtSoFar)
        {
            buyer.Snapshot.State.ShouldEqual(state);
            buyer.Snapshot.CurrentPrice.ShouldEqual(currentPrice);
            buyer.Snapshot.NumberInStock.ShouldEqual(numberInStock);
            buyer.Snapshot.BoughtSoFar.ShouldEqual(boughtSoFar);
        }
    }
}
