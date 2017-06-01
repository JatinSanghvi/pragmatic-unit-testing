using System.Data;
using System.Data.SqlClient;

namespace Tests.Integration
{
    public abstract class Tests
    {
        internal const string ConnectionString =
            "Data Source=JASANGHV-LT;Initial Catalog=AutoBuyer;Integrated Security=True";

        public Tests()
        {
            ClearDatabase();
        }

        private void ClearDatabase()
        {
            string query = "DELETE FROM dbo.Buyer";

            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = new SqlCommand(query, connection)
                {
                    CommandType = CommandType.Text
                };

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
