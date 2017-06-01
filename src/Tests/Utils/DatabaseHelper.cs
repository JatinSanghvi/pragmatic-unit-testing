using AutoBuyer.Logic.Domain;
using Dapper;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.Utils
{
    internal static class DatabaseHelper
    {
        public static BuyerDto GetBuyer(string itemId)
        {
            using (var connection = new SqlConnection(Integration.Tests.ConnectionString))
            {
                string query = "SELECT * FROM dbo.Buyer WHERE ItemID = @ItemID";
                return connection.Query<BuyerDto>(query, new { ItemID = itemId }).Single();
            }
        }
    }

    public struct BuyerDto
    {
        public readonly string BuyerName;
        public readonly int MaximumPrice;
        public readonly int NumberToBuy;
        public readonly int CurrentPrice;
        public readonly int NumberInStock;
        public readonly int BoughtSoFar;
        public readonly BuyerState State;
    }
}
