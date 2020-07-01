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
    public partial class InsertItems : Form
    {
        public string ExpDate;

        // database con
        SqlConnection con = new SqlConnection("Data Source=SLBRAVO_06;Initial Catalog=pharmacy;Integrated Security=True");       

        public InsertItems()
        {
            InitializeComponent();

            // fill comboboxes by parameters
            string qryFillGenName = "SELECT DISTINCT genericName FROM product ORDER BY genericName DESC";
            string qryFillCategory = "SELECT DISTINCT category FROM product ORDER BY category DESC";
            string qryFillSupplier = "SELECT DISTINCT supplier FROM product ORDER BY supplier DESC";
            string qryFillPackSize = "SELECT DISTINCT packSize FROM product ORDER BY packSize DESC";

            //calling the same function to fill comboboxes
            fillComboBox(qryFillGenName, "genericName");
            fillComboBox(qryFillCategory,"category");
            fillComboBox(qryFillSupplier,"supplier");
            fillComboBox(qryFillPackSize,"packSize");

            txtBarcode.Select();
        }

        private void checkBoxNonExpiry_Click(object sender, EventArgs e)
        {
            if (checkBoxNonExpiry.Checked == true)
            {
                ExpDate = null;
                dateTimePickerExp.Enabled = false;
            }
            else if (checkBoxNonExpiry.Checked == false)
            {
                ExpDate = dateTimePickerExp.Value.Date.ToString();
                dateTimePickerExp.Enabled = true;
                groupBox1.Text = ExpDate;
            }
            
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            //key enumeration is used in the if condition. for more info visit:
            //www.docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netcore-3.1
            //www.youtube.com/watch?v=EcZQghn88vE

            if(e.KeyChar == 13) //13 is the 'enter' key.
            {
                txtBrandName.Select();
            }
        }

        private void txtBrandName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //13 is the 'enter' key.
            {
                comboBoxGenericName.Select();
            }
        }


        public void fillComboBox(string qryFill, string columnName)
        {
            //distinct query to retrieve category list and filter the duplicates.
            
            SqlCommand cmd1 = new SqlCommand(qryFill, con);
            SqlDataReader read;


            try
            {
                con.Open();
                read = cmd1.ExecuteReader();

                while (read.Read())
                {
                    if (columnName == "genericName")
                    {
                        //fill the comboBoxGenericName
                        string genericName = read["genericName"].ToString();
                        comboBoxGenericName.Items.Add(genericName);
                    }

                    else if (columnName == "category")
                    {
                        //fill the comboBoxCategory
                        string category = read["category"].ToString();
                        comboBoxCategory.Items.Add(category);
                    }
                    else if (columnName == "supplier")
                    {
                        //fill the comboBoxSupplier
                        string supplier = read["supplier"].ToString();
                        comboBoxSupplier.Items.Add(supplier);
                    }
                    else if (columnName == "packSize")
                    {
                        //fill the comboBoxpackSize
                        string packSize = read["packSize"].ToString();
                        comboBoxPackSize.Items.Add(packSize);
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("an error occoured trying to connect the database." + ex , ("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

            // textbox clear function
        public void clearTextboxes()
        {
            txtBarcode.Clear();
            txtBrandName.Clear();
            comboBoxGenericName.Text = "";
            comboBoxCategory.Text = "";
            comboBoxSupplier.Text = "";
            comboBoxPackSize.Text = "";
            txtQtyAvailable.Clear();
            txtQtyRemaining.Clear();
            txtQtySold.Clear();
            txtWholeSalePrice.Clear();
            txtRetailPrice.Clear();
            txtDiscount.Clear();

            txtBarcode.Select();
        }

        // INSERT values to the database
        private void btnInsert_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string brandName = txtBrandName.Text.Trim();
           

                // check for null values
            if(barcode == null || barcode == "" || brandName == null || brandName == "" || txtWholeSalePrice.Text == "" || txtRetailPrice.Text == "" || txtDiscount.Text == "")
            {
                MessageBox.Show("values cannot be empty", ("Empty fields"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                    // check if the values exists in the database
                string queryCheckDb = "SELECT * FROM product WHERE bid = '"+barcode+"'";
                SqlDataAdapter da = new SqlDataAdapter(queryCheckDb, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count >= 1)
                {
                    MessageBox.Show("the product you are trying to insert, already exists in the database.", ("Duplicate values"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        // variables
                        string genericName = comboBoxGenericName.Text.Trim();
                        string category = comboBoxCategory.Text.Trim();
                        string supplier = comboBoxSupplier.Text.Trim();
                        string packSize = comboBoxPackSize.Text.Trim();
                        string qtyAvailable = txtQtyAvailable.Text;
                        string qtySold = txtQtySold.Text;
                        string qtyRemaining = txtQtyRemaining.Text;
                        //(string 'ExpDate' is decalred at the top of this file)
                        double wholeSalePrice = double.Parse(txtWholeSalePrice.Text);
                        double retailPrice = double.Parse(txtRetailPrice.Text);
                        double discount = double.Parse(txtDiscount.Text);

                            // INSERT TO database
                        string queryInsert = "INSERT INTO product (bid,productname,genericName,category,supplier,packSize,qtyAvailable,qtySold,qtyRemaining,expDate,wholeSalePrice,price,discount) VALUES ('" + barcode + "','" + brandName + "','" + genericName + "','" + category + "','" + supplier + "','" + packSize + "','" + qtyAvailable + "','" + qtySold + "','" + qtyRemaining + "','" + ExpDate + "','" + wholeSalePrice + "','" + retailPrice + "','" + discount + "')";
                        con.Open();
                        SqlCommand cmd = new SqlCommand(queryInsert, con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        
                        MessageBox.Show("Product inserted successfully", ("Product added"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // clear the textboxes!
                        clearTextboxes();
                        
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("unexpected error occured! " + ex);
                    }
                }
            }

            
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            cmd = new SqlCommand("select * from product where bid='" + txtBarcode.Text+ "'", con);
            con.Open();
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                txtBrandName.Text = dr["productname"].ToString();
                comboBoxGenericName.Text = dr["genericName"].ToString();
                comboBoxCategory.Text = dr["category"].ToString();
                comboBoxSupplier.Text = dr["supplier"].ToString();
                comboBoxPackSize.Text = dr["packSize"].ToString();

                txtQtyAvailable.Text = dr["qtyAvailable"].ToString();
                txtQtySold.Text = dr["qtySold"].ToString();
                txtQtyRemaining.Text = dr["qtyRemaining"].ToString();
                //dateTimePickerExp.Text =  dr["expDate"].ToString();   
                string dateRetrieved = dr["expDate"].ToString();
                //dateTimePickerExp.Value.Date.ToSt= dateRetrieved;
                txtWholeSalePrice.Text = dr["wholeSalePrice"].ToString();
                txtRetailPrice.Text = dr["price"].ToString();
                //txtDiscount.Text = dr["discount"].ToString();
                txtDiscount.Text = dr["discount"].ToString();
               

                MessageBox.Show("found record of '" + txtBarcode.Text + "'. ", ("Match found"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            con.Close();
            

        }

    }
}
