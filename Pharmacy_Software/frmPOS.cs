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
    public partial class frmPOS : Form
    {
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public string medid;

        public string getholdid;

        public decimal discount, total, result;
        public frmPOS()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                this.Dispose();
        }

        public void GetID()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(DISTINCT(invoiceid)) FROM tblcart2", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            Random myrand = new Random();
            int mynum = myrand.Next(20, 1000);

            lblInvoiceNo.Text = "IVC" + DateTime.Now.Year + "10-" + num + mynum;
        }

        public void GetCategory()
        {
            cboCategory.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcategory", cn);
            dr = cm.ExecuteReader();          
            while (dr.Read())
            {               
                cboCategory.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void GetCustomer()
        {       
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcustomer", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                
                cboCustomer.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void txtInStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
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

        private void txtAmountPaid_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cboPaymentMode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtChange_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (cboCustomer.Text == "WALK IN CUSTOMER")
            {
                txtCustomerPhone.Text = "0000-000-0000";
            }
            else
            {
                cn.Close();
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblcustomer WHERE name = @name", cn);
                cm.Parameters.AddWithValue("@name", cboCustomer.Text);
                dr = cm.ExecuteReader();               
                dr.Read();
                if (dr.HasRows)
                {
                    txtCustomerPhone.Text = dr["phoneno"].ToString();
                }
                dr.Close();
                cn.Close();
            }
        }

        private void GetRemainingStock()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE medid = @medid", cn);
            cm.Parameters.AddWithValue("@medid", txtMedID.Text);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtInStock.Text = dr["quantity"].ToString();
            }
            dr.Close();
            cn.Close();
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboMedicine.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE category = @category", cn);
            cm.Parameters.AddWithValue("@category", cboCategory.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboMedicine.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboMedicine_SelectedIndexChanged(object sender, EventArgs e)
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblmedicine WHERE name = @name", cn);
            cm.Parameters.AddWithValue("@name", cboMedicine.Text);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtMedID.Text = dr["medid"].ToString();
                txtBatchID.Text = dr["batchid"].ToString();
                txtInStock.Text = dr["quantity"].ToString();
                txtPrice.Text = dr["price"].ToString();
            }
            dr.Close();
            cn.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboCategory.Text == "")
            {
                MessageBox.Show("Please select a valid category name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cboMedicine.Text == "")
            {
                MessageBox.Show("Please select a valid medicine name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtQuantity.Text == "" || Convert.ToInt16(txtQuantity.Text) < 1)
            {
                txtQuantity.Focus();
                MessageBox.Show("Please enter a valid quantity!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPrice.Text == "" || Convert.ToDecimal(txtPrice.Text) < 1)
            {
                txtPrice.Focus();
                MessageBox.Show("Selling Price can't be zero or less than zero!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtInStock.Text == "" || Convert.ToDecimal(txtInStock.Text) < 1)
            {
                txtInStock.Focus();
                MessageBox.Show("You are out of Stock!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add to Cart?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Close();
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblcart2 WHERE invoiceid = @invoiceid AND medid = @medid", cn);
                cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                cm.Parameters.AddWithValue("@medid", txtMedID.Text);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {
                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblcart2 SET quantity = quantity + @quantity ,invoicetotal = quantity * price WHERE invoiceid = @invoiceid AND medid = @medid", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@medid", txtMedID.Text);
                    cm.Parameters.AddWithValue("@quantity", Convert.ToInt32(txtQuantity.Text));
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblmedicine SET quantity = quantity - @quantity WHERE medid = @medid", cn);
                    cm.Parameters.AddWithValue("@medid", txtMedID.Text);
                    cm.Parameters.AddWithValue("@quantity", txtQuantity.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                    GetTotal();
                    GetRemainingStock();
                    txtQuantity.Text = "0";
                }
                else
                {
                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblcart2(invoiceid,medid,medname,quantity,price,invoicetotal,status) VALUES(@invoiceid,@medid,@medname,@quantity,@price,@invoicetotal,status)", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@medid", txtMedID.Text);
                    cm.Parameters.AddWithValue("@medname", cboMedicine.Text);
                    cm.Parameters.AddWithValue("@quantity", Convert.ToInt32(txtQuantity.Text));
                    cm.Parameters.AddWithValue("@price", Convert.ToDecimal(txtPrice.Text));
                    cm.Parameters.AddWithValue("@invoicetotal", Convert.ToDecimal(txtPrice.Text) * Convert.ToInt32(txtQuantity.Text));
                    cm.Parameters.AddWithValue("@status", "Pending");
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblmedicine SET quantity = quantity - @quantity WHERE medid = @medid", cn);
                    cm.Parameters.AddWithValue("@medid", txtMedID.Text);
                    cm.Parameters.AddWithValue("@quantity", txtQuantity.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                    GetTotal();
                    GetRemainingStock();
                    txtQuantity.Text = "0";
                }
                dr.Close();
                cn.Close();
            }

        }

        public void GetTotal()
        {
            if (dataGridView1.Rows.Count < 1)
            {
                lblTotal.Text = "0.00";
                lblGrandTotal.Text = "0.00";
            }
            else
            {
                cn.Open();
                cm = new MySqlCommand("SELECT SUM(invoicetotal) FROM tblcart2 WHERE invoiceid = @invoiceid", cn);
                cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                string num = cm.ExecuteScalar().ToString();
                cn.Close();

                lblTotal.Text = num;
                lblGrandTotal.Text = num;
            }
        }

        public void LoadCart()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcart2 WHERE invoiceid = @invoiceid ORDER BY medname ASC", cn);
            cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["medid"].ToString(), dr["medname"].ToString(), dr["quantity"].ToString(), dr["price"].ToString(), dr["invoicetotal"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboCategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cboMedicine_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            medid = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            string qty = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Item?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblcart2 WHERE medid = @medid", cn);
                    cm.Parameters.AddWithValue("@medid", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblmedicine SET quantity = quantity + @quantity WHERE medid = @medid", cn);
                    cm.Parameters.AddWithValue("@medid", medid);
                    cm.Parameters.AddWithValue("@quantity", qty);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                    GetTotal();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboCustomer.Text == "")
            {
                cboCustomer.Focus();
                MessageBox.Show("Please select a valid customer", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtAmountPaid.Text == "")
            {
                txtAmountPaid.Focus();
                MessageBox.Show("Amount paid field is cannot be empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Convert.ToDecimal(txtAmountPaid.Text) < Convert.ToDecimal(lblGrandTotal.Text))
            {
                txtAmountPaid.Focus();
                MessageBox.Show("Cannot proceed with transaction! \n Insufficient Amount Paid!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboPaymentMode.Text == "")
            {
                cboPaymentMode.Focus();
                MessageBox.Show("Please select a valid mode of payment!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtAmountPaid.Text == "0.00" || Convert.ToDecimal(txtAmountPaid.Text) < 1)
            {
                txtAmountPaid.Focus();
                MessageBox.Show("Please enter a valid amount to pay!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.Rows.Count < 1)
            {
                lblTotal.Text = "0.00";
                lblGrandTotal.Text = "0.00";
                txtAmountPaid.Text = "0.00";
                txtChange.Text = "0.00";
                txtDiscount.Text = "0.00";
                MessageBox.Show("Cart is Empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Save Transaction?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblsalespayment WHERE invoiceid = @invoiceid", cn);
                cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    decimal num = Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(lblGrandTotal.Text);
                    txtChange.Text = num.ToString();

                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblsalespayment SET cusname = @cusname,cusphone = @cusphone,saletotal = @saletotal,grandtotal = @grandtotal,amountpaid = @amountpaid,schange = @schange,discount = @discount,paymode = @paymode,status = @status,date = @date,time = @time WHERE invoiceid = @invoiceid", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@cusname", cboCustomer.Text);
                    cm.Parameters.AddWithValue("@cusphone", txtCustomerPhone.Text);
                    cm.Parameters.AddWithValue("@saletotal", lblTotal.Text);
                    cm.Parameters.AddWithValue("@grandtotal", lblGrandTotal.Text);
                    cm.Parameters.AddWithValue("@amountpaid", txtAmountPaid.Text);
                    cm.Parameters.AddWithValue("@schange", txtChange.Text);
                    cm.Parameters.AddWithValue("@discount", txtDiscount.Text);
                    cm.Parameters.AddWithValue("@paymode", cboPaymentMode.Text);
                    cm.Parameters.AddWithValue("@status", "Settled");
                    cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                    cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblcart2 SET status = @status WHERE invoiceid = @invoiceid", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@status", "Settled");
                    cm.ExecuteNonQuery();
                    cn.Close();

                    var f1 = new frmSalesReceipt();
                    f1.invoiceid = this.lblInvoiceNo.Text;
                    f1.ShowDialog();

                    GetID();
                    LoadCart();
                    lblTotal.Text = "0.00";
                    lblGrandTotal.Text = "0.00";
                    txtAmountPaid.Text = "0.00";
                    txtChange.Text = "0.00";
                    txtDiscount.Text = "0.00";
                    txtQuantity.Text = "0";
                    txtPrice.Text = "0.00";
                    cboCustomer.Text = "";
                    txtCustomerPhone.Text = "";
                    txtBatchID.Clear();
                    cboMedicine.Text = "";
                    txtMedID.Clear();
                    txtInStock.Text = "0.00";
                    MessageBox.Show("Sales has been saved succesfully!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    decimal num = Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(lblGrandTotal.Text);
                    txtChange.Text = num.ToString();

                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblsalespayment(invoiceid,cusname,cusphone,saletotal,grandtotal,amountpaid,schange,discount,paymode,status,cashier,date,time) VALUES(@invoiceid,@cusname,@cusphone,@saletotal,@grandtotal,@amountpaid,@schange,@discount,@paymode,@status,@cashier,@date,@time)", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@cusname", cboCustomer.Text);
                    cm.Parameters.AddWithValue("@cusphone", txtCustomerPhone.Text);
                    cm.Parameters.AddWithValue("@saletotal", lblTotal.Text);
                    cm.Parameters.AddWithValue("@grandtotal", lblGrandTotal.Text);
                    cm.Parameters.AddWithValue("@amountpaid", txtAmountPaid.Text);
                    cm.Parameters.AddWithValue("@schange", txtChange.Text);
                    cm.Parameters.AddWithValue("@discount", txtDiscount.Text);
                    cm.Parameters.AddWithValue("@paymode", cboPaymentMode.Text);
                    cm.Parameters.AddWithValue("@status", "Settled");
                    cm.Parameters.AddWithValue("@cashier", frmLogin.username.ToUpper());
                    cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                    cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblcart2 SET status = @status WHERE invoiceid = @invoiceid", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@status", "Settled");
                    cm.ExecuteNonQuery();
                    cn.Close();

                    var f1 = new frmSalesReceipt();
                    f1.invoiceid = this.lblInvoiceNo.Text;
                    f1.ShowDialog();

                    GetID();
                    LoadCart();
                    lblTotal.Text = "0.00";
                    lblGrandTotal.Text = "0.00";
                    txtAmountPaid.Text = "0.00";
                    txtChange.Text = "0.00";
                    txtDiscount.Text = "0.00";
                    txtQuantity.Text = "0";
                    txtPrice.Text = "0.00";
                    cboCustomer.Text = "";
                    txtCustomerPhone.Text = "";
                    txtBatchID.Clear();
                    cboMedicine.Text = "";
                    txtMedID.Clear();
                    txtInStock.Text = "0.00";
                    MessageBox.Show("Sales has been saved succesfully!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
            }
        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {
            var gettotal = Convert.ToDecimal(lblTotal.Text);
            var discount = Convert.ToDecimal(txtDiscount.Text);
            var total = Convert.ToDecimal(lblTotal.Text);

            if (Convert.ToDecimal(txtDiscount.Text) < 0 && Convert.ToDecimal(txtDiscount.Text) > 100)
            {
                MessageBox.Show("Discount can't be less than 0 or greater than 100", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = ((discount / 100) * total);

            var i = gettotal - result;
            lblGrandTotal.Text = i.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToLongTimeString();
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            var f1 = new frmReprint();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtInvoiceSearch.Text == String.Empty)
            {
                txtInvoiceSearch.Focus();
                txtInvoiceSearch.BackColor = Color.BlanchedAlmond;
                MessageBox.Show("Search box can't be empty", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                GetCartData();
                GetCustomerData();                              
            }
        }

        private void GetCustomerData()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsalespayment WHERE invoiceid = @invoiceid AND status = @status", cn);
            cm.Parameters.AddWithValue("@invoiceid", txtInvoiceSearch.Text);
            cm.Parameters.AddWithValue("@status", "Settled");
            dr = cm.ExecuteReader();
            dr.Read();
            if(dr.HasRows)
            {
                txtAmountPaid.Text = dr["amountpaid"].ToString();
                lblGrandTotal.Text = dr["grandtotal"].ToString();
                lblTotal.Text = dr["saletotal"].ToString();
                cboCustomer.Text = dr["cusname"].ToString();
                txtCustomerPhone.Text = dr["cusphone"].ToString();
                txtChange.Text = dr["schange"].ToString();
                txtDiscount.Text = dr["discount"].ToString();
                cboPaymentMode.Text = dr["paymode"].ToString();
                lblInvoiceNo.Text = dr["invoiceid"].ToString();
            }
            else
            {
                cn.Close();
                MessageBox.Show("Transaction with Invoice No: " + txtInvoiceSearch.Text + " not found", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dr.Close();
            cn.Close();
        }

        private void GetCartData()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcart2 WHERE invoiceid = @invoiceid AND status = @status", cn);
            cm.Parameters.AddWithValue("@invoiceid", txtInvoiceSearch.Text);
            cm.Parameters.AddWithValue("@status", "Settled");
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["medid"].ToString(), dr["medname"].ToString(), dr["quantity"].ToString(), dr["price"].ToString(), dr["invoicetotal"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            //if (cboCustomer.Text == "")
            //{
            //    cboCustomer.Focus();
            //    MessageBox.Show("Please select a valid customer", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (txtAmountPaid.Text == "")
            //{
            //    txtAmountPaid.Focus();
            //    MessageBox.Show("Amount paid field is cannot be empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (cboPaymentMode.Text == "")
            //{
            //    cboPaymentMode.Focus();
            //    MessageBox.Show("Please select a valid mode of payment!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (txtAmountPaid.Text == "0.00" || Convert.ToDecimal(txtAmountPaid.Text) < 1)
            //{
            //    txtAmountPaid.Focus();
            //    MessageBox.Show("Please enter a valid amount to pay!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (Convert.ToDecimal(txtAmountPaid.Text) < Convert.ToDecimal(lblGrandTotal.Text))
            //{
            //    txtAmountPaid.Focus();
            //    MessageBox.Show("Cannot proceed with transaction! \n Insufficient Amount Paid!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            if (dataGridView1.Rows.Count < 1)
            {
                lblTotal.Text = "0.00";
                lblGrandTotal.Text = "0.00";
                txtAmountPaid.Text = "0.00";
                txtChange.Text = "0.00";
                txtDiscount.Text = "0.00";
                MessageBox.Show("Cart is Empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Hold Transaction?, Click yes to confirm!", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblsalespayment WHERE invoiceid = @invoiceid AND status = @status", cn);
                cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                cm.Parameters.AddWithValue("@status", "Hold");
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    cn.Close();
                    MessageBox.Show("This transaction with Invoice No: " + lblInvoiceNo.Text + " is on Hold!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    decimal num = Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(lblGrandTotal.Text);
                    txtChange.Text = num.ToString();

                    cn.Close();
                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblsalespayment(invoiceid,cusname,cusphone,saletotal,grandtotal,amountpaid,schange,discount,paymode,status,cashier,date,time) VALUES(@invoiceid,@cusname,@cusphone,@saletotal,@grandtotal,@amountpaid,@schange,@discount,@paymode,@status,@cashier,@date,@time)", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@cusname", cboCustomer.Text);
                    cm.Parameters.AddWithValue("@cusphone", txtCustomerPhone.Text);
                    cm.Parameters.AddWithValue("@saletotal", lblTotal.Text);
                    cm.Parameters.AddWithValue("@grandtotal", lblGrandTotal.Text);
                    cm.Parameters.AddWithValue("@amountpaid", txtAmountPaid.Text);
                    cm.Parameters.AddWithValue("@schange", txtChange.Text);
                    cm.Parameters.AddWithValue("@discount", txtDiscount.Text);
                    cm.Parameters.AddWithValue("@paymode", cboPaymentMode.Text);
                    cm.Parameters.AddWithValue("@status", "Hold");
                    cm.Parameters.AddWithValue("@cashier", frmLogin.username.ToUpper());
                    cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                    cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblcart2 SET status = @status WHERE invoiceid = @invoiceid", cn);
                    cm.Parameters.AddWithValue("@invoiceid", lblInvoiceNo.Text);
                    cm.Parameters.AddWithValue("@status", "Hold");
                    cm.ExecuteNonQuery();
                    cn.Close();

                    GetID();
                    LoadCart();
                    lblTotal.Text = "0.00";
                    lblGrandTotal.Text = "0.00";
                    txtAmountPaid.Text = "0.00";
                    txtChange.Text = "0.00";
                    txtDiscount.Text = "0.00";
                    txtQuantity.Text = "0";
                    txtPrice.Text = "0.00";
                    MessageBox.Show("Transaction has been hold succesfully!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                dr.Close();
                cn.Close();
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (txtHold.Text == String.Empty)
            {
                txtHold.Focus();
                txtHold.BackColor = Color.BlanchedAlmond;
                MessageBox.Show("Search box can't be empty", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {               
                GetItemsOnHold();
                //GetCustomerOnHold();
            }
        }

        private void GetItemsOnHold()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcart2 WHERE invoiceid = @invoiceid AND status = @status", cn);
            cm.Parameters.AddWithValue("@invoiceid", txtHold.Text);
            cm.Parameters.AddWithValue("@status", "Hold");
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["medid"].ToString(), dr["medname"].ToString(), dr["quantity"].ToString(), dr["price"].ToString(), dr["invoicetotal"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void GetItemsOnHold2()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblcart2 WHERE invoiceid = @invoiceid AND status = @status", cn);
            cm.Parameters.AddWithValue("@invoiceid", getholdid);
            cm.Parameters.AddWithValue("@status", "Hold");
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["medid"].ToString(), dr["medname"].ToString(), dr["quantity"].ToString(), dr["price"].ToString(), dr["invoicetotal"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void GetCustomerOnHold()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsalespayment WHERE invoiceid = @invoiceid AND status = @status", cn);
            cm.Parameters.AddWithValue("@invoiceid", txtHold.Text);
            cm.Parameters.AddWithValue("@status", "Hold");
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                bool found = true;

                if (found == true)
                {
                    cboCustomer.Text = dr["cusname"].ToString();
                    txtCustomerPhone.Text = dr["cusphone"].ToString();
                    lblTotal.Text = dr["saletotal"].ToString();
                    lblGrandTotal.Text = dr["grandtotal"].ToString();
                    txtAmountPaid.Text = dr["amountpaid"].ToString();
                    txtChange.Text = dr["schange"].ToString();
                    txtDiscount.Text = dr["discount"].ToString();
                    cboPaymentMode.Text = dr["paymode"].ToString();
                    lblInvoiceNo.Text = dr["invoiceid"].ToString();
                }
            }
            else
            {
                cn.Close();
                MessageBox.Show("Not Found", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dr.Close();
            cn.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dispose();
        }

        public void GetCustomerOnHold2()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsalespayment WHERE invoiceid = @invoiceid AND status = @status", cn);
            cm.Parameters.AddWithValue("@invoiceid", getholdid);
            cm.Parameters.AddWithValue("@status", "Hold");
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                string phone;
                txtAmountPaid.Text = dr["amountpaid"].ToString();
                lblGrandTotal.Text = dr["grandtotal"].ToString();
                lblTotal.Text = dr["saletotal"].ToString();
                cboCustomer.Text = dr["cusname"].ToString();
                phone = dr["cusphone"].ToString();
                txtCustomerPhone.Text = phone;
                txtChange.Text = dr["schange"].ToString();
                txtDiscount.Text = dr["discount"].ToString();
                cboPaymentMode.Text = dr["paymode"].ToString();
                lblInvoiceNo.Text = dr["invoiceid"].ToString();
            }
            else
            {
                cn.Close();
                MessageBox.Show("Not Found", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dr.Close();
            cn.Close();
        }

        private void btnSalesRecord_Click(object sender, EventArgs e)
        {
            this.Dispose();
            var f1 = new frmSalesRecord();
            f1.LoadRecord();
            f1.ShowDialog();
        }
    }
}
