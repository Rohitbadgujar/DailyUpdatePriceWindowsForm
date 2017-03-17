using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshDatagrid();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Value = new DateTime();

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

        private void SaveData_Click(object sender, EventArgs e)
        {
            if (ValidateGoldPrice())
            {
                try
                {
                    string conStr = ConfigurationManager.ConnectionStrings["WDBTESTConnectionString"].ToString();
                    SqlConnection con = new SqlConnection(conStr);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UpdateDailyGoldPrice", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = txtPrice.Text;
                    cmd.Parameters.Add("@Day", SqlDbType.DateTime).Value = dateTimePicker1.Value;
                    SqlParameter returnParameter = cmd.Parameters.Add("@Flag", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    int id = (int)returnParameter.Value;
                    if (id == 0)
                        MessageBox.Show("Gold Rate Successfully Inserted " + Environment.NewLine + Environment.NewLine + " Gold Rate : " + txtPrice.Text + "     Date : " + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "");

                    else
                        MessageBox.Show("Gold Rate Successfully Updated " + Environment.NewLine + Environment.NewLine + " Gold Rate : " + txtPrice.Text + "     Date : " + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "");
                    con.Close();
                    RefreshDatagrid();
                    txtPrice.Text = "";
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }               
            }
        }

        private void RefreshDatagrid() {
            try
            {
                string conStr = ConfigurationManager.ConnectionStrings["WDBTESTConnectionString"].ToString();
                SqlConnection con = new SqlConnection(conStr);
                con.Open();
                SqlCommand cmd1 = new SqlCommand("Select top 5 * from DailyGoldRate order by LastUpdatedDate desc", con);       //Get last Update top 5 record and display is DataGird
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd1;
                DataTable dt = new DataTable();
                da.Fill(dt);
                BindingSource bsource = new BindingSource();
                bsource.DataSource = dt;
                dataGridView1.DataSource = bsource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private bool ValidateGoldPrice() {
            //Validate price upto 5 decimal
            if (txtPrice.Text == "") {
                MessageBox.Show("Enter Gold Price for day :" + dateTimePicker1.Text );
                return false;
            }
            decimal d = decimal.Parse(txtPrice.Text);
            Int64 n = (Int64)d;
            string v = (d - n).ToString();
            string[] parts = v.Split('.');

            if (parts[1].Length >= 5)
            {
                MessageBox.Show("Enter Gold Price upto 3 Decimal");
                return false;
            }
            else {
                return true;
            }
        }
        private void dateTimePicker1_LeaveFocus(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > DateTime.Now) {
                MessageBox.Show("You have Selected future Date");     //Alert User about future date
            }
         
        }

        private void txtPrice_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            if (null != gridView)
            {
                foreach (DataGridViewRow r in (gridView.Rows))
                {
                    if(gridView.Rows[r.Index].Index <  5)
                             gridView.Rows[r.Index].HeaderCell.Value = (r.Index + 1).ToString(); //Display Row number in DataGrid
                }
            }
        }
    }
}
