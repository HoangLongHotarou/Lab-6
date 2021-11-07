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
    public partial class BillsDetailForm : Form
    {
        public BillsDetailForm()
        {
            InitializeComponent();
        }

        public void LoadBillsDetail(int id)
        {
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = $"SELECT B.ID,B.[InvoiceID],FOOD.[NAME],B.[Quantity] FROM [BillDetails] as B,[FOOD] WHERE [InvoiceID] = {id} AND [FoodID] = FOOD.ID";
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("Bill Detail");
            da.Fill(dt);
            dgvBills.DataSource = dt;
            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
            TranslateToVietnamese();
        }

        private void TranslateToVietnamese()
        {
            dgvBills.Columns["InvoiceID"].HeaderText = "Mã Bill";
            dgvBills.Columns["NAME"].HeaderText = "Tên món";
            dgvBills.Columns["Quantity"].HeaderText = "Số lượng";
        }
    }
}
