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
    public partial class frmCategory : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string updatename;

        public frmCategory()
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
            cm = new MySqlCommand("SELECT * FROM tblcategory ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["name"].ToString(), dr["description"].ToString());
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
            cm.Parameters.AddWithValue("@operation", "added a new category name having " + txtName.Text);
            cm.ExecuteNonQuery();
            cn.Close();
        }

        private void UpdateLogSuccess()
        {
            cn.Open();
            cm = new MySqlCommand("INSERT INTO tbllogs(username,date,time,operation) VALUES(@username,@date,@time,@operation)", cn);
            cm.Parameters.AddWithValue("@username", frmLogin.username);
            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
            cm.Parameters.AddWithValue("@operation", "updated a category name having " + updatename + " to " + txtName.Text);
            cm.ExecuteNonQuery();
            cn.Close();
        }

        private void DeleteLogSuccess()
        {
            cn.Open();
            cm = new MySqlCommand("INSERT INTO tbllogs(username,date,time,operation) VALUES(@username,@date,@time,@operation)", cn);
            cm.Parameters.AddWithValue("@username", frmLogin.username);
            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
            cm.Parameters.AddWithValue("@operation", "deleted a category name having " + updatename);
            cm.ExecuteNonQuery();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Please enter a category name", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Save Category?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblcategory WHERE name = @name", cn);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Close();
                    cn.Close();
                    MessageBox.Show("Name already existed!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    dr.Close();
                    //cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblcategory(name, description) VALUES(@name,@description)", cn);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@description", txtDescription.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    InsertLogSuccess();
                    txtName.Clear();
                    txtDescription.Clear();
                    MessageBox.Show("Category has been saved", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                txtName.Focus();
                MessageBox.Show("Please enter a category name", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Update Category?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblcategory SET name = @name, description = @description WHERE name = '" + updatename + "'", cn);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@description", txtDescription.Text);
                    cm.ExecuteNonQuery();

                    cm = new MySqlCommand("UPDATE tblmedicine SET category = @category WHERE category = '" + updatename + "'", cn);
                    cm.Parameters.AddWithValue("@category", txtName.Text);
                    cm.ExecuteNonQuery();

                    cn.Close();
                    LoadRecord();
                    UpdateLogSuccess();
                    txtName.Clear();
                    txtDescription.Clear();
                    MessageBox.Show("Category has been updated", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtDescription.Clear();
            txtName.Focus();
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (ColName == "ColEdit")
            {
                btnUpdate.Enabled = true;
                btnSave.Enabled = false;
                txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtDescription.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                updatename = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
            else if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Category?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblcategory WHERE name = @name", cn);
                    cm.Parameters.AddWithValue("@name", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    DeleteLogSuccess();
                    MessageBox.Show("Category has been removed", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcategory WHERE name LIKE '%" + txtSearch.Text + "%'", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["name"].ToString(), dr["description"].ToString());
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
