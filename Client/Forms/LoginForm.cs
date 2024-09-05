using EntryControl.Core.Services;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace EntryControl.Client
{
    public partial class LoginForm : MaterialForm
    {
        private readonly IAuthService _authService;

        public event EventHandler LoginCompleted;

        public LoginForm(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            InitializeComponent();
            InitializeMaterialSkin();
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

        private void InitializeMaterialSkin()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.LightBlue200,
                TextShade.WHITE);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InitializeComponent()
        {
            this.txtUsername = new MaterialSkin.Controls.MaterialTextBox();
            this.txtPassword = new MaterialSkin.Controls.MaterialTextBox();
            this.btnLogin = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Hint = "Username";
            this.txtUsername.Location = new System.Drawing.Point(12, 73);
            this.txtUsername.MaxLength = 32767;
            this.txtUsername.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Password = false;
            this.txtUsername.SelectedText = "";
            this.txtUsername.SelectionLength = 0;
            this.txtUsername.SelectionStart = 0;
            this.txtUsername.Size = new System.Drawing.Size(260, 48);
            this.txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Hint = "Password";
            this.txtPassword.Location = new System.Drawing.Point(12, 127);
            this.txtPassword.MaxLength = 32767;
            this.txtPassword.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Password = true;
            this.txtPassword.SelectedText = "";
            this.txtPassword.SelectionLength = 0;
            this.txtPassword.SelectionStart = 0;
            this.txtPassword.Size = new System.Drawing.Size(260, 48);
            this.txtPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            this.btnLogin.AutoSize = false;
            this.btnLogin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLogin.Depth = 0;
            this.btnLogin.DrawShadows = true;
            this.btnLogin.HighEmphasis = true;
            this.btnLogin.Icon = null;
            this.btnLogin.Location = new System.Drawing.Point(12, 191);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnLogin.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(260, 36);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Login";
            this.btnLogin.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLogin.UseAccentColor = false;
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 250);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Name = "LoginForm";
            this.Text = "Login";
            this.ResumeLayout(false);
        }

        private MaterialSkin.Controls.MaterialTextBox txtUsername;
        private MaterialSkin.Controls.MaterialTextBox txtPassword;
        private MaterialSkin.Controls.MaterialButton btnLogin;
    }
}
