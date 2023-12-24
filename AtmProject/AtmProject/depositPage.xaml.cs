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
    /// Interaction logic for depositPage.xaml
    /// </summary>
    public partial class depositPage : Window
    {

        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public depositPage(UtilisateurActif actif)
        {
            InitializeComponent();
            utilisateur = actif;
        }

        private void accounts_Loaded(object sender, RoutedEventArgs e)
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

                                depositIntoComboBox.Items.Add(accountId + "-" + accountType + " - Account balance is : " + roundedBalance);
                            }
                        }
                    }
                }
            }
        }


        private void depositProceed_Click(object sender, RoutedEventArgs e)
        {
            string selectedAccount = depositIntoComboBox.SelectedItem?.ToString();

            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account to deposit into.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (utilisateur != null)
            {
                string[] accountParts = selectedAccount.Split('-');

                if (accountParts.Length >= 2)
                {
                    int accountID = int.Parse(accountParts[0].Trim());
                    int userID = int.Parse(utilisateur.User_id);

                    // Retrieve the deposit amount from the TextBox
                    if (!float.TryParse(depositAmountTextBox.Text, out float depositAmount) || depositAmount <= 0)
                    {
                        MessageBox.Show("Invalid deposit amount entered. Amount must be a positive number.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand("DepositFunds", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@user_id", userID);
                            command.Parameters.AddWithValue("@accountId", accountID);
                            command.Parameters.AddWithValue("@amount", depositAmount);

                            try
                            {
                                var result = command.ExecuteScalar();

                                if (result != null && result.ToString() == "Deposit successful")
                                {
                                    MessageBox.Show("Deposit successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Refresh the deposit window
                                    depositPage DP = new depositPage(utilisateur);
                                    DP.Show();
                                    this.Close();
                                }
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error occurred during the deposit: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("An unexpected error occurred during the deposit: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid account format.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void depositCancelBtn_Click(object sender, RoutedEventArgs e)
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