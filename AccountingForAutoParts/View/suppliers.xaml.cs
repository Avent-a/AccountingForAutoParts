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
using System.Windows.Shapes;

namespace AccountingForAutoParts.View
{
    /// <summary>
    /// Логика взаимодействия для suppliers.xaml
    /// </summary>
    public partial class suppliers : Page
    {
        private string sqlCon = (@"Data Source=WIN-FBAVCHIBBBS\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;");
        private DataTable suppliersDataTable;

        public suppliers()
        {
            InitializeComponent();
            suppliersDataTable = new DataTable();
            DataGridSupplires.ItemsSource = suppliersDataTable.DefaultView;
            LoadGrid();
        }

        public void LoadGrid()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT ID, [имя поставщика] AS SupplierName, [Контактное лицо] AS ContactPerson, телефон AS Phone, адрес AS Address, [электроная почта] AS Email FROM suppliers", connection);
                    suppliersDataTable.Clear(); // Очистка таблицы перед заполнением новых данных
                    dataAdapter.Fill(suppliersDataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Действия при нажатии кнопки "Удалить"
            try
            {
                DataRowView selectedRow = (DataRowView)DataGridSupplires.SelectedItem;
                if (selectedRow != null)
                {
                    int id = (int)selectedRow["ID"];

                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(sqlCon))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand("DELETE FROM suppliers WHERE ID = @ID", connection);
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                        }

                        LoadGrid();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Действия при нажатии кнопки "Сбросить"
            NameID.Text = "";
            Name_Copy.Text = "";
            Name_Copy1.Text = "";
            Name_Copy2.Text = "";
            Name_Copy3.Text = "";
            Name_Copy4.Text = "";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = Name_Copy.Text;
                string name_contact = Name_Copy1.Text;
                string name_phone = Name_Copy2.Text;
                string name_adress = Name_Copy3.Text;
                string name_email = Name_Copy4.Text;
                int id = int.Parse(NameID.Text); // Получаем значение ID из TextBox

                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO suppliers (Id, [имя поставщика], [Контактное лицо], телефон, адрес, [электроная почта]) VALUES (@Id, @Name, @Contact, @Phone, @Address, @Email)", connection);

                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Contact", name_contact);
                    command.Parameters.AddWithValue("@Phone", name_phone);
                    command.Parameters.AddWithValue("@Address", name_adress);
                    command.Parameters.AddWithValue("@Email", name_email);
                    command.ExecuteNonQuery();
                }

                LoadGrid();

                // Очистка полей после добавления записи
                NameID.Text = "";
                Name_Copy.Text = "";
                Name_Copy1.Text = "";
                Name_Copy2.Text = "";
                Name_Copy3.Text = "";
                Name_Copy4.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}