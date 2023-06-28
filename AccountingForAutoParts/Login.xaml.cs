using AccountingForAutoParts.View;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccountingForAutoParts
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private const string connectionString = @"Data Source=WIN-FBAVCHIBBBS\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;";

        public Login()
        {
            InitializeComponent();
        }

        private void BtnSubmitClick(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                try
                {
                    sqlCon.Open();
                    string query = "SELECT COUNT(1) FROM tblUser WHERE Username=@Username AND Password=@Password";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@Username", username);
                    sqlCmd.Parameters.AddWithValue("@Password", password);

                    int count = Convert.ToInt32(sqlCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        MainWindow mainWindow = new MainWindow(username, password);
                        mainWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Неправильное имя пользователя или пароль.");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
