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

            lblUserName.Text = LoginInfo.userId;

            // fill comboboxes by parameters
            string qryFillGenName = "SELECT DISTINCT genericName FROM product ORDER BY genericName DESC";
            string qryFillCategory = "SELECT DISTINCT category FROM product ORDER BY category DESC";
            string qryFillSupplier = "SELECT DISTINCT supplier FROM product ORDER BY supplier DESC";
            string qryFillPackSize = "SELECT DISTINCT packSize FROM product ORDER BY packSize DESC";

            //calling the function to fill comboboxes
            fillComboBox(qryFillGenName, "genericName");
            fillComboBox(qryFillCategory,"category");
            fillComboBox(qryFillSupplier,"supplier");
            fillComboBox(qryFillPackSize,"packSize");

            txtBarcode.Select();
        }

        // function to check if the datetimepicker is enabled or not
        public void checkDateEnabled()
        {
            if (checkBoxNonExpiry.Checked == true)
            {
                ExpDate = "";
                dateTimePickerExp.Enabled = false; //hide datetime picker
            }
            else if (checkBoxNonExpiry.Checked == false)
            {
                ExpDate = dateTimePickerExp.Value.Date.ToString();
                dateTimePickerExp.Enabled = true;  //show datetime picker

            }
        }

        private void checkBoxNonExpiry_Click(object sender, EventArgs e)
        {
            checkDateEnabled();
            
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

        // function to fill comboboxes from database columns
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

        // function to clear textboxs
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
                        ExpDate = dateTimePickerExp.Value.Date.ToString();
                        double wholeSalePrice = double.Parse(txtWholeSalePrice.Text);
                        double retailPrice = double.Parse(txtRetailPrice.Text);
                        double discount = double.Parse(txtDiscount.Text);

                             // checks for the expiry date checkbox is enabled or not
                        checkDateEnabled();

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
                        MessageBox.Show("unexpected error occured! " + ex, ("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            
        }

        
        private void btnFind_Click(object sender, EventArgs e)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            cmd = new SqlCommand("select * from product where bid='" + txtBarcode.Text+ "'", con);
            
            try
            {
                con.Open();
                dr = cmd.ExecuteReader();

                if (txtBarcode.Text == "" || txtBarcode.Text == null)
                {
                    MessageBox.Show("Enter a barcode value to find", ("Empty value"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (dr.Read())
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
                    string expDateRetrieved = dr["expDate"].ToString();
                    dateTimePickerExp.Text = expDateRetrieved;
                    txtWholeSalePrice.Text = dr["wholeSalePrice"].ToString();
                    txtRetailPrice.Text = dr["price"].ToString();
                    txtDiscount.Text = dr["discount"].ToString();
                    //txtDiscount.Text = dateRetrieved;

                    if (expDateRetrieved == "1/1/1900 12:00:00 AM" || expDateRetrieved == null)
                    {
                        checkBoxNonExpiry.Checked = true;
                        dateTimePickerExp.Enabled = false;
                    }
                    else
                    {
                        checkBoxNonExpiry.Checked = false;
                        dateTimePickerExp.Enabled = true;
                    }


                    MessageBox.Show("Found record of '" + txtBarcode.Text + "'. ", ("Match found"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("No matching record found on'" + txtBarcode.Text + "'. ", ("Not found"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                con.Close();
            }
            catch(Exception ex)
            {

                MessageBox.Show("Unexpected error occured" + ex, ("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            SqlCommand cmd;
            SqlDataReader dr;
            cmd = new SqlCommand("select * from product where bid='" + txtBarcode.Text+ "'", con);

            try
            {
                //checks for empty values
                if (txtBarcode.Text == null || txtBarcode.Text == "" || txtWholeSalePrice.Text == "" || txtRetailPrice.Text == "" || txtDiscount.Text == "")
                {
                    MessageBox.Show("Barcode or other values are empty! " + "\n" + "Enter the barcode and click 'Find' button to edit and save the items", ("Empty values"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // variables
                    string brandName = txtBrandName.Text.Trim();
                    string genericName = comboBoxGenericName.Text.Trim();
                    string category = comboBoxCategory.Text.Trim();
                    string supplier = comboBoxSupplier.Text.Trim();
                    string packSize = comboBoxPackSize.Text.Trim();
                    string qtyAvailable = txtQtyAvailable.Text;
                    string qtySold = txtQtySold.Text;
                    string qtyRemaining = txtQtyRemaining.Text;
                    ExpDate = dateTimePickerExp.Value.Date.ToString();
                    double wholeSalePrice = double.Parse(txtWholeSalePrice.Text);
                    double retailPrice = double.Parse(txtRetailPrice.Text);
                    double discount = double.Parse(txtDiscount.Text);

                    con.Open();
                    dr = cmd.ExecuteReader();

                    //checks for barcode is correct
                    if (dr.Read())
                    {
                        con.Close();
                        con.Open();

                        checkDateEnabled();
                        string queryEdit = "UPDATE product SET productname = '" + brandName + "',genericName = '" + genericName + "', category = '" + category + "',supplier = '" + supplier + "',packSize = '" + packSize + "',qtyAvailable = '" + qtyAvailable + "',qtySold = '" + qtySold + "',qtyRemaining = '" + qtyRemaining + "',expDate = '" + ExpDate + "',wholeSalePrice= '" + wholeSalePrice + "',price= '" + retailPrice + "',discount = '" + discount + "' WHERE bid= '" + txtBarcode.Text + "'";
                        SqlCommand cmd2 = new SqlCommand(queryEdit, con);
                        cmd2.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show(" Product: '" + txtBarcode.Text + "' " + " " + " '" + txtBrandName.Text + "' updated and saved successfully", ("Product updated"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("product not found! " + "\n" + "Enter the barcode and click 'Find' button, then edit and save the item", ("try again"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show("Unexpected error occured" + exc, ("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
                
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlCommand cmd;
            cmd = new SqlCommand("DELETE FROM product WHERE bid='" + txtBarcode.Text + "'", con);

            if(txtBarcode.Text == null || txtBarcode.Text == "")
            {
                MessageBox.Show("Barcode is empty! " + "\n" + "Enter the barcode to delete an item", ("Empty values"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                DialogResult dialogDelete = MessageBox.Show("Are you sure you want to Delete '"+txtBarcode.Text+"' " +" "+ " '"+txtBrandName.Text+"' from the database?"+"\n"+ "You cannot undo this change. ", ("Delete an item"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogDelete == DialogResult.Yes)
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show(" '" + txtBarcode.Text + "' item deleted successfully!", ("Delete"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                                
            }
        }

    }
}
