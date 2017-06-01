using AutoBuyer.Logic.Connections;
using AutoBuyer.Logic.Database;
using AutoBuyer.Logic.Domain;
using AutoBuyer.UI;
using Tests.Utils;
using Moq;
using Should;
using Xunit;

namespace Tests.Integration
{
    public class BuyerViewModelTests : Tests
    {
        [Fact]
        public void Joining_a_sale()
        {
            var connectionMock = new Mock<IStockItemConnection>();

            new BuyerViewModel("Item ID", 100, 10, "Buyer Name", connectionMock.Object,
                new BuyerRepository(ConnectionString));

            BuyerDto buyer = DatabaseHelper.GetBuyer("Item ID");

            buyer.MaximumPrice.ShouldEqual(100);
            buyer.NumberToBuy.ShouldEqual(10);
            buyer.BuyerName.ShouldEqual("Buyer Name");
            buyer.State.ShouldEqual(BuyerState.Joining);
            buyer.BoughtSoFar.ShouldEqual(0);
            buyer.CurrentPrice.ShouldEqual(0);
            buyer.NumberInStock.ShouldEqual(0);
        }

        [Fact]
        public void Buying_an_item()
        {
            var connectionMock = new Mock<IStockItemConnection>();

            new BuyerViewModel("Item ID", 100, 10, "Buyer Name", connectionMock.Object,
                new BuyerRepository(ConnectionString));

            connectionMock.Raise(
                x => x.MessageReceived += null,
                "Event: PRICE; CurrentPrice: 50; NumberInStock: 200;");

            BuyerDto buyer = DatabaseHelper.GetBuyer("Item ID");

            buyer.MaximumPrice.ShouldEqual(100);
            buyer.NumberToBuy.ShouldEqual(10);
            buyer.BuyerName.ShouldEqual("Buyer Name");
            buyer.State.ShouldEqual(BuyerState.Buying);
            buyer.BoughtSoFar.ShouldEqual(0);
            buyer.CurrentPrice.ShouldEqual(50);
            buyer.NumberInStock.ShouldEqual(200);
            connectionMock.Verify(x => x.SendMessage("Command: BUY; Price: 50; Number: 10"));
        }
    }
}
