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
    public partial class Inventory : Form
    {
        public Inventory()
        {
            InitializeComponent();
            fillComboBox();

            // user name from the login
            lblUserName.Text = LoginInfo.userId;
        }

        // database con
        SqlConnection con = new SqlConnection("Data Source=SLBRAVO_06;Initial Catalog=pharmacy;Integrated Security=True");       

        private void Inventory_Load(object sender, EventArgs e)
        {
            //string qryLoadData = "SELECT (bid,productname,genericName,category,supplier,packSize,qtyAvailable,qtySold,qtyRemaining,expDate,price,discount) FROM product";
            string qryLoadData = "SELECT bid,productname,genericName,category,supplier,packSize,qtyAvailable,qtySold,qtyRemaining,expDate,price,discount FROM product";
            disp_data(qryLoadData);
        }

        //display data to the data table function (reusable)
            // more info at :https://www.youtube.com/watch?v=TIAOr2S6-SY
        public void disp_data(string qry)
        {
            //string query = qry;
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //cmd.CommandText = "SELECT * FROM product";
            cmd.CommandText = qry;
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            con.Close();


        }
        // to fill dropdown menu from the databases category column
            //more info at:https://www.youtube.com/watch?v=cdkDHkXyVFI
        public void fillComboBox()
        {
                    //distinct query to retrieve category list and filter the duplicates.
            string qryFill = "SELECT DISTINCT category FROM product ORDER BY category DESC";
            SqlCommand cmd1 = new SqlCommand(qryFill, con);
            SqlDataReader read;
            

            try
            {
                con.Open();
                read = cmd1.ExecuteReader();

                while(read.Read())
                {
                            //fill the combobox/dropdownlist
                    string category = read["category"].ToString();
                    comboBox1.Items.Add(category);
                }
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        // filter by category 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string qryLoadCategoryData = "SELECT * FROM product WHERE category='"+comboBox1.Text+"'";
            disp_data(qryLoadCategoryData);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string qryLoadData = "SELECT * FROM product";
            disp_data(qryLoadData);
        }

        //SEARCH brand by text changed
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string qrySearchData = "SELECT * FROM product WHERE productname like '"+txtSearch.Text+"%' ";
            disp_data(qrySearchData);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            string qryLoadData = "SELECT * FROM product";
            disp_data(qryLoadData);
        }
    }
}
