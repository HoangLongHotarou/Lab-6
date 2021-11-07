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
    public partial class FoodForm : Form
    {
        public int CategoryID { get; set; }
        private int index = -1;
        private DataGridViewRow row;
        private List<int> indexs;

        public FoodForm()
        {
            InitializeComponent();
            indexs = new List<int>();
        }

        public void LoadFood(int categoryID)
        {
            this.CategoryID = categoryID;
            //create a chain link to RestaurantManagement database
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security=true;";
            //create Object connection
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            //create command object execute
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            //Establish query to command object
            string query = $"SELECT Name FROM Category where ID = {categoryID}";
            sqlCommand.CommandText = query;
            //Open connect to dadabase
            sqlConnection.Open();

            //tag product name for title
            string catName = sqlCommand.ExecuteScalar().ToString();
            this.Text = "DS món ăn thuộc nhóm: " + catName;
            sqlCommand.CommandText = $"SELECT * FROM Food WHERE FOODCATEGORYID = {categoryID}";

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

            DataTable dt = new DataTable("Food");
            da.Fill(dt);

            dgvFood.DataSource = dt;

            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
            TranslateToVietnamese();
        }

        void TranslateToVietnamese()
        {
            dgvFood.Columns["ID"].HeaderText = "Tên tài khoản";
            dgvFood.Columns["Name"].HeaderText = "Tên món";
            dgvFood.Columns["Unit"].HeaderText = "Đơn vị";
            dgvFood.Columns["FoodCategoryID"].HeaderText = "Mã loại món ăn";
            dgvFood.Columns["Price"].HeaderText = "Giá";
            dgvFood.Columns["Notes"].HeaderText = "Notes";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int numOfRowsEffected = 0;
            foreach (var index in indexs)
            {
                //Create object connectionString
                row = dgvFood.Rows[index];
                string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                //Create object execute command
                string c5 = row.Cells[5].Value.ToString() == "" ? "NULL" : $"N'{row.Cells[5].Value}'";
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                string query = "";
                if (row.Cells[0].Value.ToString() == "")
                {
                    query = $"INSERT INTO FOOD([Name], [Unit], [FoodCategoryID], [Price], [Notes]) " +
                        $"VALUES (N'{row.Cells[1].Value}',N'{row.Cells[2].Value}',{row.Cells[3].Value},{row.Cells[4].Value}," + c5 + ")";
                }
                else
                {
                    query = $"UPDATE FOOD SET [Name] = N'{row.Cells[1].Value}', [Unit] = N'{row.Cells[2].Value}', [FoodCategoryID] = {row.Cells[3].Value}," +
                        $"[Price] = {row.Cells[4].Value}, [Notes] = " + c5 + $" WHERE ID = {row.Cells[0].Value}";
                }
                //Open and connect database
                sqlCommand.CommandText = query;
                sqlConnection.Open();
                ////Execute command by ExecuteNonQuery
                numOfRowsEffected += sqlCommand.ExecuteNonQuery();

                ////Close connect
                sqlConnection.Close();

            }
            if (numOfRowsEffected > 0)
            {
                MessageBox.Show("Chỉnh sửa món ăn thành công");
                btnDelete.Enabled = false;
                //Loading database
                LoadFood(CategoryID);
            }
            else
            {
                MessageBox.Show("Đã có lỗi xải ra. Vui lòng thử lại");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //plan A
            //if (dgvFood.Rows.Count > 0)
            //{
            //    foreach (DataGridViewRow item in dgvFood.SelectedRows)
            //    {
            //         
            //    }
            //}

            //plan B
            //Create object connectionString
            row = dgvFood.Rows[indexs[indexs.Count - 1]];
            if (row.Cells[0].Value.ToString() == "")
            {
                dgvFood.Rows.RemoveAt(index);
                return;
            };

            string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            //Create object execute command
            string c5 = row.Cells[5].Value.ToString() == "" ? "NULL" : $"'{row.Cells[5].Value}'";
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = $"Delete from Food where ID = {row.Cells[0].Value}";
            //Open and connect database
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            ////Execute command by ExecuteNonQuery
            int numOfRowsEffected = sqlCommand.ExecuteNonQuery();
            ////Close connect
            sqlConnection.Close();
            if (numOfRowsEffected == 1)
            {
                MessageBox.Show("Xóa món ăn thành công");
                btnDelete.Enabled = false;
                //Loading database
                dgvFood.Rows.RemoveAt(index);
                indexs.RemoveAt(indexs.Count - 1);
                LoadFood(CategoryID);
            }
            else
            {
                MessageBox.Show("Đã có lỗi xải ra. Vui lòng thử lại");
            }
        }

        private void dgvFood_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            btnDelete.Enabled = true;
            DataGridViewRow currow = dgvFood.Rows[e.RowIndex];
            currow.Cells[0].ReadOnly = true;
            currow.Cells[3].ReadOnly = true;
            //if (dgvFood.Rows.Count < 1) return;
            if (currow.Cells[0].Value.ToString() == "")
            {
                //row.Cells[0].Value = Convert.ToInt16(dgvFood.Rows[e.RowIndex - 1].Cells[0].Value.ToString()) + 1;
                currow.Cells[3].Value = CategoryID;
            }
            if (e.RowIndex >= 0 && !indexs.Contains(e.RowIndex) && currow.Cells[1].Value.ToString()!="")
            {
                index = e.RowIndex;
                //MessageBox.Show(row.Cells[0].Value.ToString());
                indexs.Add(index);
            }
        }
    }
}
