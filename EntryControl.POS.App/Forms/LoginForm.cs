using EntryControl.POS.Core.Interfaces.Services;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace EntryControl.POS.App
{
    public partial class LoginForm : MaterialForm
    {
        private readonly IAuthService _authService;

        public event EventHandler LoginCompleted;

        public LoginForm(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var text = btnLogin.Text;
            btnLogin.Text = "PROCESSANDO...";
            try
            {
                var username = txtUsername.Text.Trim();
                var password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    ShowError("Username and password cannot be empty.");
                    return;
                }

                var token = await _authService.LoginAsync(username, password);
                //MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Dispara o evento de login concluído
                OnLoginCompleted(EventArgs.Empty);
            }
            catch (UnauthorizedAccessException)
            {
                ShowError("Invalid username or password.");
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
            Enabled = true;
            btnLogin.Text = text;
        }

        protected virtual void OnLoginCompleted(EventArgs e)
        {
            LoginCompleted?.Invoke(this, e);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
