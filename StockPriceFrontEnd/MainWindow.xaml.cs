using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StockPrice;
using System.Threading;

namespace StockPriceFrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        decimal progress = 0m;
        string fileLocation = string.Empty;

        decimal Progress
        {
            set
            {
                progress = value;
                progressBar.Value = (double) value * 100;
                percentageLbl.Content = String.Format("{0:0.00}%", value * 100);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true)
            {
                fileLocationTxtBox.Text = dlg.FileName;
                fileLocation = dlg.FileName;
            }
        }

        async private void RunBtn_Click(object sender, RoutedEventArgs e)
        {

            await RunAsync();

        }

        public async Task<bool> RunAsync()
        {
            await Task.Run(() =>
            {
                RunAnalysis();
            });
            return true;
        }

        public bool RunAnalysis()
        {
            //UpdateLabel ul = new UpdateLabel(updateInfoLabel);

            updateInfoLabel("Creating stock dictionary");
            Dictionary<string, Stock> stocks = new Dictionary<string, Stock>();

            //string filePath = fileLocationTxtBox.Text;

            updateInfoLabel("Reading data from file");
            stocks = Reader.GetAllStockData(fileLocation);

            updateInfoLabel("Analyzing data");
            MarketHistoryAnalyzer.FillAllWithDefaults(stocks, SetProgress);

            updateInfoLabel("Saving to disk");
            StockState sc1 = new StockState(stocks);
            sc1.Serialize("stocksWithIndicators.bin");

            updateInfoLabel("Removing stocks not traded every day");
            List<Stock> tradedEveryday = StockComparer.RemoveStocksNotTradedEveryday(stocks);

            updateInfoLabel("Creating Stock Comparator");
            StockComparer sc = new StockComparer(tradedEveryday);

            updateInfoLabel("Running comparison");
            sc.RankStocksByCompare();

            updateInfoLabel("Saving to disk");
            StockState sc2 = new StockState(sc.RankedStocks);
            sc2.Serialize("rankedStocks.bin");

            updateInfoLabel("Emailing results");
            EmailNotifier en = new EmailNotifier("ff12sender", "33914047");
            en.Send(sc.RankedStocks);

            updateInfoLabel("All done. Check your email");

            return true;
        }

        public void SetProgress(decimal percent)
        {
            Dispatcher.Invoke(() =>
            {
                Progress = percent;
            },
            System.Windows.Threading.DispatcherPriority.Normal);
        }

        public delegate void UpdateLabel(string text);
        
        public void updateInfoLabel(string text)
        {
            Dispatcher.Invoke(() =>
            {
                additionalInfoLbl.Content = text;
            },
            System.Windows.Threading.DispatcherPriority.Normal);
        }

    }
}
