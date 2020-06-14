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

        SqlConnection con = new SqlConnection("Data Source=SLBRAVO_06;Initial Catalog=pharmacy;Integrated Security=True");
        SqlCommand cmd;  //for the'add' button
        SqlCommand cmd1;  //for the 'print' button
        SqlDataAdapter dr;
        SqlDataReader read;

        private void txtdcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // when user enter id the name and price will be filled.
            //key enumeration is used in the if condition. for more info visit:
            //www.docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netcore-3.1
            //www.youtube.com/watch?v=EcZQghn88vE

            if(e.KeyChar == 13) //13 is the 'enter' key.
            {
                cmd = new SqlCommand("select * from product where id='"+txtdcode.Text+"'",con);
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
            if(txtqty.Text != "")
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

            }
            else
            {
                MessageBox.Show("enter the quantity",("empty value"),MessageBoxButtons.OK,MessageBoxIcon.Warning);
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

                sql1 = "insert into sales(subtotal,pay,balance)values(@subtotal,@pay,@balance) select @@identity;";
                con.Open();
                cmd = new SqlCommand(sql1, con);
                cmd.Parameters.AddWithValue("@subtotal", total);
                cmd.Parameters.AddWithValue("@pay", pay);
                cmd.Parameters.AddWithValue("@balance", bal);
                int lastid = int.Parse(cmd.ExecuteScalar().ToString());

                string dname;
                int price = 0;
                int qty = 0;
                int tot = 0;

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




    }
}
