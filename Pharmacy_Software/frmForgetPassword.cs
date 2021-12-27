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
    public partial class frmForgetPassword : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmForgetPassword()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnGetPassword_Click(object sender, EventArgs e)
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username AND fullname = @fullname", cn);
            cm.Parameters.AddWithValue("@username", txtUsername.Text);
            cm.Parameters.AddWithValue("fullname", txtFullName.Text);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtPassword.Text = dr["password"].ToString();
            }
            else
            {
                cn.Close();
                txtPassword.Clear();
                MessageBox.Show("Invalid details provided", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;               
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
