using System;
using System.Collections.ObjectModel;
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
    /// Interaction logic for PeopleWindow.xaml
    /// </summary>
    public partial class PeopleWindow : Window
    {

        private Person person = null;
        private ObservableCollection<Person> contactsList = new ObservableCollection<Person>();

        public PeopleWindow()
        {
            InitializeComponent();

            ShowPeopleInTable();
            ShowAllContacts();
            SelectCounty();
            SelectDisease();

        }

        private void ShowPeopleInTable()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT p.id, p.first_name, p.last_name, p.age, c.name AS c_name, d.name AS d_name " +
                                "FROM people p " +
                                "INNER JOIN counties c ON p.county_id = c.id " +
                                "INNER JOIN diseases d ON p.disease_id = d.id";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table != null && table.Rows.Count > 0)
                        peopleDataGrid.ItemsSource = table.DefaultView;
                    else
                        peopleDataGrid.ItemsSource = null;
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

        private void ShowAllContacts()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM people;";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table != null && table.Rows.Count > 0)
                        addContactsDataGrid.ItemsSource = table.DefaultView;
                    else
                        addContactsDataGrid.ItemsSource = null;
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

        private void SelectCounty()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM counties";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    cbCounty.SelectedValuePath = "id";
                    cbCounty.DisplayMemberPath = "name";
                    cbCounty.ItemsSource = table.DefaultView;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error when getting data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void SelectDisease()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM diseases";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    cbDisease.SelectedValuePath = "id";
                    cbDisease.DisplayMemberPath = "name";
                    cbDisease.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when getting data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
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

                string query = "SELECT p.id, p.first_name, p.last_name, p.age, c.name AS c_name, d.name AS d_name " +
                                "FROM people p " +
                                "INNER JOIN counties c ON p.county_id = c.id " +
                                "INNER JOIN diseases d ON p.disease_id = d.id " +
                                "WHERE first_name LIKE '%" + tbSearch.Text + "%' OR last_name LIKE '%" + tbSearch.Text + "%'";

                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    peopleDataGrid.ItemsSource = table.DefaultView;
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

        private bool CheckIfAlreadyExists(string name, string surname)
        {
            SqlConnection connection = new SqlConnection(Database.connectionString);
            connection.Open();
            string tmpQuery = "SELECT * FROM people WHERE first_name=@name AND last_name=@surname";
            SqlCommand command = new SqlCommand(tmpQuery, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);

            SqlDataReader reader = command.ExecuteReader();
            return reader.HasRows ? true : false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckIfAlreadyExists(tbFirstName.Text, tbLastName.Text))
                    MessageBox.Show($"Person with this name ({tbFirstName.Text} {tbLastName.Text}) already exists.\nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    CheckIfEmptyInput();

                    Database.OpenConnectionWithDatabase();

                    string query = "INSERT INTO people(first_name, last_name, age, county_id, disease_id) " +
                                    "OUTPUT INSERTED.ID " +
                                    "VALUES(@firstName, @lastName, @age, @countyID, @diseaseID)";
                    SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                    command.Parameters.AddWithValue("@firstName", tbFirstName.Text);
                    command.Parameters.AddWithValue("@lastName", tbLastName.Text);
                    command.Parameters.AddWithValue("@age", int.Parse(tbAge.Text));
                    command.Parameters.AddWithValue("@countyID", cbCounty.SelectedValue);
                    command.Parameters.AddWithValue("@diseaseID", cbDisease.SelectedValue);

                    int id = (int)command.ExecuteScalar();

                    AddToContactedPeopleDatabaseTable(id);

                    MessageBox.Show($"{tbFirstName.Text} {tbLastName.Text} is successfully added to database.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ShowPeopleInTable();
                SetFieldsToNone();
                ShowAllContacts();
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
                    if (peopleDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)peopleDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "UPDATE people " +
                                        "SET first_name='" + tbFirstName.Text + "', " +
                                        "last_name='" + tbLastName.Text + "', " +
                                        "age=" + tbAge.Text + ", " +
                                        "county_id=" + cbCounty.SelectedValue + ", " +
                                        "disease_id=" + cbDisease.SelectedValue + " " +
                                        "WHERE id=" + id;
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                        command.ExecuteNonQuery();

                        RemoveFromContactedPeopleDatabaseTable(id);
                        AddToContactedPeopleDatabaseTable(id);

                        MessageBox.Show($"{tbFirstName.Text} {tbLastName.Text} is successfully updated.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when updating person data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
                ShowPeopleInTable();
                SetFieldsToNone();
                ShowAllContacts();
            }
        }

        private static void RemoveFromContactedPeopleDatabaseTable(int id)
        {
            SqlConnection newConnection = new SqlConnection(Database.connectionString);
            newConnection.Open();
            string tmpQuery = "DELETE FROM contacted_people " +
                                "WHERE person_id=" + id;
            SqlCommand tmpCommand = new SqlCommand(tmpQuery, newConnection);
            tmpCommand.ExecuteNonQuery();
            newConnection.Close();
        }

        private void AddToContactedPeopleDatabaseTable(int id)
        {
            SqlConnection newConnection = new SqlConnection(Database.connectionString);
            newConnection.Open();
            foreach (Person pers in contactsList)
            {
                string tmpQuery = "INSERT INTO contacted_people(person_id, contact_id) " +
                                    "VALUES(@personID,@contactID)";
                SqlCommand tmpCommand = new SqlCommand(tmpQuery, newConnection);
                tmpCommand.Parameters.AddWithValue("@personID", id);
                tmpCommand.Parameters.AddWithValue("@contactID", pers.Id);
                tmpCommand.ExecuteNonQuery();
            }
            newConnection.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this person?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Database.OpenConnectionWithDatabase();

                    CheckIfEmptyInput();
                    if (peopleDataGrid.SelectedItem != null)
                    {
                        DataRowView data = (DataRowView)peopleDataGrid.SelectedItem;
                        int id = Convert.ToInt32(data["id"]);

                        string query = "DELETE " +
                                        "FROM people " +
                                        "WHERE id=" + id + ";";
                        SqlCommand command = new SqlCommand(query, Database.sqlConnection);

                        command.ExecuteNonQuery();

                        MessageBox.Show($"{tbFirstName.Text} {tbLastName.Text} is successfully deleted.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (WrongInputException exception)
            {
                MessageBox.Show(exception.Message + " \nPlease try again.", "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deleting person data: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.sqlConnection.Close();
                ShowPeopleInTable();
                SetFieldsToNone();
                ShowAllContacts();
            }
        }

        private void CheckIfEmptyInput()
        {
            if (tbFirstName.Text == "" || tbLastName.Text == "" || !CheckForDigits(tbAge.Text) || cbCounty.SelectedIndex < 0 || cbDisease.SelectedIndex < 0)
            {
                throw new WrongInputException("Wrong input! Please enter all information and make sure correct value is entered in particular field.");
            }
        }

        private void ShowAddedContacts()
        {
            addedContactsDataGrid.ItemsSource = contactsList;
        }

        private void SetFieldsToNone()
        {
            tbFirstName.Text = string.Empty;
            tbLastName.Text = string.Empty;
            tbAge.Text = string.Empty;
            cbCounty.SelectedIndex = -1;
            cbDisease.SelectedIndex = -1;
            contactsList.Clear();
            ShowAddedContacts();
        }

        private bool CheckForDigits(string age)
        {
            bool onlyDigits = true;
            for(int i = 0; i < age.Length; i++)
            {
                if (age[i] < '0' || age[i] > '9')
                    onlyDigits = false;
            }
            return onlyDigits;
        }

        private void peopleDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (peopleDataGrid.SelectedItem != null)
                {
                    Database.OpenConnectionWithDatabase();

                    DataRowView data = (DataRowView)peopleDataGrid.SelectedItem;

                    int id = (int)data["id"];
                    ShowContactsOfSelectedPerson(id);

                    tbFirstName.Text = (string)data["first_name"];
                    tbLastName.Text = (string)data["last_name"];
                    tbAge.Text = data["age"].ToString();
                    cbCounty.SelectedValue = ShowCountyInComboBox((string)data["c_name"]);
                    cbDisease.SelectedValue = ShowDiseaseInComboBox((string)data["d_name"]);
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

        private int ShowCountyInComboBox(string name)
        {
            SqlConnection newConnetion = new SqlConnection(Database.connectionString);
            newConnetion.Open();
            string query = "SELECT id FROM counties WHERE name='" + name + "'";
            SqlCommand command = new SqlCommand(query, Database.sqlConnection);
            int id = (int)command.ExecuteScalar();
            newConnetion.Close();
            return id;
        }

        private int ShowDiseaseInComboBox(string name)
        {
            SqlConnection newConnetion = new SqlConnection(Database.connectionString);
            newConnetion.Open();
            string query = "SELECT id FROM diseases WHERE name='" + name + "'";
            SqlCommand command = new SqlCommand(query, Database.sqlConnection);
            int id = (int)command.ExecuteScalar();
            newConnetion.Close();
            return id;
        }

        private void ShowContactsOfSelectedPerson(int id)
        {
            string query = "SELECT p.id, p.first_name, p.last_name, p.age, p.county_id, p.disease_id " +
                            "FROM contacted_people cp " +
                            "INNER JOIN people p ON p.id = cp.contact_id " +
                            "WHERE cp.person_id=" + id;
            SqlCommand command = new SqlCommand(query, Database.sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            SqlDataReader reader = command.ExecuteReader();
            ObservableCollection<Person> tmpContactsList = new ObservableCollection<Person>();
            using (reader)
            {
                while (reader.Read())
                {
                    int personId = (int)reader["id"];
                    string personName = (string)reader["first_name"];
                    string personSurname = (string)reader["last_name"];
                    int personAge = (int)reader["age"];
                    County personCounty = FindPersonCounty((int)reader["county_id"]);
                    Disease personDisease = FindPersonDisease((int)reader["disease_id"]);

                    tmpContactsList.Add(new Person(personId, personName, personSurname, personAge, personCounty, personDisease, null));
                }

                addedContactsDataGrid.ItemsSource = tmpContactsList;
            }

            using (adapter)
            {
                DataTable table = new DataTable();
                adapter.Fill(table);

                personContactsDataGrid.ItemsSource = tmpContactsList;
            }
            contactsList = tmpContactsList;
        }

        private County FindPersonCounty(int countyId)
        {
            County county = null;
            SqlConnection newConnection = new SqlConnection(Database.connectionString);
            newConnection.Open();
            string query = "SELECT * FROM counties WHERE id=" + countyId;
            SqlCommand command = new SqlCommand(query, newConnection);
            SqlDataReader reader = command.ExecuteReader();
            using(reader)
            {
                while(reader.Read())
                {
                    int id = (int)reader["id"];
                    string name = (string)reader["name"];
                    int population = (int)reader["population"];
                    int affectedPopulation = (int)reader["affected_population"];

                    county = new County(id, name, population, affectedPopulation);
                }
            }
            newConnection.Close();
            return county;
        }

        private Disease FindPersonDisease(int diseaseId)
        {
            Disease disease = null;
            SqlConnection newConnection = new SqlConnection(Database.connectionString);
            newConnection.Open();
            string query = "SELECT * FROM diseases WHERE id=" + diseaseId;
            SqlCommand command = new SqlCommand(query, newConnection);
            SqlDataReader reader = command.ExecuteReader();
            using (reader)
            {
                while (reader.Read())
                {
                    int id = (int)reader["id"];
                    string name = (string)reader["name"];
                    bool isVirus = (bool)reader["is_virus"];

                    disease = new Disease(id, name, isVirus, null);
                }
            }
            newConnection.Close();
            return disease;
        }

        private void addContactsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (addContactsDataGrid.SelectedItem != null)
                {
                    DataRowView data = (DataRowView)addContactsDataGrid.SelectedItem;

                    int id = (int)data["id"];
                    string firstName = (string)data["first_name"];
                    string lastName = (string)data["last_name"];
                    int age = (int)data["age"];
                    int countyId = (int)data["county_id"];
                    int diseaseId = (int)data["disease_id"];

                    person = new Person(id, firstName, lastName, age, FindPersonCounty(countyId), FindPersonDisease(diseaseId), null);
                }
                else
                {
                    SetFieldsToNone();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when choosing contacts: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveContact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this contact?", "AI Hospital", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Person data = (Person)addedContactsDataGrid.SelectedItem;

                foreach (Person pers in contactsList)
                {
                    if (pers.Id == data.Id)
                    {
                        contactsList.Remove(pers);
                        break;
                    }
                }
            }
            ShowAddedContacts();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            contactsList.Add(person);

            ShowAddedContacts();
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

        private void VirusesButton_Click(object sender, RoutedEventArgs e)
        {
            var virusesWindow = new VirusesWindow();
            virusesWindow.Show();
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
