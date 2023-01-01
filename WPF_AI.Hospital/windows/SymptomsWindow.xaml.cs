using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using WPF_AI.Hospital.database;
using WPF_AI.Hospital.exceptions;
using WPF_AI.Hospital.model;

namespace WPF_AI.Hospital.windows
{
    /// <summary>
    /// Interaction logic for SymptomsWindow.xaml
    /// </summary>
    public partial class SymptomsWindow : Window
    {
        public SymptomsWindow()
        {
            InitializeComponent();

            ShowSymptomsInTable();
            FillComboBoxValues();
        }

        private void ShowSymptomsInTable()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM symptoms;";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using(adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table != null && table.Rows.Count > 0)
                        symptomsDataGrid.ItemsSource = table.DefaultView;
                    else
                        symptomsDataGrid.ItemsSource = null;
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

        private void FillComboBoxValues()
        {
            cbValue.Items.Insert(0, "---SELECT VALUE---");
            
            foreach(Symptom.SymptomValue symptomValue in Enum.GetValues(typeof(Symptom.SymptomValue)))
            {
                cbValue.Items.Add(symptomValue);
            }

            cbValue.SelectedIndex = 0;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM symptoms WHERE name LIKE '%" + tbSearch.Text + "%';";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    symptomsDataGrid.ItemsSource = table.DefaultView;
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
            string tmpQuery = "SELECT * FROM symptoms WHERE name=@name";
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
                    MessageBox.Show($"Symptom with this name ({tbName.Text}) already exists.\nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    CheckIfEmptyInput();

                    Database.OpenConnectionWithDatabase();

                    string query = "INSERT INTO symptoms(name, value) " +
                                    "VALUES(@name, @value);";
                    SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                    command.Parameters.AddWithValue("@name", tbName.Text);
                    command.Parameters.AddWithValue("@value", cbValue.SelectedItem.ToString());

                    command.ExecuteNonQuery();

                    MessageBox.Show($"{tbName.Text} with value {cbValue.SelectedItem.ToString()} is successfully added to database.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (WrongInputException ex)
            {
                MessageBox.Show(ex.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Error when adding new data to database: " + ex2.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
                ShowSymptomsInTable();
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
                    if (symptomsDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)symptomsDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "UPDATE symptoms " +
                                        "SET name='" + tbName.Text + "', " +
                                        "value='" + cbValue.SelectedItem.ToString() + "' " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                        command.ExecuteNonQuery();

                        MessageBox.Show($"{tbName.Text} with value {cbValue.SelectedItem.ToString()} is successfully updated.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when updating symptom data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.sqlConnection.Close();
                ShowSymptomsInTable();
                SetFieldsToNone();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this symptom?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Database.OpenConnectionWithDatabase();

                    CheckIfEmptyInput();
                    if (symptomsDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)symptomsDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "DELETE " +
                                        "FROM symptoms " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                        command.ExecuteNonQuery();

                        MessageBox.Show($"{tbName.Text} with value {cbValue.SelectedItem.ToString()} is successfully deleted.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deleting symptom data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.sqlConnection.Close();
                ShowSymptomsInTable();
                SetFieldsToNone();
            }
        }

        private void CheckIfEmptyInput()
        {
            if (tbName.Text == "" || cbValue.SelectedIndex == 0)
            {
                throw new WrongInputException("Wrong input! Fields can't be empty.");
            }
        }

        private void SetFieldsToNone()
        {
            tbName.Text = string.Empty;
            cbValue.SelectedIndex = 0;
        }

        private void symptomsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (symptomsDataGrid.SelectedItem != null)
                {
                    DataRowView data = (DataRowView)symptomsDataGrid.SelectedItem;

                    tbName.Text = data["name"].ToString();

                    Symptom.SymptomValue sympt = (Symptom.SymptomValue)Enum.Parse(typeof(Symptom.SymptomValue), data["value"].ToString());
                    foreach (Symptom.SymptomValue symptomValue in Enum.GetValues(typeof(Symptom.SymptomValue)))
                    {
                        if (symptomValue.Equals(sympt))
                        {
                            cbValue.SelectedIndex = (int)symptomValue + 1;
                        }
                    }
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

        private void CountiesButton_Click(object sender, RoutedEventArgs e)
        {
            var countiesWindow = new CountiesWindow();
            countiesWindow.Show();
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
