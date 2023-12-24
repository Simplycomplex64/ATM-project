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
    /// Interaction logic for createAccount.xaml
    /// </summary>
    public partial class createAccount : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public createAccount(UtilisateurActif actif)
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            utilisateur = actif;
            ouverture = true;
        }

        private void createAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the database connection
                connexion.Open();

                // Get the values from the text boxes
                string accountType = accountTypeTextBox.Text;
                int userID = Convert.ToInt32(userIdTextBox.Text);

                // Create the SQL command for calling the stored procedure
                using (SqlCommand command = new SqlCommand("CreateAccountForUser", connexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    command.Parameters.AddWithValue("@AccountType", accountType);
                    command.Parameters.AddWithValue("@UserID", userID);

                    // Execute the stored procedure
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int accountID = Convert.ToInt32(result);
                        MessageBox.Show($"Account created successfully. Account ID: {accountID}");

                        accountTypeTextBox.Text = string.Empty; accountTypeTextBox.Focus();
                        userIdTextBox.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Account creation failed.");
                    }
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


        private void cancelCreateAccountBtn_Click(object sender, RoutedEventArgs e)
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
