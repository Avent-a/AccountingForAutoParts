using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace AccountingForAutoParts.View
{
    public class Order
    {
        public string Customer { get; set; }
    }

    public partial class Orders : Page
    {
        private string sqlCon = @"Data Source=WIN-FBAVCHIBBBS\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;";

        public string UserName { get; set; }

        public List<Order> ListOrders { get; set; } = new List<Order>();

        public Orders(string userName)
        {
            InitializeComponent();
            UserName = userName;
            DataContext = this;

            LoadSparesData();
        }

        private void LoadSparesData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    // Загрузка данных из таблицы "suppliers"
                    string query = "SELECT [Имя поставщика] FROM suppliers";
                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable suppliersTable = new DataTable();
                    adapter.Fill(suppliersTable);

                    // Очистка ComboBox
                    comboBox.ItemsSource = null;
                    comboBox.Items.Clear();

                    // Добавление имен поставщиков в ComboBox
                    foreach (DataRow row in suppliersTable.Rows)
                    {
                        var customer = row.ItemArray[0].ToString();
                        ListOrders.Add(new Order()
                        {
                            Customer = customer,
                        });
                    }

                    // Привязка списка заказов к ComboBox
                    comboBox.ItemsSource = ListOrders;

                    // Загрузка данных в DataGrid
                    string tableName = $"Orders_{UserName}";
                    query = $"SELECT ID, Запчасть, Поставщик FROM {tableName}";
                    command = new SqlCommand(query, connection);

                    adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    string tableName = $"Orders_{UserName}";

                    // Получаем выделенную строку в DataGrid
                    DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                    if (selectedRow == null)
                    {
                        MessageBox.Show("Выберите запись для удаления");
                        return;
                    }

                    // Получаем ID записи
                    int id = (int)selectedRow["ID"];

                    // Создаем SQL-запрос для удаления записи из таблицы
                    string query = $"DELETE FROM {tableName} WHERE ID = @Id";

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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxId.Text = string.Empty;
            TextBoxDetali.Text = string.Empty;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlCon))
                {
                    connection.Open();

                    string tableName = $"Orders_{UserName}";

                    // Получаем значения полей из текстовых полей
                    string id = TextBoxId.Text;
                    string запчасть = TextBoxDetali.Text;

                    // Проверяем, чтобы обязательные поля были заполнены
                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(запчасть))
                    {
                        MessageBox.Show("Заполните все поля");
                        return;
                    }

                    // Получаем выбранного поставщика из ComboBox
                    Order selectedOrder = comboBox.SelectedItem as Order;
                    if (selectedOrder == null)
                    {
                        MessageBox.Show("Выберите поставщика");
                        return;
                    }

                    string поставщик = selectedOrder.Customer;

                    // Создаем SQL-запрос для добавления записи в таблицу
                    string query = $"INSERT INTO {tableName} (ID, Запчасть, Поставщик) VALUES (@Id, @Запчасть, @Поставщик)";

                    // Создаем команду с параметрами
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Запчасть", запчасть);
                    command.Parameters.AddWithValue("@Поставщик", поставщик);

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обработчик события изменения выбора в ComboBox
        }

        private int GetOrdersCount(string tableName)
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
                MessageBox.Show("Произошла ошибка при получении количества записей: " + ex.Message);
                return 0;
            }
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private string GetRandomOrderID()
        {
            // Генерация случайного идентификатора заказа
            Random random = new Random();
            int orderNumber = random.Next(100000, 999999);
            return $"ORD{orderNumber}";
        }

        private DateTime GetRandomOrderDate()
        {
            // Генерация случайной даты заказа в пределах последних 30 дней
            Random random = new Random();
            int daysOffset = random.Next(0, 30);
            return DateTime.Now.AddDays(-daysOffset);
        }

        private decimal GetRandomOrderAmount()
        {
            // Генерация случайной стоимости заказа в диапазоне от 1000 до 5000 рублей
            Random random = new Random();
            return random.Next(1000, 5000);
        }

        private string GetRandomDeliveryMethod()
        {
            // Генерация случайного способа доставки
            List<string> deliveryMethods = new List<string>()
            {
                "Курьерская доставка",
                "Почта России",
                "Самовывоз",
                "Транспортная компания"
            };

            Random random = new Random();
            int index = random.Next(deliveryMethods.Count);
            return deliveryMethods[index];
        }

        private string GetRandomPaymentMethod()
        {
            // Генерация случайного способа оплаты
            List<string> paymentMethods = new List<string>()
            {
                "Наличные",
                "Банковская карта",
                "Электронные деньги",
                "Перевод на счет"
            };

            Random random = new Random();
            int index = random.Next(paymentMethods.Count);
            return paymentMethods[index];
        }

        private string GetRandomDeliveryDate()
        {
            // Генерация случайной даты доставки в пределах следующих 30 дней
            Random random = new Random();
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = startDate.AddDays(30);
            TimeSpan timeSpan = endDate - startDate;
            int totalDays = random.Next(0, timeSpan.Days);
            DateTime deliveryDate = startDate.AddDays(totalDays);
            return deliveryDate.ToShortDateString();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание документа Word
                string fileName = "Оформление_заказа_" + UserName + ".docx";
                using (WordprocessingDocument document = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
                {
                    // Создание основного содержимого документа
                    MainDocumentPart mainPart = document.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Заголовок документа
                    Paragraph titleParagraph = body.AppendChild(new Paragraph());
                    Run titleRun = titleParagraph.AppendChild(new Run(new Text("Оформление заказа")));
                    titleParagraph.AppendChild(new Break());

                    // Запись значений из DataGrid в документ Word
                    Random random = new Random();
                    DataTable dataTable = ((DataView)dataGrid.ItemsSource).ToTable();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string id = GetRandomOrderID();
                        string sparePart = row["Запчасть"].ToString();
                        string supplier = row["Поставщик"].ToString();
                        string deliveryDate = GetRandomDeliveryDate();

                        Paragraph orderParagraph = body.AppendChild(new Paragraph());

                        // Установка красной строки
                        ParagraphProperties paragraphProperties = orderParagraph.AppendChild(new ParagraphProperties());
                        Indentation indentation = paragraphProperties.AppendChild(new Indentation() { FirstLine = "720" });

                        Run orderRun = orderParagraph.AppendChild(new Run(new Text($"ID: {id};")));
                        orderParagraph.AppendChild(new Break());

                        orderRun = orderParagraph.AppendChild(new Run(new Text($"Запчасть: {sparePart};")));
                        orderParagraph.AppendChild(new Break());

                        orderRun = orderParagraph.AppendChild(new Run(new Text($"Поставщик: {supplier};")));
                        orderParagraph.AppendChild(new Break());

                        orderRun = orderParagraph.AppendChild(new Run(new Text($"Дата доставки: {deliveryDate}")));
                        orderParagraph.AppendChild(new Break());
                        orderParagraph.AppendChild(new Break());
                    }

                    // Генерация случайных данных для остальных полей заказа
                    string orderID = GetRandomOrderID();
                    DateTime orderDate = GetRandomOrderDate();
                    decimal orderAmount = GetRandomOrderAmount();
                    string deliveryMethod = GetRandomDeliveryMethod();
                    string paymentMethod = GetRandomPaymentMethod();

                    // Вывод информации о заказе
                    Paragraph orderInfoParagraph = body.AppendChild(new Paragraph());
                    Run orderInfoRun = orderInfoParagraph.AppendChild(new Run(new Text($"Идентификатор заказа: {orderID}")));
                    orderInfoParagraph.AppendChild(new Break());
                    orderInfoRun = orderInfoParagraph.AppendChild(new Run(new Text($"Дата заказа: {orderDate.ToShortDateString()}")));
                    orderInfoParagraph.AppendChild(new Break());
                    orderInfoRun = orderInfoParagraph.AppendChild(new Run(new Text($"Стоимость заказа: {orderAmount} рублей")));
                    orderInfoParagraph.AppendChild(new Break());
                    orderInfoRun = orderInfoParagraph.AppendChild(new Run(new Text($"Способ доставки: {deliveryMethod}")));
                    orderInfoParagraph.AppendChild(new Break());
                    orderInfoRun = orderInfoParagraph.AppendChild(new Run(new Text($"Способ оплаты: {paymentMethod}")));
                    orderInfoParagraph.AppendChild(new Break());
                }

                // Открытие документа Word
                System.Diagnostics.Process.Start(fileName);

                MessageBox.Show("Оформление заказа успешно сохранено в документе Word и открыто.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при сохранении заказа в документе Word: " + ex.Message);
            }
        }

        private decimal GetRandomPrice()
        {
            // Генерация случайной цены в диапазоне от 100 до 1000 рублей
            Random random = new Random();
            return random.Next(100, 1000);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных из DataGrid
                DataTable dataTable = ((DataView)dataGrid.ItemsSource).ToTable();

                // Проверка наличия данных
                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для создания чека.");
                    return;
                }

                // Генерация текста для чека
                string checkText = "+----------------------------------------+\n";
                checkText += "|          Чек на покупку автозапчастей          |\n";
                checkText += "|                                              |\n";
                checkText += "| Дата: " + DateTime.Now.ToString("dd.MM.yyyy").PadRight(16) + "|\n";
                checkText += "| Время: " + DateTime.Now.ToString("HH:mm:ss").PadRight(15) + "|\n";
                checkText += "|                                              |\n";
                checkText += "|                   Позиции:                   |\n";

                int positionCount = 1;
                decimal totalAmount = 0;

                foreach (DataRow row in dataTable.Rows)
                {
                    string partName = row["Запчасть"].ToString();
                    int quantity = 1; // В данном примере предполагается, что количество всегда равно 1
                    decimal price = GetRandomPrice(); // Здесь можно использовать цену из базы данных или генерировать случайную цену
                    decimal amount = quantity * price;

                    checkText += "| " + positionCount + ". " + partName.PadRight(32) + "|\n";
                    checkText += "|    " + "Кол-во: " + quantity + " x Цена: " + price.ToString("F2").PadLeft(7) + "  |\n";
                    checkText += "|    " + "Сумма: " + amount.ToString("F2").PadLeft(7) + "                      |\n";
                    checkText += "|                                              |\n";

                    positionCount++;
                    totalAmount += amount;
                }

                checkText += "| Общая сумма: " + totalAmount.ToString("F2").PadLeft(14) + "         |\n";
                checkText += "|                                              |\n";
                checkText += "|                                              |\n";
                checkText += "|  Спасибо за покупку! Если у вас возникнут   |\n";
                checkText += "|  вопросы или проблемы, обратитесь в нашу    |\n";
                checkText += "|  службу поддержки по телефону                |\n";
                checkText += "|  +375293212785 или по адресу                  |\n";
                checkText += "|  alexbakulin@iclod.com.                       |\n";
                checkText += "|                                              |\n";
                checkText += "+----------------------------------------------+";

                // Создание и сохранение файла чека
                string folderPath = @"C:\Users\User\Desktop\AccountingForAutoParts\";
                string fileName = folderPath + "Чек_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + UserName + ".docx";

                // Создание папки, если она не существует
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Создание объекта приложения Word
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Visible = false;

                // Создание нового документа Word
                Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add();

                // Добавление текста в документ
                doc.Content.Text = checkText;

                // Сохранение документа
                doc.SaveAs(fileName);
                doc.Close();
                wordApp.Quit();

                MessageBox.Show("Чек успешно создан и сохранен в файле " + fileName);

                // Открытие файла чека
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при создании чека: " + ex.Message);
            }
        }



    }
}
