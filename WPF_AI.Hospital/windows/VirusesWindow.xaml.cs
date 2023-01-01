using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_AI.Hospital.database;
using WPF_AI.Hospital.exceptions;
using WPF_AI.Hospital.model;

namespace WPF_AI.Hospital.windows
{
    /// <summary>
    /// Interaction logic for VirusesWindow.xaml
    /// </summary>
    public partial class VirusesWindow : Window
    {

        private Symptom symptom = null;
        private ObservableCollection<Symptom> symptomsList = new ObservableCollection<Symptom>();

        public VirusesWindow()
        {
            InitializeComponent();

            ShowDiseasesInTable();
            ShowAllSymptoms();

        }

        private void ShowDiseasesInTable()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM diseases WHERE is_virus=1";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table != null && table.Rows.Count > 0)
                        virusesDataGrid.ItemsSource = table.DefaultView;
                    else
                        virusesDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when showing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void ShowAllSymptoms()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM symptoms;";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table != null && table.Rows.Count > 0)
                        addSymptomsDataGrid.ItemsSource = table.DefaultView;
                    else
                        addSymptomsDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
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

                string query = "SELECT * FROM diseases WHERE is_virus=1 AND name LIKE '%" + tbSearch.Text + "%';";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    virusesDataGrid.ItemsSource = table.DefaultView;
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
            string tmpQuery = "SELECT * FROM diseases WHERE is_virus=1 AND name=@name";
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
                    MessageBox.Show($"Virus with this name ({tbName.Text}) already exists.\nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    CheckIfEmptyInput();

                    Database.OpenConnectionWithDatabase();

                    string query = "INSERT INTO diseases(name, is_virus) " +
                                    "OUTPUT INSERTED.ID " +
                                    "VALUES(@name, 1);";
                    SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                    command.Parameters.AddWithValue("@name", tbName.Text);
                    int id = (int)command.ExecuteScalar();

                    AddToDiseaseSymptomDatabaseTable(id);

                    MessageBox.Show($"{tbName.Text} is successfully added to database.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ShowDiseasesInTable();
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
                    if (virusesDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)virusesDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "UPDATE diseases " +
                                        "SET name='" + tbName.Text + "' " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                        command.ExecuteNonQuery();

                        RemoveFromDiseaseSymptomDatabaseTable(id);
                        AddToDiseaseSymptomDatabaseTable(id);

                        MessageBox.Show($"{tbName.Text} is successfully updated.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when updating virus data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
                ShowDiseasesInTable();
                SetFieldsToNone();
            }
        }

        private static void RemoveFromDiseaseSymptomDatabaseTable(int id)
        {
            SqlConnection newConnection = new SqlConnection(Database.connectionString);
            newConnection.Open();
            string tmpQuery = "DELETE FROM disease_symptom " +
                                "WHERE disease_id=" + id;
            SqlCommand tmpCommand = new SqlCommand(tmpQuery, newConnection);
            tmpCommand.ExecuteNonQuery();
            newConnection.Close();
        }

        private void AddToDiseaseSymptomDatabaseTable(int id)
        {
            SqlConnection newConnection = new SqlConnection(Database.connectionString);
            newConnection.Open();
            foreach (Symptom symptom in symptomsList)
            {
                string tmpQuery = "INSERT INTO disease_symptom(disease_id, symptom_id) " +
                                    "VALUES(@diseaseID,@symptomID)";
                SqlCommand tmpCommand = new SqlCommand(tmpQuery, newConnection);
                tmpCommand.Parameters.AddWithValue("@diseaseID", id);
                tmpCommand.Parameters.AddWithValue("@symptomID", symptom.Id);
                tmpCommand.ExecuteNonQuery();
            }
            newConnection.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this virus?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Database.OpenConnectionWithDatabase();

                    CheckIfEmptyInput();
                    if (virusesDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)virusesDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "DELETE " +
                                        "FROM diseases " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                        command.ExecuteNonQuery();

                        MessageBox.Show($"{tbName.Text} is successfully deleted.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deleting virus data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.sqlConnection.Close();
                ShowDiseasesInTable();
                SetFieldsToNone();
            }
        }

        private void CheckIfEmptyInput()
        {
            if (tbName.Text == "" || symptomsList.Count == 0)
            {
                throw new WrongInputException("Wrong input! Name must be entered and at least 1 symptom must be selected.");
            }
        }

        private void ShowAddedSymptoms()
        {
            addedSymptomsDataGrid.ItemsSource = symptomsList;
        }

        private void SetFieldsToNone()
        {
            tbName.Text = string.Empty;
            symptomsList.Clear();
            ShowAddedSymptoms();
        }

        private void diseasesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (virusesDataGrid.SelectedItem != null)
                {
                    Database.OpenConnectionWithDatabase();

                    DataRowView data = (DataRowView)virusesDataGrid.SelectedItem;

                    int id = (int)data["id"];
                    ShowSymptomsOfSelectedVirus(id);

                    tbName.Text = (string)data["name"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when showing selected data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void ShowSymptomsOfSelectedVirus(int id)
        {
            string query = "SELECT * FROM symptoms s " +
                            "INNER JOIN disease_symptom ds ON ds.symptom_id = s.id " +
                            "WHERE ds.disease_id=" + id;
            SqlCommand command = new SqlCommand(query, Database.sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            SqlDataReader reader = command.ExecuteReader();
            ObservableCollection<Symptom> tmpSymptomList = new ObservableCollection<Symptom>();
            using (reader)
            {
                while (reader.Read())
                {
                    int symptomId = (int)reader["id"];
                    string symptomName = (string)reader["name"];
                    string symptomValue = (string)reader["value"];

                    tmpSymptomList.Add(new Symptom(symptomId, symptomName, symptomValue));
                }

                addedSymptomsDataGrid.ItemsSource = tmpSymptomList;
            }

            using (adapter)
            {
                DataTable table = new DataTable();
                adapter.Fill(table);

                symptomsOfVirusDataGrid.ItemsSource = tmpSymptomList;
            }
            symptomsList = tmpSymptomList;
        }

        private void addSymptomsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (addSymptomsDataGrid.SelectedItem != null)
                {
                    DataRowView data = (DataRowView)addSymptomsDataGrid.SelectedItem;

                    int id = (int)data["id"];
                    string name = (string)data["name"];
                    string value = (string)data["value"];

                    symptom = new Symptom(id, name, value);
                }
                else
                {
                    SetFieldsToNone();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when choosing symptoms: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            symptomsList.Add(symptom);

            ShowAddedSymptoms();
        }

        private void RemoveSymptoms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this symptom?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Symptom data = (Symptom)addedSymptomsDataGrid.SelectedItem;

                foreach (Symptom symptom in symptomsList)
                {
                    if (symptom.Id == data.Id)
                    {
                        symptomsList.Remove(symptom);
                        break;
                    }
                }
            }
            ShowAddedSymptoms();
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
