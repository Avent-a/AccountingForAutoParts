using AccountingForAutoParts.viewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AccountingForAutoParts.View
{
    public partial class Customer : Page
    {
        private string sqlCon = @"Data Source=WIN-FBAVCHIBBBS\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;";

        public Customer()
        {
            InitializeComponent();
            LoadGrid();
        }

        public void LoadGrid()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM tblUser", connection);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    DataGridAccounts.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(sqlCon))
                    {
                        string userName = Name.Text.Replace(" ", "_"); // Замена пробелов в имени пользователя на символ подчеркивания
                        string query = "INSERT INTO tblUser (UserID, UserName, Password) VALUES (@UserID, @UserName, @Password)";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@UserID", ID.Text);
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Password", Password.Text);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        ClearData();
                        LoadGrid();

                        // Создание таблицы с именем пользователя
                        string createTableQuery = $"CREATE TABLE [{userName}] (ID INT, Наименование NVARCHAR(50), Цена NVARCHAR(50), [Марка авто] NVARCHAR(50))"; // Используйте квадратные скобки для обозначения имени таблицы
                        SqlCommand createTableCmd = new SqlCommand(createTableQuery, connection);
                        createTableCmd.ExecuteNonQuery();

                        // Создание таблицы заказов пользователя
                        string createOrdersTableQuery = $"CREATE TABLE [Orders_{userName}] (ID INT, Запчасть NVARCHAR(100), Поставщик NVARCHAR(100))"; // Исправлено: добавлено пропущенное ключевое слово "NVARCHAR"
                        SqlCommand createOrdersTableCmd = new SqlCommand(createOrdersTableQuery, connection);
                        createOrdersTableCmd.ExecuteNonQuery();

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(sqlCon))
                    {
                        string query = "UPDATE tblUser SET UserName = @UserName, Password = @Password WHERE UserID = @UserID";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@UserID", ID.Text);
                        cmd.Parameters.AddWithValue("@UserName", Name.Text);
                        cmd.Parameters.AddWithValue("@Password", Password.Text);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        ClearData();
                        LoadGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при редактировании данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ID.Text))
                {
                    using (SqlConnection connection = new SqlConnection(sqlCon))
                    {
                        string query = "DELETE FROM tblUser WHERE UserID = @UserID";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@UserID", ID.Text);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        ClearData();
                        LoadGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ClearData()
        {
            ID.Clear();
            Name.Clear();
            Password.Clear();
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(ID.Text))
            {
                MessageBox.Show("Введите ID", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(Name.Text))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(Password.Text))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ClearData();
        }
    }
}
