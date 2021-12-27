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

namespace Pharmacy_Software
{
    public partial class frmPreview : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public string _id;

        public frmPreview()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadDetails()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM vwpreview WHERE medid = @medid", cn);
            cm.Parameters.AddWithValue("@medid", _id);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblMedID.Text = dr["medid"].ToString();
                lblCategory.Text = dr["category"].ToString();
                lblMedicineName.Text = dr["name"].ToString();
                lblbatchID.Text = dr["quantity"].ToString();
                lblDateAdded.Text = dr["date1"].ToString();
                lblDateManufactured.Text = dr["date2"].ToString();
                lblExpired.Text = dr["date3"].ToString();
                lblPrice.Text = dr["price"].ToString();
                lblSupplierName.Text = dr["suppliername"].ToString();
            }
            else
            {
                lblMedID.Text = "";
                lblCategory.Text = "";
                lblMedicineName.Text = "";
                lblbatchID.Text = "";
                lblDateAdded.Text = "";
                lblDateManufactured.Text = "";
                lblExpired.Text = "";
                lblPrice.Text = "";
                lblSupplierName.Text = "";
            }
            dr.Close();
            cn.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
