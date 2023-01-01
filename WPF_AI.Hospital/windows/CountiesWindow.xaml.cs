using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_AI.Hospital.database;
using WPF_AI.Hospital.exceptions;

namespace WPF_AI.Hospital.windows
{
    /// <summary>
    /// Interaction logic for CountiesWindow.xaml
    /// </summary>
    public partial class CountiesWindow : Window
    {
        public CountiesWindow()
        {
            InitializeComponent();

            ShowCountiesInTable();
        }

        private void ShowCountiesInTable()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM counties;";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using(adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table != null && table.Rows.Count > 0)
                        countiesDataGrid.ItemsSource = table.DefaultView;
                    else
                        countiesDataGrid.ItemsSource = null;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error when showing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM counties WHERE name LIKE '%" + tbSearch.Text + "%';";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using(adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    countiesDataGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when showing search matching data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }
        private bool CheckIfAlreadyExists(string name)
        {
            SqlConnection connection = new SqlConnection(Database.connectionString);
            connection.Open();
            string tmpQuery = "SELECT * FROM counties WHERE name=@name";
            SqlCommand command = new SqlCommand(tmpQuery, connection);
            command.Parameters.AddWithValue("@name", name);

            SqlDataReader reader = command.ExecuteReader();
            return reader.HasRows ? true : false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckIfAlreadyExists(tbName.Text))
                    MessageBox.Show($"County with this name ({tbName.Text}) already exists.\nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    CheckIfEmptyInput();

                    Database.OpenConnectionWithDatabase();

                    string query = "INSERT INTO counties(name, population, affected_population) " +
                                    "VALUES(@name, @population, @affected);";
                    SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                    command.Parameters.AddWithValue("@name", tbName.Text);
                    command.Parameters.AddWithValue("@population", tbPopulation.Text);
                    command.Parameters.AddWithValue("@affected", tbAffectedPopulation.Text);

                    command.ExecuteNonQuery();

                    MessageBox.Show($"{tbName.Text} (population -> {tbPopulation.Text}, affected population -> {tbAffectedPopulation.Text}) is successfully added to database.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(WrongInputException ex)
            {
                MessageBox.Show(ex.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception ex2)
            {
                MessageBox.Show("Error when adding new data to database: " + ex2.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
                ShowCountiesInTable();
                SetFieldsToNone();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to update?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    Database.OpenConnectionWithDatabase();

                    CheckIfEmptyInput();
                    if (countiesDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)countiesDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "UPDATE counties " +
                                        "SET name='" + tbName.Text + "', " +
                                        "population=" + tbPopulation.Text + ", " +
                                        "affected_population=" + tbAffectedPopulation.Text + " " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                        command.ExecuteNonQuery();

                        MessageBox.Show($"{tbName.Text} (population -> {tbPopulation.Text}, affected population -> {tbAffectedPopulation.Text}) is successfully updated.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when updating county data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.sqlConnection.Close();
                ShowCountiesInTable();
                SetFieldsToNone();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this county?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Database.OpenConnectionWithDatabase();

                    CheckIfEmptyInput();
                    if (countiesDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)countiesDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "DELETE " +
                                        "FROM counties " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                        command.ExecuteNonQuery();

                        MessageBox.Show($"{tbName.Text} (population -> {tbPopulation.Text}, affected population -> {tbAffectedPopulation.Text}) is successfully deleted.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deleting county data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.sqlConnection.Close();
                ShowCountiesInTable();
                SetFieldsToNone();
            }
        }

        private void CheckIfEmptyInput()
        {
            if (tbName.Text == "" || tbPopulation.Text == "" || tbAffectedPopulation.Text == "")
            {
                throw new WrongInputException("Wrong input! Fields can't be empty.");
            }
        }

        private void SetFieldsToNone()
        {
            tbName.Text = string.Empty;
            tbPopulation.Text = string.Empty;
            tbAffectedPopulation.Text = string.Empty;
        }

        private void countiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (countiesDataGrid.SelectedItem != null)
                {
                    DataRowView data = (DataRowView)countiesDataGrid.SelectedItem;

                    tbName.Text = data["name"].ToString();
                    tbPopulation.Text = data["population"].ToString();
                    tbAffectedPopulation.Text = data["affected_population"].ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when showing selected data in TextBox: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new MainWindow();
            homeWindow.Show();
            this.Close();
        }

        private void SymptomsButton_Click(object sender, RoutedEventArgs e)
        {
            var symptomsWindow = new SymptomsWindow();
            symptomsWindow.Show();
            this.Close();
        }

        private void DiseasesButton_Click(object sender, RoutedEventArgs e)
        {
            var diseasesWindow = new DiseasesWindow();
            diseasesWindow.Show();
            this.Close();
        }

        private void VirusesButton_Click(object sender, RoutedEventArgs e)
        {
            var virusesWindow = new VirusesWindow();
            virusesWindow.Show();
            this.Close();
        }

        private void PeopleButton_Click(object sender, RoutedEventArgs e)
        {
            var peopleWindow = new PeopleWindow();
            peopleWindow.Show();
            this.Close();
        }

        private void ClinicButton_Click(object sender, RoutedEventArgs e)
        {
            var clinicWindow = new ClinicWindow();
            clinicWindow.Show();
            this.Close();
        }
    }
}
