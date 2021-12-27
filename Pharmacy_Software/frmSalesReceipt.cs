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
using Microsoft.Reporting.WinForms;

namespace Pharmacy_Software
{
    public partial class frmSalesReceipt : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        MySqlDataAdapter da;
        ClassDB db = new ClassDB();

        string _Name, _Phone, _Email, _Address, _Website, _RegNo;

        public string invoiceid;

        public frmSalesReceipt()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void frmSalesReceipt_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
            LoadHeader();
            LoadReceipt();
        }

        public void LoadHeader()
        {
            ReportDataSource rptDS = new ReportDataSource();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsettings", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                _Name = dr["name"].ToString();
                _Phone = dr["phone"].ToString();
                _Website = dr["website"].ToString();
                _RegNo = dr["regno"].ToString();
                _Email = dr["email"].ToString();
                _Address = dr["address"].ToString();

                dr.Close();
                cn.Close();

                reportViewer1.LocalReport.ReportPath = Application.StartupPath + "/Reports/Receipt.rdlc";
                reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                da = new MySqlDataAdapter();


                cn.Open();
                da.SelectCommand = new MySqlCommand("SELECT * FROM tblsettings WHERE name = '" + _Name + "'", cn);
                da.Fill(ds, "dtName");
                cn.Close();

                ReportParameter pName = new ReportParameter("pName", _Name);
                ReportParameter pEmail = new ReportParameter("pEmail", _Email);
                ReportParameter pPhone = new ReportParameter("pPhone", _Phone);
                ReportParameter pWebsite = new ReportParameter("pWebsite", _Website);
                ReportParameter pRegNo = new ReportParameter("pRegNo", _RegNo);
                ReportParameter pAddress = new ReportParameter("pAddress", _Address);

                reportViewer1.LocalReport.SetParameters(pName);
                reportViewer1.LocalReport.SetParameters(pEmail);
                reportViewer1.LocalReport.SetParameters(pPhone);
                reportViewer1.LocalReport.SetParameters(pWebsite);
                reportViewer1.LocalReport.SetParameters(pRegNo);
                reportViewer1.LocalReport.SetParameters(pAddress);

                rptDS = new ReportDataSource("DataSet1", ("dtName"));
                reportViewer1.LocalReport.DataSources.Add(rptDS);
            }
            else
            {
                _Name = "";
                _Phone = "";
                _Email = "";
                _Address = "";
            }
            dr.Close();
            cn.Close();
        }

        public void LoadReceipt()
        {
            cn.Open();
            da = new MySqlDataAdapter("SELECT * FROM vwreceipt WHERE invoiceid = '" + invoiceid + "'", cn);
            DataSet1 ds = new DataSet1();
            da.Fill(ds, "dtReceipt");

            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[2]);

            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(datasource);
            this.reportViewer1.RefreshReport();
            cn.Close();
        }

    }
}
