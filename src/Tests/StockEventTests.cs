using System;
using AutoBuyer.Logic.Domain;
using Should;
using Xunit;

namespace Tests
{
    public class StockEventTests
    {
        [Fact]
        public void Parses_close_event()
        {
            string inMessage = "Event: CLOSE;";

            StockEvent stockEvent = StockEvent.From(inMessage);
            string outMessage = stockEvent.ToString();

            stockEvent.Type.ShouldEqual(StockEventType.Close);
            outMessage.ShouldEqual(inMessage);
        }

        [Fact]
        public void Parses_price_event()
        {
            string inMessage = "Event: PRICE; CurrentPrice: 12; NumberInStock: 34;";

            StockEvent stockEvent = StockEvent.From(inMessage);
            string outMessage = stockEvent.ToString();

            stockEvent.Type.ShouldEqual(StockEventType.Price);
            stockEvent.CurrentPrice.ShouldEqual(12);
            stockEvent.NumberInStock.ShouldEqual(34);

            outMessage.ShouldEqual(inMessage);
        }

        [Fact]
        public void Parses_purchase_event()
        {
            string inMessage = "Event: PURCHASE; BuyerName: Buyer; NumberSold: 1;";

            StockEvent stockEvent = StockEvent.From(inMessage);
            string outMessage = stockEvent.ToString();

            stockEvent.Type.ShouldEqual(StockEventType.Purchase);
            stockEvent.BuyerName.ShouldEqual("Buyer");
            stockEvent.NumberSold.ShouldEqual(1);

            outMessage.ShouldEqual(inMessage);
        }

        [Fact]
        public void Does_not_parse_event_with_incorrect_format()
        {
            string inMessage = "incorrect message";

            Action action = () => StockEvent.From(inMessage);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Does_not_parse_event_with_unknown_type()
        {
            string inMessage = "Event: UNKNOWN;";

            Action action = () => StockEvent.From(inMessage);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Close_method_returns_a_close_event()
        {
            StockEvent stockEvent = StockEvent.Close();

            stockEvent.Type.ShouldEqual(StockEventType.Close);
            stockEvent.ToString().ShouldEqual("Event: CLOSE;");
        }

        [Fact]
        public void Price_method_returns_a_price_event()
        {
            StockEvent stockEvent = StockEvent.Price(10, 15);

            stockEvent.Type.ShouldEqual(StockEventType.Price);
            stockEvent.CurrentPrice.ShouldEqual(10);
            stockEvent.NumberInStock.ShouldEqual(15);
            stockEvent.ToString().ShouldEqual("Event: PRICE; CurrentPrice: 10; NumberInStock: 15;");
        }

        [Fact]
        public void Purchase_method_returns_a_purchase_event()
        {
            StockEvent stockEvent = StockEvent.Purchase("some user", 1);

            stockEvent.Type.ShouldEqual(StockEventType.Purchase);
            stockEvent.BuyerName.ShouldEqual("some user");
            stockEvent.NumberSold.ShouldEqual(1);
            stockEvent.ToString().ShouldEqual("Event: PURCHASE; BuyerName: some user; NumberSold: 1;");
        }
    }
}