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
    public partial class frmLogs : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmLogs()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadRecord()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbllogs", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["operation"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void GetUser()
        {
            cboUserID.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboUserID.Items.Add(dr["username"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void cboUserID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cboUserID_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbllogs WHERE username = @username", cn);
            cm.Parameters.AddWithValue("@username", cboUserID.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["operation"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbllogs WHERE date BETWEEN @d1 AND @d2", cn);
            cm.Parameters.AddWithValue("@d1", dtpFrom.Text);
            cm.Parameters.AddWithValue("@d2", dtpTo.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["operation"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all records?, Click Yes to Confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("DELETE FROM tbllogs", cn);
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("All records has been deleted!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
