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
    /// Interaction logic for withdrawPage.xaml
    /// </summary>
    public partial class withdrawPage : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public withdrawPage(UtilisateurActif actif)
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

                                WithdrawFromComboBox.Items.Add(accountId + "-" + accountType + " - Account balance is : " + roundedBalance);
                            }
                        }
                    }
                }
            }
        }


        private void withdrawProceed_Click(object sender, RoutedEventArgs e)
        {
            string selectedAccount = WithdrawFromComboBox.SelectedItem?.ToString();

            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account to withdraw from.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (utilisateur != null)
            {
                string[] accountParts = selectedAccount.Split('-');

                if (accountParts.Length >= 2)
                {
                    long fromAccountID = long.Parse(accountParts[0].Trim());

                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();

                        if (!decimal.TryParse(withdrawAmountTextBox.Text, out decimal wA) || wA <= 0)
                        {
                            MessageBox.Show("Invalid withdrawal amount entered.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Check if withdrawal amount is a multiple of 10
                        if (wA % 10 != 0)
                        {
                            MessageBox.Show("Withdrawal amount must be a multiple of 10.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Check if withdrawal amount exceeds 1000
                        if (wA > 1000)
                        {
                            MessageBox.Show("Withdrawal amount cannot exceed 1000.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        try
                        {
                            using (SqlCommand command = new SqlCommand("WithdrawProcedure", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@account_id", fromAccountID);
                                command.Parameters.AddWithValue("@amount", wA);
                                command.Parameters.AddWithValue("@user_id", utilisateur.User_id);

                                // Execute the stored procedure
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            MessageBox.Show("Withdrawal successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Open a new withdrawPage and close the current window
                            withdrawPage WP = new withdrawPage(utilisateur);
                            WP.Show();
                            this.Close();
                        }
                        catch (SqlException ex)
                        {
                            transaction.Rollback();

                            if (ex.Message.Contains("Insufficient funds.") || ex.Message.Contains("Insufficient funds in the ATM."))
                            {
                                MessageBox.Show("Insufficient funds in the ATM.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle other exceptions
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid account format.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid user.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void withdrawCancelBtn_Click(object sender, RoutedEventArgs e)
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
