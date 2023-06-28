using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Логика взаимодействия для Spares.xaml
    /// </summary>
    public partial class Spares : Page
    {
        private string sqlCon = (@"Data Source=WIN-FBAVCHIBBBS\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;");

        public string UserName { get; set; }

        public Spares(string userName)
        {
            InitializeComponent();
            UserName = userName;
            DataContext = this;

            // Получение количества запчастей в таблице
            int sparepartsCount = GetSparepartsCount($"[{UserName}]");


            LoadSparesData();
        }

        private int GetSparepartsCount(string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
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
                MessageBox.Show("Произошла ошибка при получении количества запчастей: " + ex.Message);
                return 0;
            }
        }

        private void LoadSparesData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    string tableName = $"[{UserName}]"; // Используем имя таблицы из UserName

                    string query = $"SELECT Id, Наиминование, Цена, [марка авто] FROM {tableName}";

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DataGridSpares.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Обработчик события нажатия на кнопку "Добавить"
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    string tableName = $"[{UserName}]"; // Используем имя таблицы из UserName

                    // Получаем значения полей из текстовых полей
                    string id = TextBoxId.Text;
                    string наименование = TextBoxName.Text;
                    string цена = TextBoxPrice.Text;
                    string марка = TextBoxCarBrand.Text;

                    // Проверяем, чтобы обязательные поля были заполнены
                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(наименование) || string.IsNullOrEmpty(цена) || string.IsNullOrEmpty(марка))
                    {
                        MessageBox.Show("Заполните все поля");
                        return;
                    }

                    // Создаем SQL-запрос для добавления записи в таблицу
                    string query = $"INSERT INTO {tableName} (Id, Наиминование, Цена, [марка авто]) VALUES (@Id, @Наименование, @Цена, @Марка)";

                    // Создаем команду с параметрами
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Наименование", наименование);
                    command.Parameters.AddWithValue("@Цена", цена);
                    command.Parameters.AddWithValue("@Марка", марка);

                    // Выполняем команду
                    command.ExecuteNonQuery();

                    // Перезагружаем данные
                    LoadSparesData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении записи: " + ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Обработчик события нажатия на кнопку "Сбросить"
            // Очищаем значения текстовых полей
            TextBoxId.Text = string.Empty;
            TextBoxName.Text = string.Empty;
            TextBoxPrice.Text = string.Empty;
            TextBoxCarBrand.Text = string.Empty;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Обработчик события нажатия на кнопку "Удалить"
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    string tableName = $"[{UserName}]"; // Используем имя таблицы из UserName

                    // Получаем выделенную строку в DataGrid
                    DataRowView selectedRow = (DataRowView)DataGridSpares.SelectedItem;
                    if (selectedRow == null)
                    {
                        MessageBox.Show("Выберите запись для удаления");
                        return;
                    }

                    // Получаем ID записи
                    int id = (int)selectedRow["Id"];

                    // Создаем SQL-запрос для удаления записи из таблицы
                    string query = $"DELETE FROM {tableName} WHERE Id = @Id";

                    // Создаем команду с параметром
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    // Выполняем команду
                    command.ExecuteNonQuery();

                    // Перезагружаем данные
                    LoadSparesData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при удалении записи: " + ex.Message);
            }
        }

        private void DataGridSpares_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    string tableName = $"[{UserName}]"; // Используем имя таблицы из UserName

                    // Получаем значение из текстового поля поиска
                    string searchText = TextBoxSearch.Text;

                    // Создаем SQL-запрос для поиска записей в таблице
                    string query = $"SELECT Id, Наиминование, Цена, [марка авто] FROM {tableName} WHERE Наиминование LIKE '%' + @SearchText + '%'";

                    // Создаем команду с параметром
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchText", searchText);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DataGridSpares.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении поиска: " + ex.Message);
            }
        }
    }
}
