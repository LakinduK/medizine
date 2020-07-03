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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            lblError.ForeColor = Color.ForestGreen;
        }

        // database con
        SqlConnection con = new SqlConnection("Data Source=SLBRAVO_06;Initial Catalog=pharmacy;Integrated Security=True");       
         SqlCommand cmd;  
        SqlDataReader read;
        private void btnClose_Click(object sender, EventArgs e)
        {
            
            Application.ExitThread();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPassword.Clear();
            txtUserName.Clear();
            lblError.ForeColor = Color.ForestGreen;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            cmd = new SqlCommand("SELECT * FROM users WHERE userName='"+txtUserName.Text.Trim()+"' AND password='"+txtPassword.Text.Trim()+"'",con);
            

            if(txtPassword.Text == "" || txtUserName.Text == "")
            {
                lblError.ForeColor = Color.OrangeRed;
                lblError.Text = "Username or password empty";
                txtUserName.Select();
            }
            else
            {
                try
                {
                    con.Open();
                    read = cmd.ExecuteReader();
                    if (read.Read())
                    {
                        // set global username to this user
                        LoginInfo.userId = txtUserName.Text.Trim();

                        // login
                        this.Hide();
                        Form1 lo = new Form1();
                        lo.Show();
                        
                    }
                    else
                    {
                        lblError.ForeColor = Color.OrangeRed;
                        lblError.Text = "Username or password incorrect";
                        txtPassword.Clear();
                        txtUserName.Clear();
                        txtUserName.Select();
                    }
                    con.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Database connection occured! " +"\n" + ex);
                    con.Close();
                }
                
            }


        }
    }
}
