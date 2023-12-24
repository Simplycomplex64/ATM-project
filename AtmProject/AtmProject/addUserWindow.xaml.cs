using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using System.Windows.Shapes;

namespace AtmProject
{
    /// <summary>
    /// Interaction logic for addUserWindow.xaml
    /// </summary>
    public partial class addUserWindow : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public addUserWindow(UtilisateurActif actif)
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            utilisateur = actif;
            ouverture = true;
        }

        private void addUserBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the database connection
                connexion.Open();

                // Create the SQL command for calling the stored procedure
                using (SqlCommand command = new SqlCommand("CreateUserWithAccounts", connexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    command.Parameters.AddWithValue("@NIP", nipTextBox.Text);
                    command.Parameters.AddWithValue("@Nom", lastNameTextBox.Text);
                    command.Parameters.AddWithValue("@Prenom", firstNameTextBox.Text);
                    command.Parameters.AddWithValue("@Telephone", telTextBox.Text);
                    command.Parameters.AddWithValue("@Email", emailTextBox.Text);
                    command.Parameters.AddWithValue("@Role", roleTextBox.Text);
                    command.Parameters.AddWithValue("@Status", statusTextBox.Text);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    MessageBox.Show("User created successfully.");

                    nipTextBox.Text = string.Empty;
                    lastNameTextBox.Text = string.Empty;
                    firstNameTextBox.Text = string.Empty;
                    telTextBox.Text = string.Empty;
                    emailTextBox.Text = string.Empty;
                    roleTextBox.Text = string.Empty;
                    statusTextBox.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Close the database connection
                connexion.Close();
            }
        }


        private void cancelAddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (utilisateur.Role != "Admin")
            {
                userDashboard dashboard = new userDashboard(utilisateur);
                dashboard.Show();
                ouverture = true;
                this.Close();
            }
            else
            {
                adminDashboard AD = new adminDashboard(utilisateur);
                AD.Show();
                ouverture = true;
                this.Close();
            }
        }
    }
}
