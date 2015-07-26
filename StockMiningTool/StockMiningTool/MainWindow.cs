using Stock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
namespace StockMiningTool
{
    public partial class MainWindow : Form
    {
        RequestScheduler _requestScheduler = new RequestScheduler();
        Stock.StockMiner _stockMiner = new StockMiner();
        Stock.StockTrade _stockTrade = new Stock.StockTrade();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            _stockMiner.Initialize(_stockTrade, new SaveStockInfo());

            

            //try
            //{
            //    var stockTrade = new StockTrade();

            //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();//引用stopwatch物件
            //    sw.Reset();//碼表歸零
            //    sw.Start();//碼表開始計時
            //    /**************/

            //    Stock.StockTradeObject stockobject = stockTrade.GetStockInfoByDate(new DateTime(2014, 7, 24));
            //    IStockInfoDbService DBService = new StockInfoDbService();
            //    DBService.SaveAsync(stockobject.msgArray);
            //    /**************/
            //    //sw.Stop();//碼錶停止
            //    //印出所花費的總豪秒數
            //    var result1 = sw.Elapsed.TotalMilliseconds;
            //    Console.WriteLine(result1);
            //}
            //catch
            //{
            //}



        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_stockMiner.CanCreateRequst)
            {
                // 內部會自動判斷是否忙碌，實際 Start 就會自己擋下來
                if (!_requestScheduler.IsBusy)
                {
                    _requestScheduler.Start(_stockMiner);
                }
            }
            else
            {
                // 下午兩點半後在進行儲存
                if (_stockMiner.ExceptionList.Any())
                {
                    btnSaveExceptionList.PerformClick();
                    _stockMiner.ExceptionList.Clear();
                }
            }

            this.Text = string.Format("Is Mining: {0}, Last Save Time: {1}, Errors: {2}", _requestScheduler.IsBusy, _stockMiner.LastSaveStockInfoTime, _stockMiner.ExceptionList.Count);
        }

        private void btnStopAutoMining_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                _requestScheduler.Stop();
                timer1.Enabled = false;
                this.Text = "Stoped Mining " + DateTime.Now;
            }
        }

        private void btnStartAutoMining_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                timer1_Tick(null, null);
                timer1.Enabled = true;
            }
        }

        private void btnSaveExceptionList_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllLines(string.Format("Errors {0}.txt", DateTime.Now.ToString("yyyyMMdd HHmmss")),
                                          _stockMiner.ExceptionList.Select(item => item.Item1 + Environment.NewLine +
                                                                                   item.Item2.StackTrace + Environment.NewLine +
                                                                                   item.Item2.Message + Environment.NewLine));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = new Stock.Core.Domain.Models.Entities();

            try
            {
                db.Database.Connection.Open();
                //var company = db.Companies.First();
                //company.ModTime = DateTime.Now;
                //var result = db.SaveChanges();
                MessageBox.Show("Succeed");
            }
            catch
            {
                MessageBox.Show("Failed");
            }
            finally
            {
                db.Dispose();
            }


        }



    }
}
