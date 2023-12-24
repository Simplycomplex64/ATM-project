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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AtmProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Creation des variables necessaires
        bool ouverture = false;
        SqlConnection connexion;
        SqlCommand commande;
        int wrongAttempts = 0;

        public MainWindow()
        {
            InitializeComponent();
            //creation de notre methode de connection
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            //focus on username txtBox
            usernameTxtBox.Focus();
            //set ouverture to true
            ouverture = true;
        }

        //Boutton CLick log in

        private void logInBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    string authentification = "SELECT * FROM tbl_Users WHERE id = '" + usernameTxtBox.Text + "' AND NIP = '" + passwordBox.Password + "'";
                    commande = new SqlCommand(authentification, connexion);
                    connexion.Open();

                    SqlDataReader lecteur = commande.ExecuteReader();

                    if (usernameTxtBox.Text == string.Empty)
                    {
                        MessageBox.Show("Veuillez saisir votre Username ou ID");
                    }
                    else if (passwordBox.Password == string.Empty)
                    {
                        MessageBox.Show("Veuillez saisir votre mot de passe");
                    }
                    else
                    {
                        if (lecteur.Read())
                        {
                            UtilisateurActif utilisateur = new UtilisateurActif();

                            utilisateur.User_id = lecteur["id"].ToString();
                            utilisateur.Prenom = lecteur["Prenom"].ToString();
                            utilisateur.Nom = lecteur["Nom"].ToString();
                            utilisateur.Status = lecteur["Status"].ToString();
                            utilisateur.Role = lecteur["Role"].ToString();

                            if (utilisateur.Status == "True")
                            {
                                if (utilisateur.Role == "Admin")
                                {
                                    MessageBox.Show("Bienvenue " + utilisateur.Nom + " " + utilisateur.Prenom);
                                    adminDashboard AD = new adminDashboard(utilisateur);
                                    AD.Show();
                                    ouverture = false;
                                    this.Close();
                                }
                                else
                                {
                                    // Check the value of ATM_Open column in the tbl_close_atm table
                                    bool isAtmOpen = CheckIfAtmOpen();

                                    if (isAtmOpen)
                                    {
                                        // ATM is open, redirect to userDashboard
                                        MessageBox.Show("Bienvenue " + utilisateur.Nom + " " + utilisateur.Prenom);
                                        userDashboard dashboard = new userDashboard(utilisateur);
                                        dashboard.Show();
                                        ouverture = true;
                                        this.Close();
                                    }
                                    else
                                    {
                                        // ATM is closed, redirect to closeAtm.xaml
                                        closeAtm closeAtmWindow = new closeAtm();
                                        closeAtmWindow.Show();
                                        ouverture = true;
                                        this.Close();
                                    }
                                }

                            }
                            else
                            {
                                MessageBox.Show("Votre compte est bloqué. Veuillez contacter l'administrateur.");
                            }
                        }
                        else
                        {
                            // Increment wrongAttempts and check if it's 3
                            wrongAttempts++;
                            if (wrongAttempts >= 3)
                            {
                                lecteur.Close();
                                // Block the user by updating the status to 0
                                string blockUserQuery = "UPDATE tbl_Users SET Status = '0' WHERE id = '" + usernameTxtBox.Text + "'";
                                SqlCommand blockCommand = new SqlCommand(blockUserQuery, connexion);
                                blockCommand.ExecuteNonQuery();

                                MessageBox.Show("3 tentatives incorrectes. Votre compte est maintenant bloqué.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private bool CheckIfAtmOpen()
        {
            bool isAtmOpen = false;

            try
            {
                // Open the connection to the database
                connexion.Open();

                // Create a command object to retrieve the ATM_Open value
                SqlCommand checkAtmOpenCommand = new SqlCommand("SELECT ATM_Open FROM tbl_close_atm", connexion);
                isAtmOpen = Convert.ToBoolean(checkAtmOpenCommand.ExecuteScalar());
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

            return isAtmOpen;
        }

    }
}
