using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hwasuboon
{
    
    public partial class Form1 : Form
    {
        int flag;
        class stockInfo
        {
            public string stockCode;
            public string stockName;
            public stockInfo(string code, string name)
            {
                this.stockCode = code;
                this.stockName = name;
            }
        }
        List<stockInfo> stockList;
        public Form1()
        {
            InitializeComponent();
            this.Enabled = false;
            axKHOpenAPI1.CommConnect();
            axKHOpenAPI1.OnEventConnect += onEventConnect;
            axKHOpenAPI1.OnReceiveTrData += onReceiveTrData;
            axKHOpenAPI1.OnReceiveRealData += onReceiveRealData;
            flag = 0;

            txtPassword.TextChanged += encryptPassword;

            btnBalanceCheck.Click += ButtonClicked;
            btnAddStock.Click += ButtonClicked;
            btnStockSearch.Click += ButtonClicked;
            btnListReset.Click += ButtonClicked;

            txtStockName.KeyDown += KeyDown;
            txtPassword.KeyDown += KeyDown;
            txtStocks.KeyDown += KeyDown;

            lstStockCode.SelectedIndexChanged += SelectedIndexChanged;
            lstStockName.SelectedIndexChanged += SelectedIndexChanged;

        }

        public void SelectedIndexChanged(object sender, EventArgs e )
        {
            if (sender.Equals(lstStockCode))
            {
                lstStockName.SelectedIndex = lstStockCode.SelectedIndex;
                txtStocks.Text = lstStockName.Text;
                ButtonClicked(btnStockSearch, null);
            }
            if (sender.Equals(lstStockName))
            {
                lstStockCode.SelectedIndex = lstStockName.SelectedIndex;
                txtStocks.Text = lstStockName.Text;
                ButtonClicked(btnStockSearch, null);
            }
        }
        public void onReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (e.sRealType == "주식체결")
            {
                 
            }
        }
        public void stockSearch(object sender, EventArgs e)
        {
            
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (sender.Equals(txtStockName))
            {
                if(e.KeyCode == Keys.Enter)
                {
                    ButtonClicked(btnAddStock, null);
                    txtStockName.Text = null;
                }
            }
            if (sender.Equals(txtStocks))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ButtonClicked(btnStockSearch, null);
                    txtStocks.Text = null;
                }
            }
            if (sender.Equals(txtPassword))
            {
                if (e.KeyCode == Keys.Enter)
                    ButtonClicked(btnBalanceCheck, null);
            }

        }
        public void ButtonClicked(object sender, EventArgs e)
        {
            if (sender.Equals(btnBalanceCheck))
            {
                if (txtPassword.Text == "") return;
                btnBalanceCheck.Enabled = false;
                if (cmbAccount.Text.Length > 0 && txtPassword.Text.Length > 0)
                {
                    
                    flag = 1;
                    this.Enabled = false; 
                }
                
            }
            if (sender.Equals(btnStockSearch))
            {

                flag = 2;
                this.Enabled = false;

            }
            if (sender.Equals(btnAddStock))
            {
                if (txtStockName.Text == "") return;
                string stockName = txtStockName.Text;
                int index = stockList.FindIndex(o => o.stockName == stockName);
                if (index == -1)
                {
                    MessageBox.Show("해당 종목이 존재하지 않습니다.");
                    return;
                }
                string stockCode = stockList[index].stockCode;
                if (lstStockCode.FindString(stockCode) == -1)
                {
                    lstStockCode.Items.Add(stockCode);
                    lstStockName.Items.Add(stockName);
                    lstStockCode.SelectedIndex = lstStockCode.Items.Count - 1;
                }
                else
                    lstStockCode.SelectedIndex = lstStockCode.FindString(stockCode);
                txtStocks.Text = txtStockName.Text;

            }
            if(sender.Equals(btnListReset))
            {
                txtStockName.Text = null;
                lstStockCode.Items.Clear();
                lstStockName.Items.Clear();
            }
        }
        public void onReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {

            if (e.sRQName == "계좌평가잔고내역요청")
            {
                long estimatedDepositedAssets = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "추정예탁자산"));
                long totalPurchase = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액"));
                long totalEstimate = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총평가금액"));
                long totalProfitLoss = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총평가손익금액"));
                double totalProfitRate = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총수익률(%)"));

                lblEstimatedDepositedAssets.Text = String.Format("{0:#,##0}", estimatedDepositedAssets);
                lblTotalPurchase.Text = String.Format("{0:#,##0}", totalPurchase);
                lblTotalEstimate.Text = String.Format("{0:#,##0}", totalEstimate);
                lblTotalProfit.Text = String.Format("{0:#,##0}", totalProfitLoss);
                lblTotalProfitRate.Text = String.Format("{0:f2}", totalProfitRate);
                btnBalanceCheck.Enabled = true;
            }
            else if (e.sRQName == "종목정보요청")
            {
                long stockPrice = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가").Trim().Replace("-", ""));
                string stockName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                long upDown = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "전일대비").Trim());
                long volume = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "거래량").Trim());
                string upDownRate = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "등락율").Trim();

                lblStockPrice.Text = String.Format("{0:#,##0}", stockPrice);

                lblStockName.Text = stockName;
                lblStockUpDown.Text = String.Format("{0:#,##0}", upDown);
                lblStockVolume.Text = String.Format("{0:#,##0}", volume);

                if (upDown > 0) { lblStockUpDown.ForeColor = Color.Red; lblStockUpDownRate.ForeColor = Color.Red; }
                else if (upDown < 0) { lblStockUpDown.ForeColor = Color.Blue; lblStockUpDownRate.ForeColor = Color.Blue; }
                else { lblStockUpDown.ForeColor = Color.Black; lblStockUpDownRate.ForeColor = Color.Black; }
             
                lblStockUpDownRate.Text = upDownRate + "%";
            }
        }
        public void encryptPassword(object sender, EventArgs e)
        {
            if (sender.Equals(txtPassword))
            {
                txtPassword.PasswordChar = '*';
                txtPassword.MaxLength = 4;
            }
        }
        public void onEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {

            stockList = new List<stockInfo>();

            if (e.nErrCode == 0)
            {
                string accountlist = axKHOpenAPI1.GetLoginInfo("ACCLIST");
                string[] account = accountlist.Split(';');

                cmbAccount.Items.Clear();
                for (int i = 0; i < account.Length; i++)
                    cmbAccount.Items.Add(account[i]);

                cmbAccount.SelectedIndex = 0;

                string userId = axKHOpenAPI1.GetLoginInfo("USER_ID");
                string userName = axKHOpenAPI1.GetLoginInfo("USER_NAME");
                string connectedServer = axKHOpenAPI1.GetLoginInfo("GetServerGubun");

                lblID.Text = userId;
                lblName.Text = userName;
                lblServer.Text = connectedServer;
                btnBalanceCheck.Enabled = true;
                cmbAccount.Enabled = true;
                txtPassword.Enabled = true;

                AutoCompleteStringCollection stockcollection = new AutoCompleteStringCollection();
                string stockCode = axKHOpenAPI1.GetCodeListByMarket(null);
                string[] stockCodeArray = stockCode.Split(';');
                for (int i = 0; i < stockCodeArray.Length; i++)
                {
                    stockList.Add(new stockInfo(stockCodeArray[i], axKHOpenAPI1.GetMasterCodeName(stockCodeArray[i])));
                }
                for (int i = 0; i < stockList.Count; i++)
                {
                    stockcollection.Add(stockList[i].stockName);
                }
                txtStocks.AutoCompleteCustomSource = stockcollection;
            }
            timer1.Enabled = true;
            this.Enabled = true;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                string accountNumber = cmbAccount.Text;
                string password = cmbAccount.Text;
                axKHOpenAPI1.SetInputValue("계좌번호", accountNumber);
                axKHOpenAPI1.SetInputValue("비밀번호", password);
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
                axKHOpenAPI1.SetInputValue("조회구분", "1");

                axKHOpenAPI1.CommRqData("계좌평가잔고내역요청", "opw00018", 0, "8100");
                flag = 0;
                this.Enabled = true;
            }
            else if(flag == 2)
            {
                if (txtStocks.Text == "") return;
                string searchStock = txtStocks.Text;
                int index = stockList.FindIndex(o => o.stockName == searchStock);

                if (index == -1)
                {
                    MessageBox.Show("해당 종목이 존재하지 않습니다.");
                    return;
                }
                string stockCode = stockList[index].stockCode;
                axKHOpenAPI1.SetInputValue("종목코드", stockCode);
                axKHOpenAPI1.CommRqData("종목정보요청", "opt10001", 0, "5000");
                flag = 0;
                this.Enabled = true;
            }
            else
            {
                
            }
        }
    }
}
