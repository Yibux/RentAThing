using Firebase.Auth;
using Firebase.Auth.Providers;
using Grpc.Core;
using Grpc.Net.Client;
using RentalSystem.Shared.AppConstants;
using RentalSystem.Shared.Protos;
using System;
using System.Net.Http;
using System.Windows;

namespace RentalSystem.Client.Desktop
{
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            btnLogin.Content = "Logowanie...";

            try
            {
                string email = txtEmail.Text;
                string password = txtPassword.Password;

                var config = new FirebaseAuthConfig
                {
                    ApiKey = AppConstants.FIREBASE_API_KEY,
                    AuthDomain = AppConstants.FIREBASE_AUTH_DOMAIN,
                    Providers = new FirebaseAuthProvider[]
                    {
                        new EmailProvider()
                    }
                };

                var authClient = new FirebaseAuthClient(config);

                var userCredential = await authClient.SignInWithEmailAndPasswordAsync(email, password);

                string token = await userCredential.User.GetIdTokenAsync();
                using var channel = GrpcChannel.ForAddress(AppConstants.BACKEND_GRPC_URL, new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Insecure
                });

                var client = new AuthService.AuthServiceClient(channel);

                var headers = new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };

                var userProfile = await client.GetMyProfileAsync(new EmptyRequest(), headers);

                if (userProfile.Role == "ADMIN")
                {
                    MainWindow mainWindow = new MainWindow(token);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("To konto nie posiada uprawnieñ administratora.", "Brak dostêpu", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (FirebaseAuthException ex)
            {
                MessageBox.Show($"B³¹d logowania Firebase: {ex.Reason}", "B³¹d", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Backend odrzuci³ po³¹czenie: {ex.Status.Detail} ({ex.StatusCode})", "B³¹d Backend", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wyst¹pi³ nieoczekiwany b³¹d: {ex.Message}", "B³¹d", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
                btnLogin.Content = "ZALOGUJ";
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}