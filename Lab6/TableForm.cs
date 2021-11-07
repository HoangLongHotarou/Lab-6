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
    public partial class TableForm : Form
    {
        string stringConection = @"Data Source=hotarou;Initial Catalog=RestaurantManagement;Integrated Security=True";
        List<int> indexs;

        public TableForm()
        {
            indexs = new List<int>();
            InitializeComponent();
            LoadTable();
        }

        public void LoadTable()
        {
            SqlConnection sqlConnection = new SqlConnection(stringConection);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "Select * from [Table]";
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dataTable = new DataTable("Table");
            da.Fill(dataTable);
            dgvTable.DataSource = dataTable;
            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
            TranslateToVietnamese();
        }

        void TranslateToVietnamese()
        {
            dgvTable.Columns["ID"].HeaderText = "Mã Bàn";
            dgvTable.Columns["Name"].HeaderText = "Tên bàn";
            dgvTable.Columns["Status"].HeaderText = "Trạng thái";
            dgvTable.Columns["Capacity"].HeaderText = "Lượng người ngồn";
        }

        private void dgvTable_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow currow = dgvTable.Rows[e.RowIndex];
            if (e.RowIndex >= 0 && !indexs.Contains(e.RowIndex) && currow.Cells[1].Value.ToString() != "")
            {
                indexs.Add(e.RowIndex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(stringConection);
            int numOfRowsEffected = 0;
            foreach (var index in indexs)
            {
                DataGridViewRow row = dgvTable.Rows[index];
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                if (row.Cells[0].Value.ToString() == "")
                {
                    sqlCommand.CommandText = "EXEC TABLE_INSERT @ID,@Name,@Status,@Capacity";
                    sqlCommand.Parameters.Add("@ID", SqlDbType.Int);
                    sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar,200);
                    sqlCommand.Parameters.Add("@Status", SqlDbType.Int);
                    sqlCommand.Parameters.Add("@Capacity", SqlDbType.Int);

                    sqlCommand.Parameters["@ID"].Value = ParameterDirection.Output;
                    sqlCommand.Parameters["@Name"].Value = row.Cells[1].Value;
                    sqlCommand.Parameters["@Status"].Value = row.Cells[2].Value;
                    sqlCommand.Parameters["@Capacity"].Value = row.Cells[3].Value;
                }
                else
                {
                    sqlCommand.CommandText = "EXEC TABLE_UPDATE @ID,@Name,@Status,@Capacity";
                    sqlCommand.Parameters.Add("@ID", SqlDbType.Int);
                    sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 200);
                    sqlCommand.Parameters.Add("@Status", SqlDbType.Int);
                    sqlCommand.Parameters.Add("@Capacity", SqlDbType.Int);

                    sqlCommand.Parameters["@ID"].Value = row.Cells[0].Value;
                    sqlCommand.Parameters["@Name"].Value = row.Cells[1].Value;
                    sqlCommand.Parameters["@Status"].Value = row.Cells[2].Value;
                    sqlCommand.Parameters["@Capacity"].Value = row.Cells[3].Value;
                }
                sqlConnection.Open();
                numOfRowsEffected += sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            if (numOfRowsEffected > 0)
            {
                MessageBox.Show("Chỉnh sửa bàn thành công");
                //btnDelete.Enabled = false;
                //Loading database
                LoadTable();
            }
            else
            {
                MessageBox.Show("Đã có lỗi xải ra. Vui lòng thử lại");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTable.SelectedRows.Count > 0)
                {
                   
                    DataGridViewRow row = dgvTable.SelectedRows[0];
                    SqlConnection sqlConnection = new SqlConnection(stringConection);
                    int numOfRowsEffected = 0;
                    if (row.IsNewRow)
                    {
                        return;
                    }
                    else if (row.Cells[0].Value.ToString() == "")
                    {
                        indexs.Remove(row.Index);
                        LoadTable();
                        return;
                    }
                    //DataGridViewRow row = dgvTable.Rows[indexs[indexs.Count - 1]];
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "EXEC TABLE_DELETE @ID";
                    sqlCommand.Parameters.Add("@ID", SqlDbType.Int);
                    sqlCommand.Parameters["@ID"].Value = row.Cells[0].Value;

                    sqlConnection.Open();
                    numOfRowsEffected = sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                    if (numOfRowsEffected == 1)
                    {
                        MessageBox.Show("Xóa món ăn thành công");
                        indexs.Remove(row.Index);
                        //Loading database
                        LoadTable();
                    }
                    else
                    {
                        MessageBox.Show("Đã có lỗi xải ra. Vui lòng thử lại");
                    }
                }
            }
            catch (SqlException exception)
            {
                MessageBox.Show("Lỗi dính khóa ngoại - nên xóa bills hoặc chuyển bill qua bàn khác để xóa bàn");
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnDelete.PerformClick();
        }

        private void showBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvTable.SelectedRows.Count > 0)
            {
                DataGridViewRow currow = dgvTable.SelectedRows[0];
                BillsForm bills = new BillsForm();
                if (currow.Cells[0].Value.ToString() == "") return;
                bills.LoadBillsUsingIDTable(int.Parse(currow.Cells[0].Value.ToString()));
                bills.ShowDialog();
            }
        }
    }
}
