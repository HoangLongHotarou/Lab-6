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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            //create a chain link to RestaurantManagement database
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security=true;";
            //create Object connection
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            //create command object execute
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            //Establish query to command object
            string query = "SELECT ID, Name, Type From Category";
            sqlCommand.CommandText = query;
            //Open connect to dadabase
            sqlConnection.Open();
            //Execute command by ExecuteReader method
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            //call function display data on screen
            this.DisplayCategory(sqlDataReader);
            sqlConnection.Close();
        }

        private void DisplayCategory(SqlDataReader reader)
        {
            lvCategory.Items.Clear();
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader["ID"].ToString());
                lvCategory.Items.Add(item);
                item.SubItems.Add(reader["Name"].ToString());
                item.SubItems.Add(reader["Type"].ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length == 0 && txtType.Text.Length == 0) return;
            //Create object connectionString
            string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            //Create object execute command
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "INSERT INTO Category(Name,[Type]) VALUES (N'" + txtName.Text + "'," + txtType.Text + ")";

            //Open and connect database
            sqlConnection.Open();

            //Execute command by ExecuteNonQuery
            int numOfRowsEffected = sqlCommand.ExecuteNonQuery();

            //Close connect
            sqlConnection.Close();

            if (numOfRowsEffected == 1)
            {
                MessageBox.Show("Thêm món ăn thành công");

                //Loading database
                btnLoad.PerformClick();

                //Delete all text are filled
                txtName.Text = "";
                txtType.Text = "";
            }
            else
            {
                MessageBox.Show("Đã có lỗi xải ra. Vui lòng thử lại");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            // Create object execute command
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // Execute command for command object
            sqlCommand.CommandText = $"UPDATE Category SET Name = N'{txtName.Text}', [Type] = N'{txtType.Text}' WHERE ID = {txtID.Text}";

            //Open and connect database
            sqlConnection.Open();

            //Open and connect database
            int numOfRowsEffected = sqlCommand.ExecuteNonQuery();

            //Close connect
            sqlConnection.Close();

            if (numOfRowsEffected == 1)
            {
                // Update data on ListView
                ListViewItem item = lvCategory.SelectedItems[0];
                item.SubItems[1].Text = txtName.Text;
                item.SubItems[2].Text = txtType.Text;

                // Delete all text are filled
                txtID.Text = "";
                txtName.Text = "";
                txtType.Text = "";

                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                MessageBox.Show("Cập nhật thành công");

            }
            else
            {
                MessageBox.Show("Đã có lỗi");
            }

        }

        private void lvCategory_Click(object sender, EventArgs e)
        {
            //get row is selected in ListView
            ListViewItem item = lvCategory.SelectedItems[0];

            //Display data on Screen
            txtID.Text = item.Text;
            txtName.Text = item.SubItems[1].Text;
            txtType.Text = item.SubItems[2].Text; //== "0" ? "Thức uống" : "Đồ án";

            //Enable update and delete btn
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
                SqlConnection sqlConnection = new SqlConnection(connectionString);

                // Create object execute command
                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                // Execute command for command object
                sqlCommand.CommandText = $"DELETE FROM Category WHERE ID = {txtID.Text}";

                //Open and connect database
                sqlConnection.Open();

                //Open and connect database
                int numOfRowsEffected = sqlCommand.ExecuteNonQuery();

                //Close connect
                sqlConnection.Close();

                if (numOfRowsEffected == 1)
                {
                    // Update data on ListView
                    ListViewItem item = lvCategory.SelectedItems[0];
                    lvCategory.Items.Remove(item);

                    // Delete all text are filled
                    txtID.Text = "";
                    txtName.Text = "";
                    txtType.Text = "";
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;
                    MessageBox.Show("Xoá thành công");
                }
                else
                {
                    MessageBox.Show("Đã có lỗi");
                }
            }
            catch (SqlException expection)
            {
                MessageBox.Show("Lỗi dính khóa ngoại: " + expection);
            }
        }

        private void tsmDelete_Click(object sender, EventArgs e)
        {
            if (lvCategory.SelectedItems.Count > 0)
            {
                btnDelete.PerformClick();
            }
        }

        private void tsmView_Click(object sender, EventArgs e)
        {
            if (txtID.Text != "")
            {
                FoodForm foodForm = new FoodForm();
                foodForm.Show(this);
                foodForm.LoadFood(Convert.ToInt32(txtID.Text));
            }
        }

        private void btnBills_Click(object sender, EventArgs e)
        {
            BillsForm billsForm = new BillsForm();
            billsForm.Show(this);
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            AccountManager accountManager = new AccountManager();
            accountManager.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TableForm tableForm = new TableForm();
            tableForm.ShowDialog();
        }
    }
}
