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
    public partial class ModifyAccount : Form
    {
        private bool used = false;
        public List<string> Roles;
        public Dictionary<string, int> hashRoles;

        public ModifyAccount(List<string> Roles)
        {
            this.Roles = Roles;
            hashRoles = new Dictionary<string, int>();
            InitializeComponent();
            CreateAccount_Loading();
        }

        private void CreateAccount_Loading()
        {
            foreach (var item in Roles)
            {
                clbRoles.Items.Add(item.Split('^')[0]);
                hashRoles.Add(item.Split('^')[0], Convert.ToInt32(item.Split('^')[1]));
            }
        }

        private void UpdateUser()
        {
            string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = $"set dateformat dmy UPDATE [Account] SET " +
                $" [Password] = N'{txtPass.Text}'," +
                $" [FullName] =  N'{txtFullName.Text}', " +
                $"[Email] = N'{txtEmail.Text}', " +
                $"[Tell] = '{txtNumber.Text}', " +
                $"[DateCreated]  = '{dtpDateCreated.Value.ToString("dd/MM/yyyy")}'" +
                $"where [AccountName] = N'{txtAccountName.Text}'";
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            int numOfRowsEffected = 0;
            numOfRowsEffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            sqlCommand.CommandText = $"Delete from [RoleAccount]  where [AccountName] = N'{txtAccountName.Text}'";
            sqlConnection.Open();
            numOfRowsEffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            foreach (var item in clbRoles.CheckedItems)
            {
                sqlCommand.CommandText = $"INSERT [RoleAccount] ([RoleID], [AccountName], [Actived], [Notes]) VALUES ({hashRoles[item.ToString()]}, N'{txtAccountName.Text}', 1, NULL)";
                sqlConnection.Open();
                numOfRowsEffected = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            if (numOfRowsEffected > 0)
            {
                MessageBox.Show("Chỉnh sửa thành công");
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (used == true) UpdateUser();
            else CreateNewAccount();

        }

        public void CreateNewAccount()
        {
            string connectionString = "server=localhost; database = RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            string query = $"set dateformat dmy INSERT INTO [Account] ([AccountName], [Password], [FullName], [Email], [Tell], [DateCreated]) VALUES ( N'{txtAccountName.Text}', N'{txtPass.Text}', N'{txtFullName.Text}', N'{txtEmail.Text}','{txtNumber.Text}', '{dtpDateCreated.Value.ToString("dd/MM/yyyy")}')";
            sqlCommand.CommandText = query;
            sqlConnection.Open();
            int numOfRowsEffected = 0;
            numOfRowsEffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            foreach (var item in clbRoles.CheckedItems)
            {
                sqlCommand.CommandText = $"INSERT [RoleAccount] ([RoleID], [AccountName], [Actived], [Notes]) VALUES ({hashRoles[item.ToString()]}, N'{txtAccountName.Text}', 1, NULL)";
                sqlConnection.Open();
                numOfRowsEffected = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            if (numOfRowsEffected > 0)
            {
                MessageBox.Show("Tạo thành công");
                this.Close();
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Lỗi");
            }
        }

        internal void LoadUser(string account)
        {
            used = true;

            string connectionString = "server=hotarou; database=RestaurantManagement; Integrated Security = true;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            SqlCommand sqlCommand2 = sqlConnection.CreateCommand();

            string query = $"SELECT [AccountName], [Password], [FullName], [Email], [Tell], [DateCreated] FROM [Account] WHERE [AccountName] = N'{account}'";
            sqlCommand.CommandText = query;
            sqlCommand2.CommandText = $"SELECT [RoleID], [AccountName], [Actived], [Notes] FROM [RoleAccount] WHERE [AccountName]=N'{account}'";
            sqlConnection.Open();
            SqlDataReader read1 = sqlCommand.ExecuteReader();
            while (read1.Read())
            {
                txtAccountName.Text = read1["AccountName"].ToString();
                txtPass.Text = read1["Password"].ToString();
                txtFullName.Text = read1["FullName"].ToString();
                txtEmail.Text = read1["Email"].ToString();
                txtNumber.Text = read1["Tell"].ToString();
                dtpDateCreated.Value = read1["DateCreated"].ToString() == "" ? DateTime.Now : Convert.ToDateTime(read1["DateCreated"].ToString());
            }
            sqlConnection.Close();
            sqlConnection.Open();
            SqlDataReader read2 = sqlCommand2.ExecuteReader();
            while (read2.Read())
            {
                for (int i = 0; i < clbRoles.Items.Count; i++)
                {
                    if (hashRoles[clbRoles.Items[i].ToString()] == Convert.ToInt32(read2["RoleID"].ToString()))
                    {
                        clbRoles.SetItemChecked(i, true);
                        //rolesOfUser.Add(read2["RoleID"].ToString());
                    }
                }
            }
            //dgvBills.DataSource = dt;
            sqlConnection.Close();
        }
    }
}
