using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace AccountingForAutoParts.View
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private const string connectionString = @"Data Source=WIN-FBAVCHIBBBS\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;";

        public string UserName { get; set; }
        public string Password { get; set; }

        public Home(string userName, string password)
        {
            InitializeComponent();
            UserName = userName;
            Password = password;
            DataContext = this;

            // Получение количества записей в таблицах
            int ordersCount = GetOrdersCount($"Orders_{UserName}");
            int sparepartsCount = GetSparepartsCount(UserName);

            // Установка значений в Label
            OrdersLabel.Content = $"Заказа: {ordersCount}";
            SparepartsLabel.Content = $"Запчасти: {sparepartsCount}";
        }

        private int GetOrdersCount(string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT COUNT(*) FROM {tableName}";
                    SqlCommand command = new SqlCommand(query, connection);

                    int count = (int)command.ExecuteScalar();

                    return count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при получении количества заказов: " + ex.Message);
                return 0;
            }
        }

        private int GetSparepartsCount(string userName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT COUNT(*) FROM {userName}";
                    SqlCommand command = new SqlCommand(query, connection);

                    int count = (int)command.ExecuteScalar();

                    return count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при получении количества запчастей: " + ex.Message);
                return 0;
            }
        }
    }
}
