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
using System.IO;

namespace Pharmacy_Software
{
    public partial class frmUserManagement : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmUserManagement()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void InsertLogSuccess()
        {
            cn.Open();
            cm = new MySqlCommand("INSERT INTO tbllogs(username,date,time,operation) VALUES(@username,@date,@time,@operation)", cn);
            cm.Parameters.AddWithValue("@username", frmLogin.username);
            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
            cm.Parameters.AddWithValue("@operation", "added a new user with id: " + txtUsername.Text);
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
            cm.Parameters.AddWithValue("@operation", "updated a user details with id " + txtUsername.Text);
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
            cm.Parameters.AddWithValue("@operation", "deleted a user with id " + txtUsername.Text);
            cm.ExecuteNonQuery();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtFullName.Text == "" || txtPassword.Text == "" || txtCPassword.Text == "" || cboUsertype.Text == "" || cboStatus.Text == "")
            {
                MessageBox.Show("Please, all field are required!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPassword.Text != txtCPassword.Text)
            {
                MessageBox.Show("Password does not match!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Save User Information?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    dr.Close();
                    cn.Close();
                    MessageBox.Show("Username already taken!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(picUser.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tbluser(username,fullname,password,picture,usertype,status) VALUES(@username,@fullname,@password,@picture,@usertype,@status)", cn);
                    cm.Parameters.AddWithValue("@username", txtUsername.Text);
                    cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                    cm.Parameters.AddWithValue("@password", txtPassword.Text);
                    cm.Parameters.AddWithValue("@picture", bytImage);
                    cm.Parameters.AddWithValue("@usertype", cboUsertype.Text);
                    cm.Parameters.AddWithValue("@status", cboStatus.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    InsertLogSuccess();
                    Clear();
                    MessageBox.Show("User Information has been saved", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
            }
        }

        public void Clear()
        {
            txtCPassword.Clear();
            txtFullName.Clear();
            txtPassword.Clear();
            txtUsername.Clear();
            cboStatus.Text = "";
            cboUsertype.Text = "";

            picUser.Image = picUser.InitialImage;
        }

        public void LoadRecord()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser ORDER BY username ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["fullname"].ToString(), dr["usertype"].ToString(), dr["status"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "image files(*.bmp;*.jpg;*.png;*.gif)|*.bmp*;*.jpg;*.png;*.gif;";

            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                picUser.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            picUser.Image = picUser.InitialImage;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser WHERE fullname LIKE '%" + txtSearch.Text + "%' ORDER BY username ASC", cn);
            dr = cm.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["fullname"].ToString(), dr["usertype"].ToString(), dr["status"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser WHERE username LIKE '%" + txtSearch2.Text + "%' ORDER BY username ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["fullname"].ToString(), dr["usertype"].ToString(), dr["status"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
            txtUsername.ReadOnly = false;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtUsername.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtFullName.Text == "" || txtPassword.Text == "" || txtCPassword.Text == "" || cboUsertype.Text == "" || cboStatus.Text == "")
            {
                MessageBox.Show("Please, all field are required!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPassword.Text != txtCPassword.Text)
            {
                MessageBox.Show("Password does not match!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Save User Information?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(picUser.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tbluser SET username = @username,fullname = @fullname,password = @password,picture = @picture,usertype = @usertype,status = @status WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@username", txtUsername.Text);
                    cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                    cm.Parameters.AddWithValue("@password", txtPassword.Text);
                    cm.Parameters.AddWithValue("@picture", bytImage);
                    cm.Parameters.AddWithValue("@usertype", cboUsertype.Text);
                    cm.Parameters.AddWithValue("@status", cboStatus.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    UpdateLogSuccess();
                    Clear();
                    MessageBox.Show("User Information has been updated", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(picUser.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tbluser SET username = @username,fullname = @fullname,password = @password,picture = @picture,usertype = @usertype,status = @status WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@username", txtUsername.Text);
                    cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                    cm.Parameters.AddWithValue("@password", txtPassword.Text);
                    cm.Parameters.AddWithValue("@picture", bytImage);
                    cm.Parameters.AddWithValue("@usertype", cboUsertype.Text);
                    cm.Parameters.AddWithValue("@status", cboStatus.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    UpdateLogSuccess();
                    Clear();
                    MessageBox.Show("User Information has been updated", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (ColName == "ColEdit")
            {
                btnUpdate.Enabled = true;
                btnSave.Enabled = false;
                string username = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", username);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    txtUsername.ReadOnly = true;
                    txtUsername.Text = dr["username"].ToString();
                    txtFullName.Text = dr["fullname"].ToString();
                    txtPassword.Text = dr["password"].ToString();
                    txtCPassword.Text = dr["password"].ToString();
                    cboUsertype.Text = dr["usertype"].ToString();
                    cboStatus.Text = dr["status"].ToString();

                    byte[] data = (byte[])dr["picture"];
                    MemoryStream ms = new MemoryStream(data);
                    picUser.Image = Image.FromStream(ms);
                }
                else
                {
                    txtUsername.Text = "";
                    txtFullName.Text = "";
                    txtPassword.Text = "";
                    txtCPassword.Text = "";
                    cboUsertype.Text = "";
                    cboStatus.Text = "";

                    picUser.Image = picUser.InitialImage;
                }
                dr.Close();
                cn.Close();
            }
            else if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove User?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tbluser WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@username", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                    DeleteLogSuccess();
                    MessageBox.Show("User has been removed", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cboUsertype_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cboStatus_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
