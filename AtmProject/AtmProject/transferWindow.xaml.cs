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
    /// Interaction logic for depositWindow.xaml
    /// </summary>
    public partial class depositWindow : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public depositWindow(UtilisateurActif actif)
        {
            InitializeComponent();
            utilisateur = actif;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
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


        private void depositFromComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (utilisateur != null)
            {
                string query = "SELECT account_id, account_balance, account_type FROM tbl_Accounts WHERE User_id = @UserID";

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand commande = new SqlCommand(query, connection))
                    {
                        commande.Parameters.AddWithValue("@UserID", utilisateur.User_id);

                        using (SqlDataReader lecteur = commande.ExecuteReader())
                        {
                            while (lecteur.Read())
                            {
                                long accountId = lecteur.GetInt64(0);
                                double accountBalance = lecteur.GetDouble(1);
                                string accountType = lecteur.GetString(2);

                                // Round the account balance to 2 decimal places
                                double roundedBalance = Math.Round(accountBalance, 2);

                                DepositFromComboBox.Items.Add(accountId + "-" + accountType + " - Account balance is : " + roundedBalance);
                            }
                        }
                    }
                }
            }
        }

        private void depositToComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (utilisateur != null)
            {
                string query = "SELECT account_id, account_balance, account_type FROM tbl_Accounts WHERE User_id = @UserID";

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand commande = new SqlCommand(query, connection))
                    {
                        commande.Parameters.AddWithValue("@UserID", utilisateur.User_id);

                        using (SqlDataReader lecteur = commande.ExecuteReader())
                        {
                            while (lecteur.Read())
                            {
                                long accountId = lecteur.GetInt64(0);
                                double accountBalance = lecteur.GetDouble(1);
                                string accountType = lecteur.GetString(2);

                                // Round the account balance to 2 decimal places
                                double roundedBalance = Math.Round(accountBalance, 2);

                                DepositToComboBox.Items.Add(accountId + "-" + accountType + " - Account balance is : " + roundedBalance);
                            }
                        }
                    }
                }
            }
        }

        private void submitTransferBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DepositFromComboBox.SelectedItem == null || DepositToComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select source and target accounts.");
                return;
            }

            string fromAccountInfo = DepositFromComboBox.SelectedItem.ToString();
            string toAccountInfo = DepositToComboBox.SelectedItem.ToString();

            int fromAccountId = int.Parse(fromAccountInfo.Substring(0, fromAccountInfo.IndexOf('-')));
            int toAccountId = int.Parse(toAccountInfo.Substring(0, toAccountInfo.IndexOf('-')));

            decimal transferAmount;

            if (!decimal.TryParse(amountTextBox.Text, out transferAmount))
            {
                MessageBox.Show("Please enter a valid transfer amount.");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand command = new SqlCommand("TransferProcedure", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Add parameters including user_id
                            command.Parameters.AddWithValue("@from_account_id", fromAccountId);
                            command.Parameters.AddWithValue("@to_account_id", toAccountId);
                            command.Parameters.AddWithValue("@amount", transferAmount);
                            command.Parameters.AddWithValue("@user_id", utilisateur.User_id);

                            // Execute the stored procedure
                            command.ExecuteNonQuery();

                            // Commit the transaction
                            transaction.Commit();

                            MessageBox.Show("Transfer successful.");

                            // Optionally, update the combo boxes to refresh account information
                            DepositFromComboBox.Items.Clear();
                            DepositToComboBox.Items.Clear();
                            depositFromComboBoxLoaded(sender, e);
                            depositToComboBoxLoaded(sender, e);

                            // Clear the amount input field
                            amountTextBox.Clear();
                        }
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();

                        if (ex.Message.Contains("Insufficient funds for transfer.") || ex.Message.Contains("Insufficient funds in the ATM."))
                        {
                            MessageBox.Show("Insufficient funds.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions
                        transaction.Rollback();
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
