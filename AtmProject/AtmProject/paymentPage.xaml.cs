using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
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
    /// Interaction logic for paymentPage.xaml
    /// </summary>
    public partial class paymentPage : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public paymentPage(UtilisateurActif actif)
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            utilisateur = actif;
            ouverture = true;
        }

        private void paymentBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(paymentAmount.Text) && !string.IsNullOrEmpty(billTxtBox.Text) && payFromAccountComboBox.SelectedItem != null)
                {
                    string[] selectedAccountInfo = payFromAccountComboBox.SelectedItem.ToString().Split('-');
                    if (selectedAccountInfo.Length >= 2)
                    {
                        int accountFrom = int.Parse(selectedAccountInfo[0]);
                        int accountTo = 0; // Replace with the appropriate account ID for the recipient

                        decimal amount = decimal.Parse(paymentAmount.Text);
                        int billId = int.Parse(billTxtBox.Text);

                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction(); // Start a transaction

                            try
                            {
                                using (SqlCommand command = new SqlCommand("MakePayment", connection, transaction))
                                {
                                    command.CommandType = CommandType.StoredProcedure;

                                    command.Parameters.AddWithValue("@Bill_id", billId);
                                    command.Parameters.AddWithValue("@Amount", amount);
                                    command.Parameters.AddWithValue("@account_id", accountFrom);
                                    command.Parameters.AddWithValue("@user_id", utilisateur.User_id);
                                    command.Parameters.AddWithValue("@accountFrom", accountFrom);
                                    command.Parameters.AddWithValue("@accountTo", accountTo);

                                    SqlParameter resultParameter = new SqlParameter("@Result", SqlDbType.Int);
                                    resultParameter.Direction = ParameterDirection.Output; // Set the parameter direction to output
                                    command.Parameters.Add(resultParameter);

                                    command.ExecuteNonQuery();

                                    int result = (int)resultParameter.Value;

                                    if (result == 0)
                                    {
                                        transaction.Commit(); // Commit the transaction
                                        MessageBox.Show("Payment successfully made!");
                                        // Perform any additional actions you need here, like updating UI, refreshing data, etc.
                                        payFromAccountComboBox.Items.Clear();
                                        billTxtBox.Text = string.Empty;
                                        paymentAmount.Text = string.Empty;
                                    }
                                    else
                                    {
                                        transaction.Rollback(); // Rollback the transaction
                                        MessageBox.Show("Payment failed. Insufficient funds.");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback(); // Rollback the transaction on exception
                                MessageBox.Show("there is not enough funds to make that payment. Please use an overdraft enabled account if you have one.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid selected account.");
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred");
            }
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

                                payFromAccountComboBox.Items.Add(accountId + "-" + accountType + " - Account balance is : " + roundedBalance);
                            }
                        }
                    }
                }
            }
        }

        private void cancelPayment_Click(object sender, RoutedEventArgs e)
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
