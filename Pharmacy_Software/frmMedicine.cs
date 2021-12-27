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
    public partial class frmMedicine : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string id;

        public frmMedicine()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
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

        public void GetID()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblmedicine", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            txtID.Text = "M-001" + num;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cboCategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public void Clear()
        {
            txtBatchNo.Clear();
            txtID.Clear();
            txtName.Clear();
            cboCategory.Text = "";
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
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboCategory.Text == "")
            {
                MessageBox.Show("Please select category", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtID.Text == "")
            {
                txtID.Focus();
                MessageBox.Show("Medicine ID field is empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtName.Text == "")
            {
                txtName.Focus();
                MessageBox.Show("Please enter medicine name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtBatchNo.Text == "")
            {
                txtBatchNo.Focus();
                MessageBox.Show("Please enter batch no!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Save Medicine?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE batchid = @batchid", cn);
                cm.Parameters.AddWithValue("@batchid", txtID.Text);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Close();
                    cn.Close();
                    MessageBox.Show("Batch ID already existed!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                dr.Close();
                cn.Close();

                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE name = @name", cn);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Close();
                    cn.Close();
                    MessageBox.Show("Medicine name already existed!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                dr.Close();
                cn.Close();

                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE medid = @medid", cn);
                cm.Parameters.AddWithValue("@medid", txtID.Text);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Close();
                    cn.Close();
                    MessageBox.Show("Medicine ID already existed!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    dr.Close();
                    //cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblmedicine(category,medid,name,batchid,quantity,date1,date2,date3,price) VALUES(@category,@medid,@name,@batchid,@quantity,@date1,@date2,@date3,@price)", cn);
                    cm.Parameters.AddWithValue("@category", cboCategory.Text);
                    cm.Parameters.AddWithValue("@medid", txtID.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@batchid", txtBatchNo.Text);
                    cm.Parameters.AddWithValue("@quantity", "");
                    cm.Parameters.AddWithValue("@date1", "");
                    cm.Parameters.AddWithValue("@date2", "");
                    cm.Parameters.AddWithValue("@date3", "");
                    cm.Parameters.AddWithValue("@price", "");
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    Clear();
                    GetID();
                    MessageBox.Show("Medicine has been saved", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (ColName == "ColEdit")
            {
                btnUpdate.Enabled = true;
                btnSave.Enabled = false;
                cboCategory.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                id = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtBatchNo.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
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
                    GetID();
                    MessageBox.Show("Medicine has been removed successfully", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
            txtName.Focus();
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
            GetID();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cboCategory.Text == "")
            {
                MessageBox.Show("Please select category", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtID.Text == "")
            {
                txtID.Focus();
                MessageBox.Show("Medicine ID field is empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtName.Text == "")
            {
                txtName.Focus();
                MessageBox.Show("Please enter medicine name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtBatchNo.Text == "")
            {
                txtBatchNo.Focus();
                MessageBox.Show("Please enter batch no!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Update Medicine?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE name = @name", cn);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Close();
                    cn.Close();
                    MessageBox.Show("Batch ID already existed!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    dr.Close();
                    //cn.Open();
                    cm = new MySqlCommand("UPDATE tblmedicine SET category = @category,medid = @medid,name = @name,batchid = @batchid WHERE medid = '" + id + "'", cn);
                    cm.Parameters.AddWithValue("@category", cboCategory.Text);
                    cm.Parameters.AddWithValue("@medid", txtID.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@batchid", txtBatchNo.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    Clear();
                    GetID();
                    MessageBox.Show("Medicine has been updated", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
            }
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
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void txtSearch3_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE medid LIKE '" + txtSearch3.Text + "%' ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE category LIKE '" + cboCat.Text + "%' ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["category"].ToString(), dr["medid"].ToString(), dr["name"].ToString(), dr["batchid"].ToString());
            }
            dr.Close();
            cn.Close();
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
