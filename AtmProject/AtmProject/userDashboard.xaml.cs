using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
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
    /// Interaction logic for userDashboard.xaml
    /// </summary>
    public partial class userDashboard : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public userDashboard( UtilisateurActif actif)
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            utilisateur =actif;  
            ouverture = true;
        }

        private void transferBtn_Click(object sender, RoutedEventArgs e)
        {
            connexion.Open();
            depositWindow dW = new depositWindow(utilisateur);
            dW.Show();
            this.Close();
        }

        private void withdrawBtn_Click(object sender, RoutedEventArgs e)
        {
            connexion.Open();
            withdrawPage WP = new withdrawPage(utilisateur);
            WP.Show();
            this.Close();
        }

        private void depositBtn_Click(object sender, RoutedEventArgs e)
        {
            connexion.Open();
            depositPage DP = new depositPage(utilisateur);
            DP.Show();
            this.Close();
        }

        private void allTransactionsBtn_Click(object sender, RoutedEventArgs e)
        {
            connexion.Open();
            alltransactions AT = new alltransactions(utilisateur);
            AT.Show();
            this.Close();
        }


        private void balancesBtn_Click(object sender, RoutedEventArgs e)
        {
            connexion.Open();
            accountBalance AB = new accountBalance(utilisateur);
            AB.Show();
            this.Close();
        }

        private void payABillBtn_Click(object sender, RoutedEventArgs e)
        {
            connexion.Open();
            paymentPage PP = new paymentPage(utilisateur);
            PP.Show();
            this.Close();
        }


        private void accountNumberLoaded(object sender, RoutedEventArgs e)
        {

            if (utilisateur != null)
            {
                // Retrieve the user ID from the utilisateur object
                string userId = "User ID : " + utilisateur.User_id;

                // Set the label's content to the retrieved user ID
                userLbl.Content = userId;
            }
        }

        private void DateLoaded(object sender, RoutedEventArgs e)
        {
            // Set the label's content to the current date
            dateLbl.Content = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        }

        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).RestartApplication();
        }

        private void firstnameLastnameLoaded(object sender, RoutedEventArgs e)
        {
            if (utilisateur != null)
            {
                string userLastName = utilisateur.Nom;
                string userFirstName = utilisateur.Prenom;

                firstAndLastNameLbl.Content = " " + userLastName + " " + userFirstName;
            }
        }

        private void overdraftLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the connection to the database
                connexion.Open();

                // Create the command object to call the stored procedure
                commande = new SqlCommand("overdraftAtAGlance", connexion);
                commande.CommandType = CommandType.StoredProcedure;

                // Add the parameter for the stored procedure
                commande.Parameters.AddWithValue("@User_id", utilisateur.User_id);
                commande.Parameters.AddWithValue("@account_type", "Checking");

                // Execute the command and read the data
                SqlDataReader reader = commande.ExecuteReader();

                // Check if the reader has rows and read the overdraft value
                if (reader.Read())
                {
                    decimal overdraftAmount = Convert.ToDecimal(reader["overdraft"]);


                    // Display the overdraft amount in the label
                    overdraftAmountLbl_txt.Content = overdraftAmount.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                // Close the connection to the database
                if (connexion.State == ConnectionState.Open)
                    connexion.Close();
            }
        }
        private void payOverdraftBtn_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                // Open the connection to the database
                connexion.Open();

                // Create a command object to retrieve the user's account balance and overdraft
                SqlCommand balanceCommand = new SqlCommand("SELECT account_balance, overdraft, account_id FROM tbl_Accounts WHERE User_id = @userId AND account_type = 'Checking'", connexion);
                balanceCommand.Parameters.AddWithValue("@userId", utilisateur.User_id);

                // Execute the balance command and read the data
                SqlDataReader balanceReader = balanceCommand.ExecuteReader();

                if (balanceReader.Read())
                {
                    long accountBalance = Convert.ToInt64(balanceReader["account_balance"]);
                    long overdraft = Convert.ToInt64(balanceReader["overdraft"]);
                    long accountId = (long)balanceReader["account_id"];

                    // Close the balance reader
                    balanceReader.Close();

                    if (overdraft == 0)
                    {
                        MessageBox.Show("There is no overdraft to pay.");
                    }
                    // Check if account balance is higher than or equal to overdraft
                    else if (accountBalance >= -(overdraft))
                    {
                        // Create the command object to call the stored procedure
                        SqlCommand payOverdraftCommand = new SqlCommand("ClearOverdraftFromBalance", connexion);
                        payOverdraftCommand.CommandType = CommandType.StoredProcedure;

                        // Add the parameter for the stored procedure
                        payOverdraftCommand.Parameters.AddWithValue("@userId", utilisateur.User_id);

                        // Execute the command
                        payOverdraftCommand.ExecuteNonQuery();

                        // Display a success message
                        MessageBox.Show("Overdraft has been paid successfully.");

                        // Update the overdraft amount label (assuming overdraftAmountLbl_txt is your label)
                        overdraftAmountLbl_txt.Content = "$0.00"; // Reset overdraft amount
                    }
                    else
                    {
                        MessageBox.Show("Not enough funds to pay the overdraft.");
                    }
                }
                else
                {
                    MessageBox.Show("Account balance and overdraft data not found.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000) // Custom error number for RAISERROR in the procedure
                {
                    MessageBox.Show(ex.Message);
                }
                else
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            finally
            {
                // Close the connection to the database
                if (connexion.State == ConnectionState.Open)
                    connexion.Close();
            }
        }
    }
}
