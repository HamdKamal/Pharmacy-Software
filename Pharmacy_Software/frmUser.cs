using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace Pharmacy_Software
{
    public partial class frmUser : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmUser()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        public void LoadChart()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT date, SUM(grandtotal) AS total FROM tblsalespayment WHERE cashier = '" + frmLogin.username + "' AND date = '" + DateTime.Now.ToShortDateString() + "'  ORDER BY date ASC", cn);
            DataSet ds = new DataSet();

            da.Fill(ds, "Chart");
            chart1.DataSource = ds.Tables["Chart"];
            Series series1 = chart1.Series["Series1"];
            series1.ChartType = SeriesChartType.Column;

            series1.Name = "DAILY SALES";

            var chart = chart1;
            chart.Series[series1.Name].XValueMember = "date";
            chart.Series[series1.Name].YValueMembers = "total";

            chart.Series[0].IsValueShownAsLabel = true;
            //chart.Series[0].LegendText = "#VALX (#PERCENT)";
        }

        public void LoadChart2()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT date, grandtotal FROM tblsalespayment WHERE cashier = '" + frmLogin.username + "'", cn);
            DataSet ds = new DataSet();

            da.Fill(ds, "Chart");
            chart2.DataSource = ds.Tables["Chart"];
            Series series2 = chart2.Series["Series1"];
            series2.ChartType = SeriesChartType.Column;

            series2.Name = "SALES SUMMARY";

            var chart = chart2;
            chart.Series[series2.Name].XValueMember = "date";
            chart.Series[series2.Name].YValueMembers = "grandtotal";

            chart.Series[0].IsValueShownAsLabel = true;
            //chart.Series[0].LegendText = "#VALX (#PERCENT)";
        }

        public void GetCustomer()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblcustomer", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblCustomer.Text = num;
        }

        public void GetTodaySale()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(grandtotal) FROM tblsalespayment WHERE cashier = @cashier AND date = @date", cn);
            cm.Parameters.AddWithValue("@cashier", frmLogin.username);
            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            if (num != String.Empty)
            {
                lblTodaySales.Text = num;
            }
            else
            {
                lblTodaySales.Text = "0.00";
            }
        }

        public void GetAllTimeSale()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(grandtotal) FROM tblsalespayment WHERE cashier = @cashier", cn);
            cm.Parameters.AddWithValue("@cashier", frmLogin.username);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            if (num != String.Empty)
            {
                lblAllTimeSale.Text = num;
            }
            else
            {
                lblAllTimeSale.Text = "0.00";
            }
        }

        public void LoadUserPic()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username", cn);
            cm.Parameters.AddWithValue("@username", frmLogin.username);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                //byte[] data = (byte[])dr["picture"];
                //MemoryStream ms = new MemoryStream(data);
                //picUser.Image = Image.FromStream(ms);

                lblFullName.Text = dr["fullname"].ToString();
            }
            else
            {
                picUser.Image = picUser.InitialImage;
            }
            dr.Close();
            cn.Close();
        }

        private void frmUser_Load(object sender, EventArgs e)
        {
            LoadChart();
            LoadChart2();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToLongTimeString();

            GetCustomer();
            GetAllTimeSale();
            GetTodaySale();
            LoadUserPic();
        }

        public void LoadUser()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username", cn);
            cm.Parameters.AddWithValue("@username", frmLogin.username);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblFullName.Text = dr["fullname"].ToString();
                lblUsertype.Text = dr["usertype"].ToString();

                //byte[] data = (byte[])dr["picture"];
                //MemoryStream ms = new MemoryStream(data);
                //picUser.Image = Image.FromStream(ms);
            }
            else
            {
                lblFullName.Text = "";
                lblUsertype.Text = "";

                picUser.Image = picUser.InitialImage;
            }
            dr.Close();
            cn.Close();
        }

        private void InsertLogSuccess()
        {
            cn.Open();
            cm = new MySqlCommand("INSERT INTO tbllogs(username,date,time,operation) VALUES(@username,@date,@time,@operation)", cn);
            cm.Parameters.AddWithValue("@username", frmLogin.username);
            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
            cm.Parameters.AddWithValue("@operation", "Successfully logged out!");
            cm.ExecuteNonQuery();
            cn.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Logout? Click yes to confirm!", "LOGGING OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                InsertLogSuccess();
                this.Hide();
                var f1 = new frmLogin();
                f1.Show();
            }
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            var f1 = new frmCustomer();
            f1.btnUpdate.Enabled = false;
            f1.GetID();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            var f1 = new frmSupplier();
            f1.btnUpdate.Enabled = false;
            f1.GetID();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            var f1 = new frmPOS();
            f1.GetID();
            f1.GetCategory();
            f1.GetCustomer();
            f1.ShowDialog();
        }

        private void btnStockList_Click(object sender, EventArgs e)
        {
            var f1 = new frmStockList();
            f1.GetCategory();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            var f1 = new frmSalesRecord();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            var f1 = new frmProfile();
            f1.LoadUserDetails();
            f1.ShowDialog();
        }

        private void btndashboard_Click(object sender, EventArgs e)
        {

        }

        private void btndashboard_MouseHover(object sender, EventArgs e)
        {
            btndashboard.ForeColor = SystemColors.GrayText;
        }

        private void btnPOS_MouseHover(object sender, EventArgs e)
        {
            btnPOS.ForeColor = SystemColors.GrayText;
        }

        private void btnSupplier_MouseHover(object sender, EventArgs e)
        {
            btnSupplier.ForeColor = SystemColors.GrayText;
        }

        private void btnCustomer_MouseHover(object sender, EventArgs e)
        {
            btnCustomer.ForeColor = SystemColors.GrayText;
        }

        private void btnStockList_MouseHover(object sender, EventArgs e)
        {
            btnStockList.ForeColor = SystemColors.GrayText;
        }

        private void btnSales_MouseHover(object sender, EventArgs e)
        {
            btnSales.ForeColor = SystemColors.GrayText;
        }

        private void btnProfile_MouseHover(object sender, EventArgs e)
        {
            btnProfile.ForeColor = SystemColors.GrayText;
        }

        private void btnLogout_MouseHover(object sender, EventArgs e)
        {
            btnLogout.ForeColor = SystemColors.GrayText;
        }

        private void btndashboard_MouseLeave(object sender, EventArgs e)
        {
            btndashboard.ForeColor = Color.White;
        }

        private void btnPOS_MouseLeave(object sender, EventArgs e)
        {
            btnPOS.ForeColor = Color.White;
        }

        private void btnSupplier_MouseLeave(object sender, EventArgs e)
        {
            btnSupplier.ForeColor = Color.White;
        }

        private void btnCustomer_MouseLeave(object sender, EventArgs e)
        {
            btnCustomer.ForeColor = Color.White;
        }

        private void btnStockList_MouseLeave(object sender, EventArgs e)
        {
            btnStockList.ForeColor = Color.White;
        }

        private void btnSales_MouseLeave(object sender, EventArgs e)
        {
            btnSales.ForeColor = Color.White;
        }

        private void btnProfile_MouseLeave(object sender, EventArgs e)
        {
            btnProfile.ForeColor = Color.White;
        }

        private void btnLogout_MouseLeave(object sender, EventArgs e)
        {
            btnLogout.ForeColor = Color.White;
        }
    }
}
