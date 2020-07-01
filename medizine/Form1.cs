using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace medizine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // database con

        SqlConnection con = new SqlConnection("Data Source=SLBRAVO_06;Initial Catalog=pharmacy;Integrated Security=True");
        SqlCommand cmd;  // for the'add' button
        SqlCommand cmd1;  // for the 'print' button
        SqlDataAdapter da;
        SqlDataReader read;

        private void txtdcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // when user enter id the name and price will be filled.
            //key enumeration is used in the if condition. for more info visit:
            //www.docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netcore-3.1
            //www.youtube.com/watch?v=EcZQghn88vE

            if(e.KeyChar == 13) //13 is the 'enter' key.
            {
                cmd = new SqlCommand("select * from product where bid='"+txtdcode.Text+"'",con);
                con.Open();
                read = cmd.ExecuteReader();

                if(read.Read())
                {
                    string pname;
                    string price;

                    pname = read["productname"].ToString();
                    price = read["price"].ToString();

                    txtdname.Text = pname;
                    txtprice.Text = price;
                    txtqty.Select();        //sets the cursor to quantity textbox
                }

                else
                {
                    MessageBox.Show("No barcode found!",("empty value"),MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                con.Close();
            }
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            // checks if the quantity is empty
            if(txtqty.Text != "" && txtdcode.Text !="")
            {
                string dcode = txtdcode.Text;
                string dname = txtdname.Text;
                double price = double.Parse(txtprice.Text);
                double qty = double.Parse(txtqty.Text);


                // to fill the datagridview
                double tot = price * qty;
                this.dataGridView1.Rows.Add(dcode, dname, price, qty, tot);

                //to calculate total
                int sum = 0;

                for (int row = 0; row < dataGridView1.Rows.Count; row++)
                {
                    sum = sum + Convert.ToInt32(dataGridView1.Rows[row].Cells[4].Value);
                }

                txttotal.Text = sum.ToString();

                //clear textboxes 
                txtdcode.Clear();
                txtdname.Clear();
                txtprice.Clear();
                txtqty.Clear();
                txtdcode.Select();        //sets the cursor to drug code textbox

            }
            else
            {
                MessageBox.Show("enter the product id & quantity",("empty values"),MessageBoxButtons.OK,MessageBoxIcon.Warning);
                txtqty.Select();        //sets the cursor to quantity textbox
            }

            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["delete"].Index && e.RowIndex >=0);
            {
                dataGridView1.Rows.Remove(dataGridView1.Rows[e.RowIndex]);

                int sum = 0;

                for (int row = 0; row < dataGridView1.Rows.Count; row++)
                {
                    sum = sum + Convert.ToInt32(dataGridView1.Rows[row].Cells[4].Value);
                }

                txttotal.Text = sum.ToString();


            }
        }

            // method for saving the bill data in 'sales' table
        public void SaleSave()
        {
            
                string total = txttotal.Text;
                string pay = txtpay.Text;
                string bal = txtbal.Text;

                string sql1;    //insert into 'sales' table
                string sql2;    //insert into 'sales_product' table
                string dateTime = DateTime.Now.ToString();

                sql1 = "INSERT INTO sales(subtotal,pay,balance,datetime)values(@subtotal,@pay,@balance,@dt) select @@identity;";
                con.Open();
                cmd = new SqlCommand(sql1, con);
                cmd.Parameters.AddWithValue("@subtotal", total);
                cmd.Parameters.AddWithValue("@pay", pay);
                cmd.Parameters.AddWithValue("@balance", bal);
                cmd.Parameters.AddWithValue("@dt", dateTime);
                int lastid = int.Parse(cmd.ExecuteScalar().ToString());

                string dname;
                int price = 0;
                int qty = 0;
                int tot = 0;

                // to save the table 'sales_product' from datagridview 
                for (int row = 0; row < dataGridView1.Rows.Count; row++)
                {
                    dname = dataGridView1.Rows[row].Cells[1].Value.ToString();
                    price = int.Parse(dataGridView1.Rows[row].Cells[2].Value.ToString());
                    qty = int.Parse(dataGridView1.Rows[row].Cells[3].Value.ToString());
                    tot = int.Parse(dataGridView1.Rows[row].Cells[4].Value.ToString());

                    sql2 = "insert into sales_product(sales_id,drugname,price,qty,total)values(@sales_id,@drugname,@price,@qty,@total)";
                    cmd1 = new SqlCommand(sql2, con);
                    cmd1.Parameters.AddWithValue("@sales_id", lastid);
                    cmd1.Parameters.AddWithValue("@drugname", dname);
                    cmd1.Parameters.AddWithValue("@price", price);
                    cmd1.Parameters.AddWithValue("@qty", qty);
                    cmd1.Parameters.AddWithValue("@total", tot);

                    cmd1.ExecuteNonQuery();

                }

                MessageBox.Show("sales completed successfully!",("notification"),MessageBoxButtons.OK,MessageBoxIcon.Information);

            // print recipt

                print p = new print();
                p.Salesid = lastid;
                p.Show();


                con.Close();
           
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //check if the balance is empty
            if(txtpay.Text !="")
            {
            double total = double.Parse(txttotal.Text);
            double pay = double.Parse(txtpay.Text);
            double bal = pay - total;
            txtbal.Text = bal.ToString();

            //to call the save sales to the database function
            SaleSave();
            }
            else
            {
                MessageBox.Show("enter the amount of payment", ("empty values"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtpay.Select();        //sets the cursor to payment amount textbox
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //clear textboxes 
            txtdcode.Clear();
            txtdname.Clear();
            txtprice.Clear();
            txtqty.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // show date/time on top of the program  = YYYY-MM-DD HH:MI:SS.
            timer1.Start();
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToLongTimeString();
        }
            
            //timer for date time update
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();
        }

        private void inventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inventory inv = new Inventory();
                
                inv.Show();
                
            
            
        }

        // Exit Application - from menu strip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogExit = MessageBox.Show("Are you sure you want to exit?", ("Exit"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogExit == DialogResult.Yes)
            {
                Application.ExitThread();
            }
            
        }
        // Exit Application - from window close button
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogExit = MessageBox.Show("Are you sure you want to exit?", ("Exit"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogExit == DialogResult.Yes)
            {
                Application.ExitThread();
            }
            else if (dialogExit == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void localBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //backup code here
        }

        private void btnNextCust_Click(object sender, EventArgs e)
        {
            //clear textboxes 
            txtdcode.Clear();
            txtdname.Clear();
            txtprice.Clear();
            txtqty.Clear();
            txttotal.Clear();
            txtpay.Clear();
            txtbal.Clear();

            dataGridView1.Rows.Clear(); //to reset datagridview
            dataGridView1.Refresh();
            txtdcode.Select();        //sets the cursor to drug code textbox

        }

        private void insertItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertItems ins = new InsertItems();
            ins.Show();
        }
    }
}
