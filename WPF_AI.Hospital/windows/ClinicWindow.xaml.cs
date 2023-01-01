using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPF_AI.Hospital.database;
using WPF_AI.Hospital.generics;
using WPF_AI.Hospital.model;
using WPF_AI.Hospital.sort;

namespace WPF_AI.Hospital.windows
{
    /// <summary>
    /// Interaction logic for ClinicWindow.xaml
    /// </summary>
    /// 
    public partial class ClinicWindow : Window
    {
        private int NumberOfInfectedToday;

        private List<County> countiesList = new List<County>();
        private List<Symptom> symptomsList = new List<Symptom>();
        private List<Disease> diseasesList = new List<Disease>();
        private List<Person> peopleList = new List<Person>();

        public ClinicWindow()
        {
            InitializeComponent();

            ImportCountiesFromDatabase();
            ImportSymptomsFromDatabase();
            ImportDiseasesFromDatabase();
            ImportPeopleFromDatabase();

            MostAffectedCounty();

            FindNumberOfInfectedToday();

            ShowVirusesInTable();

        }

        private void ImportCountiesFromDatabase()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM counties";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                using(reader)
                {
                    while(reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        int population = (int)reader["population"];
                        int affectedPopulation = (int)reader["affected_population"];

                        countiesList.Add(new County(id, name, population, affectedPopulation));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error when importing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void ImportSymptomsFromDatabase()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM symptoms";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        string value = (string)reader["value"];

                        symptomsList.Add(new Symptom(id, name, value));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when importing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void ImportDiseasesFromDatabase()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM diseases";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        bool isVirus = (bool)reader["is_virus"];

                        SqlConnection tmpConnection = new SqlConnection(Database.connectionString);
                        tmpConnection.Open();
                        string tmpQuery = "SELECT * FROM disease_symptom WHERE id=" + id;
                        SqlCommand tmpCommand = new SqlCommand(tmpQuery, tmpConnection);
                        SqlDataReader tmpReader = tmpCommand.ExecuteReader();
                        List<Symptom> tmpSymptomsList = new List<Symptom>();
                        using(tmpReader)
                        {
                            while(tmpReader.Read())
                            {
                                int symptomID = (int)tmpReader["symptom_id"];
                                tmpSymptomsList.Add(symptomsList.Find(symp => symp.Id == symptomID));
                            }
                        }
                        tmpConnection.Close();

                        if (isVirus)
                            diseasesList.Add(new Virus(id, name, tmpSymptomsList));
                        else
                            diseasesList.Add(new Disease(id, name, isVirus, tmpSymptomsList));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when importing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void ImportPeopleFromDatabase()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM people";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                using(reader)
                {
                    while(reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["first_name"];
                        string surname = (string)reader["last_name"];
                        int age = (int)reader["age"];
                        int countyID = (int)reader["county_id"];
                        int diseaseID = (int)reader["disease_id"];
                        County county = countiesList.Find(cnty => cnty.Id == countyID);
                        Disease disease = diseasesList.Find(dis => dis.Id == diseaseID);

                        SqlConnection tmpConnection = new SqlConnection(Database.connectionString);
                        tmpConnection.Open();
                        string tmpQuery = "SELECT * FROM contacted_people WHERE id=" + id;
                        SqlCommand tmpCommand = new SqlCommand(tmpQuery, tmpConnection);
                        SqlDataReader tmpReader = tmpCommand.ExecuteReader();
                        List<Person> tmpContactsList = new List<Person>();
                        if (tmpReader.HasRows)
                        {
                            using (tmpReader)
                            {
                                while (tmpReader.Read())
                                {
                                    int contactID = (int)tmpReader["contact_id"];
                                    tmpContactsList.Add(peopleList.Find(pers => pers.Id == contactID));
                                }
                            }
                        }
                        tmpConnection.Close();

                        peopleList.Add(new Person(id, name, surname, age, county, disease, tmpContactsList));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error when importing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void MostAffectedCounty()
        {
            countiesList.Sort(new MostAffectedInCounty());
            tbAffectedCounty.Text = string.Format(countiesList[0].Name + " " + Math.Round(countiesList[0].PercentageOfAffectedPopulation, 2) + "%.");
        }

        private void FindNumberOfInfectedToday()
        {
            Random randomNumber = new Random();
            NumberOfInfectedToday = randomNumber.Next(0, 500);
            tbTodayNumAffected.Text = NumberOfInfectedToday.ToString();
        }

        private void ShowVirusesInTable()
        {
            try
            {
                Database.OpenConnectionWithDatabase();

                string query = "SELECT * FROM diseases WHERE is_virus=1";
                SqlCommand command = new SqlCommand(query, Database.sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using(adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    virusesDataGrid.ItemsSource = table.DefaultView;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error when importing data from database: " + ex.Message, "AI Hospital", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnectionWithDatabase();
            }
        }

        private void virusesDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (virusesDataGrid.SelectedItem != null)
                {
                    Database.OpenConnectionWithDatabase();

                    DataRowView data = (DataRowView)virusesDataGrid.SelectedItem;

                    int id = (int)data["id"];
                    ShowPeopleSufferingFromSelectedVirus(id);
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

        private void ShowPeopleSufferingFromSelectedVirus(int id)
        {
            string query = "SELECT * FROM people WHERE disease_id=" + id;
            SqlCommand command = new SqlCommand(query, Database.sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            using(adapter)
            {
                DataTable table = new DataTable();
                adapter.Fill(table);

                sufferersDataGrid.ItemsSource = table.DefaultView;
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


    }
}
