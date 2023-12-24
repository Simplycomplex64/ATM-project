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
    /// Interaction logic for transactionHistory.xaml
    /// </summary>
    public partial class transactionHistory : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public transactionHistory(UtilisateurActif actif)
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            utilisateur = actif;
            ouverture = true;
        }

        private void transactionSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the connection to the database
                connexion.Open();

                // Create a command object for executing the stored procedure
                SqlCommand searchTransactionsCommand = new SqlCommand("SearchTransactionsByUser", connexion);
                searchTransactionsCommand.CommandType = CommandType.StoredProcedure;

                // Add the parameter for the stored procedure
                searchTransactionsCommand.Parameters.AddWithValue("@User_id", Convert.ToInt32(searchIdTextBox.Text));

                // Create a data adapter and dataset to retrieve the results
                SqlDataAdapter adapter = new SqlDataAdapter(searchTransactionsCommand);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                // Bind the dataset to the DataGrid
                dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
            }
            catch (SqlException ex)
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

        private void backBtn_Click(object sender, RoutedEventArgs e)
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
