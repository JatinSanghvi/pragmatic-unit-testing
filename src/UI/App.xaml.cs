using AutoBuyer.Logic.Connections;
using AutoBuyer.Logic.Database;

namespace AutoBuyer.UI
{
    public partial class App
    {
        private const string BuyerName = "Buyer";
        private const string ConnectionString =
            "Data Source=JASANGHV-LT;Initial Catalog=AutoBuyer;Integrated Security=True";

        public App()
        {
            var connection = new WarehouseConnection();
            var repository = new BuyerRepository(ConnectionString);
            var mainViewModel = new MainViewModel(BuyerName, connection, repository);

            var window = new MainWindow
            {
                DataContext = mainViewModel
            };
            window.ShowDialog();
        }
    }
}
