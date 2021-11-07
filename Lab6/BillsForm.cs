using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6
{
    public partial class BillsForm : Form
    {
        public BillsForm()
        {
            InitializeComponent();
            LoadBills();
        }

        private void LoadBills()
        {
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = "SELECT * FROM BILLS";
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("Bills");
            da.Fill(dt);
            dgvBills.DataSource = dt;
            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
        }

        public void LoadBillsUsingIDTable(int idTable)
        {
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = $"SELECT * FROM BILLS where [TableID] = {idTable}";
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("Bills");
            da.Fill(dt);
            dgvBills.DataSource = dt;
            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = $"set dateformat dmy select * from Bills where '{dtpFrom.Value.ToString("dd/MM/yyyy")}' <= CHECKOUTDATE and CHECKOUTDATE<= '{dtpTo.Value.ToString("dd/MM/yyyy")}'";
            Console.WriteLine(query);
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("Bills");
            da.Fill(dt);
            dgvBills.DataSource = dt;
            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
        }



        private void dgvBills_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvBills.Rows[e.RowIndex];
            BillsDetailForm form = new BillsDetailForm();
            if (row.Cells[0].Value.ToString() == "") return;    
            form.LoadBillsDetail(Convert.ToInt32(row.Cells[0].Value));
            form.Show();
        }
    }
}
