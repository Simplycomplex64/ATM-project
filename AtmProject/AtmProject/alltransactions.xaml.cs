﻿using System;
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
    /// Interaction logic for alltransactions.xaml
    /// </summary>
    public partial class alltransactions : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        SqlCommand commande;

        public alltransactions(UtilisateurActif actif)
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            utilisateur = actif;
            ouverture = true;
        }

        private void goBackBtn_CLick(object sender, RoutedEventArgs e)
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

        private void allTransactionsDataGridLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the connection to the database
                connexion.Open();

                // Create the command object to call the stored procedure
                commande = new SqlCommand("allTransactions", connexion);
                commande.CommandType = CommandType.StoredProcedure;

                // Add the parameter for the stored procedure
                commande.Parameters.AddWithValue("@user_id", utilisateur.User_id);

                // Create a DataTable to store the retrieved data
                DataTable dataTable = new DataTable();
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(commande))
                {
                    // Fill the DataTable with the data from the stored procedure
                    dataAdapter.Fill(dataTable);
                }

                // Bind the DataTable to the DataGrid
                allTransactionDataGrid.ItemsSource = dataTable.DefaultView;
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

    }
}
