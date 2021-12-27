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
    public partial class frmAdjustQuantity : Form
    {

        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmAdjustQuantity()
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
            cm = new MySqlCommand("SELECT * FROM tblmedicine ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString(), dr["quantity"].ToString(), dr["date1"].ToString(), dr["date2"].ToString(), dr["date3"].ToString(), dr["price"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtPrice.Text == "" || txtPrice.Text == "0.00" || txtPrice.Text == "0")
            {
                txtPrice.Focus();
                MessageBox.Show("Please enter a valid price!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtNewQuantity.Text == "" || txtNewQuantity.Text == "0")
            {
                txtNewQuantity.Focus();
                MessageBox.Show("Please enter a valid quantity!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Update Quantity?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblmedicine SET date1 = @date1,date2 = @date2,date3 = @date3,quantity = quantity + @quantity,price = @price WHERE medid = @medid", cn);
                    cm.Parameters.AddWithValue("@medid", txtID.Text);
                    cm.Parameters.AddWithValue("@date1", dateAdded.Text);
                    cm.Parameters.AddWithValue("@date2", dateManufactured.Text);
                    cm.Parameters.AddWithValue("@date3", dateExpired.Text);
                    cm.Parameters.AddWithValue("@quantity", txtNewQuantity.Text);
                    cm.Parameters.AddWithValue("@price", txtPrice.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    Clear();
                    txtNewQuantity.Text = "0";
                    txtPrice.Text = "0.00";
                    txtQuantity.Text = "0";
                    MessageBox.Show("Quantity has been updated", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
        }

        private void Clear()
        {
            txtBatchNo.Clear();
            txtID.Clear();
            txtName.Clear();
            txtNewQuantity.Text = "0";
            txtPrice.Text = "0.00";
            txtQuantity.Text = "0";
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNewQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
            txtNewQuantity.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE name LIKE '%" + txtSearch.Text + "%' ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString(), dr["quantity"].ToString(), dr["date1"].ToString(), dr["date2"].ToString(), dr["date3"].ToString(), dr["price"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void GetCategory()
        {
            cboCat.Items.Clear();
            cboCategory.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcategory", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboCat.Items.Add(dr["name"].ToString());
                cboCategory.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE category = @category ORDER BY name ASC", cn);
            cm.Parameters.AddWithValue("@category", cboCat.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString(), dr["quantity"].ToString(), dr["date1"].ToString(), dr["date2"].ToString(), dr["date3"].ToString(), dr["price"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (ColName == "ColEdit")
            {
                cboCategory.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtBatchNo.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtQuantity.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                dateAdded.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                dateManufactured.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
                dateExpired.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();
                txtPrice.Text = dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString();
            }
            else if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Medicine?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblmedicine WHERE medid = @medid", cn);
                    cm.Parameters.AddWithValue("@medid", dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    MessageBox.Show("Medicine has been removed successfully", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
