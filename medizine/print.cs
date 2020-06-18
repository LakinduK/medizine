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
    public partial class print : Form
    {
        public print()
        {
            InitializeComponent();
        }

        // database con

        SqlConnection con = new SqlConnection("Data Source=SLBRAVO_06;Initial Catalog=pharmacy;Integrated Security=True");
        SqlCommand cmd;  // for the'add' button
        SqlCommand cmd1;  // for the 'print' button
        SqlDataAdapter dr;
        SqlDataReader read;

        // get / set methods for Salesid
        private int salesid;

        public int Salesid
        {
            get { return salesid; }
            set { salesid = value; }
        }
        


        private void print_Load(object sender, EventArgs e)
        {
            con.Open();
                // fill data from sales table
            DataTable dt = new DataTable();
            cmd = new SqlCommand("select * from sales where id = '"+Salesid+"'", con);

            dr = new SqlDataAdapter(cmd);
            dr.Fill(dt);

                // fill data from sales_product table
            DataTable dt1 = new DataTable();
            cmd1 = new SqlCommand("select * from sales_product where sales_id = '" + Salesid + "'", con);

            dr = new SqlDataAdapter(cmd1);
            dr.Fill(dt1);

            con.Close();

            CrystalReport1 cr = new CrystalReport1();
            cr.Database.Tables["sales"].SetDataSource(dt);
            cr.Database.Tables["sales_product"].SetDataSource(dt1);

            //this.crystalReportViewer1.ReportSource = cr;

            cr.PrintToPrinter(1, false, 0, 0);
        }
    }
}
