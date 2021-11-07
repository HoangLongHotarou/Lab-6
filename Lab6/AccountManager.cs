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
    public partial class AccountManager : Form
    {
        private List<string> items;
        private string roleName,roleID;

        public AccountManager()
        {
            items = new List<string>();
            InitializeComponent();
            LoadRole();
            LoadGridView();
        }

        private void LoadRole()
        {
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security=true;";
            //create Object connection
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            //create command object execute
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            //Establish query to command object
            string query = "SELECT ID, RoleName From Role ";
            sqlCommand.CommandText = query;
            //Open connect to dadabase
            sqlConnection.Open();
            //Execute command by ExecuteReader method
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            //call function display data on screen
            this.GetItemsForMenu(sqlDataReader);
            InsertMenu();
            roleID = items[0].Split('^')[1];
            this.lbRole.Text = items[0].Split('^')[0];
            sqlConnection.Close();
        }

        private void GetItemsForMenu(SqlDataReader reader)
        {
            //items = new List<string>();
            while (reader.Read())
            {
                items.Add(reader["RoleName"].ToString()+"^"+ reader["ID"].ToString());
            }
        }

        private void InsertMenu()
        {
            foreach (var item in items)
            {
                string [] str = item.Split('^');
                ToolStripMenuItem i = new ToolStripMenuItem(str[0]);
                i.Tag = str[1];
                tsmiRole.DropDownItems.Add(i);
                i.Click += new EventHandler(ItemClick);
            }
        }

        private void ItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            //MessageBox.Show(item.Text);
            roleName = item.Text;
            roleID = Convert.ToString(item.Tag);
            this.lbRole.Text = roleName;
            LoadGridView();
        }

        private void activateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(activateToolStripMenuItem.Checked == false)
            {
                activateToolStripMenuItem.Checked = true;
            }
            else
            {
                activateToolStripMenuItem.Checked = false;
            }
            LoadGridView();
        }

        private void tsmiAdd_Click(object sender, EventArgs e)
        {
            ModifyAccount modifyAccount = new ModifyAccount(items);
            modifyAccount.ShowDialog();
            LoadGridView();
        }

        private void dgvAccount_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvAccount.Rows[e.RowIndex];
            ModifyAccount form = new ModifyAccount(items);
            form.LoadUser(row.Cells[1].Value.ToString());
            form.ShowDialog();
            LoadGridView();
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvAccount.SelectedRows)
            {
                string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                string query = $"UPDATE [RoleAccount] SET [Actived] = 0 WHERE [AccountName] = N'{item.Cells[1].Value.ToString()}' and [RoleID] = {item.Cells[0].Value.ToString()}";
                sqlCommand.CommandText = query;
                sqlConnection.Open();
                int numOfRowsEffected = sqlCommand.ExecuteNonQuery();
                if (numOfRowsEffected > 0)
                {
                    MessageBox.Show("Xóa thành công");
                    LoadGridView();
                }
                else
                {
                    MessageBox.Show("Lỗi");
                }
                sqlConnection.Close();
            }

        }

        private void LoadGridView()
        {
            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string activate = activateToolStripMenuItem.Checked == false ? "" : "and [Actived]=1";
            string query = $"SELECT * FROM [RoleAccount] WHERE [RoleID] = {roleID} "+activate;
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("RoleAccount");
            da.Fill(dt);
            dgvAccount.DataSource = dt;
            sqlConnection.Close();
            sqlConnection.Dispose();
            da.Dispose();
            TranslateToVietnamese();
        }

        private void TranslateToVietnamese()
        {
            dgvAccount.Columns["RoleID"].HeaderText = "Mã số TK";
            dgvAccount.Columns["AccountName"].HeaderText = "Tên người dùng";
            dgvAccount.Columns["Actived"].HeaderText = "Kích hoạt";
            dgvAccount.Columns["Notes"].HeaderText = "Ghi chú";
        }
    }
}
