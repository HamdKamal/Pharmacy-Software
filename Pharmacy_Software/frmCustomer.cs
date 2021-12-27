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
    public partial class frmCustomer : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmCustomer()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void Clear()
        {
            txtEmail.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtID.Text = "";
            txtAddress.Text = "";

        }

        public void GetID()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblcustomer", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            Random myrand = new Random();
            int mynum = myrand.Next(20, 1000);
            txtID.Text = "C-001" + num + mynum;
        }

        public void LoadRecord()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcustomer ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["cusid"].ToString(), dr["name"].ToString(), dr["phoneno"].ToString(), dr["email"].ToString(), dr["address"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                txtName.Focus();
                MessageBox.Show("Please provide a name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPhone.Text == "")
            {
                txtPhone.Focus();
                MessageBox.Show("Please provide a valid phone number!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtEmail.Text == "")
            {
                txtEmail.Focus();
                MessageBox.Show("Please provide a valid email address!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Save Customer Information?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("INSERT INTO tblcustomer(cusid,name,email,phoneno,address) VALUES(@sid,@name,@email,@phoneno,@address)", cn);
                cm.Parameters.AddWithValue("@sid", txtID.Text);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                cm.Parameters.AddWithValue("@phoneno", txtPhone.Text);
                cm.Parameters.AddWithValue("@email", txtEmail.Text);
                cm.Parameters.AddWithValue("@address", txtAddress.Text);
                cm.ExecuteNonQuery();
                cn.Close();
                LoadRecord();
                Clear();
                GetID();
                MessageBox.Show("Customer Information has been saved", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                txtName.Focus();
                MessageBox.Show("Please provide a name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPhone.Text == "")
            {
                txtPhone.Focus();
                MessageBox.Show("Please provide a valid phone number!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtEmail.Text == "")
            {
                txtEmail.Focus();
                MessageBox.Show("Please provide a valid email address!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Update Customer Information?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("UPDATE tblcustomer SET name = @name,phoneno = @phoneno,email = @email,address = @address WHERE cusid = @cusid", cn);
                cm.Parameters.AddWithValue("@cusid", txtID.Text);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                cm.Parameters.AddWithValue("@phoneno", txtPhone.Text);
                cm.Parameters.AddWithValue("@email", txtEmail.Text);
                cm.Parameters.AddWithValue("@address", txtAddress.Text);
                cm.ExecuteNonQuery();
                cn.Close();
                LoadRecord();
                Clear();
                GetID();
                MessageBox.Show("Customer Information has been updated", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (ColName == "ColEdit")
            {
                btnUpdate.Enabled = true;
                btnSave.Enabled = false;
                txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtPhone.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtEmail.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtAddress.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
            }
            else if (ColName == "ColDelete")
            {
                if (frmLogin.usertype == "Administrator")
                {
                    if (MessageBox.Show("Remove Customer?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cn.Open();
                        cm = new MySqlCommand("DELETE FROM tblcustomer WHERE cusid = @cusid", cn);
                        cm.Parameters.AddWithValue("@cusid", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                        cm.ExecuteNonQuery();
                        cn.Close();
                        LoadRecord();
                        GetID();
                        MessageBox.Show("Customer has been removed", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Access Denied", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcustomer WHERE name LIKE '%" + txtSearch.Text + "%' ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["cusid"].ToString(), dr["name"].ToString(), dr["phoneno"].ToString(), dr["email"].ToString(), dr["address"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtName.Focus();
            GetID();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
