
using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Domain.Entities;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntryControl.POS.App
{
    public partial class MainForm : MaterialForm
    {
        public enum EntryFilter
        {
            All,
            Synchronized,
            Dirty
        }

        private readonly LoginForm _loginForm;
        private readonly IAuthService _authService;
        private readonly IEntryService _entryService;
        private readonly ISynchronizeService _synchronizeService;
        private readonly ISendService _sendService;
        private readonly IHealthCheckService _healthCheckService;
        private MaterialTabSelector tabSelector;
        private EntryFilter _currentFilter = EntryFilter.All;

        public MainForm(
                        LoginForm loginForm,
                        IAuthService authService,
                        IEntryService entryService,
                        ISynchronizeService synchronizeService,
                        ISendService sendService,
                        IHealthCheckService healthCheckService)
        {
            _loginForm = loginForm;
            _authService = authService;
            _entryService = entryService ?? throw new ArgumentNullException(nameof(entryService));
            _synchronizeService = synchronizeService ?? throw new ArgumentNullException(nameof(synchronizeService));
            _sendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));

            InitializeComponent();

            SubscribeToEvents();

            RefreshEntryList().Wait();
        }

        private void SubscribeToEvents()
        {
            this.Shown += MainForm_Shown;
            this.FormClosing += MainForm_FormClosing;
            _loginForm.LoginCompleted += LoginForm_LoginCompleted;
            _loginForm.FormClosing += LoginForm_FormClosing;
            _healthCheckService.OnHealthStatusChanged += HandleHealthStatusChanged;
            _synchronizeService.OnSyncCompleted += HandleSyncCompleted;
            _sendService.OnSendCompleted += HandleSendCompleted;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LoginForm_FormClosing(sender, e)));
                return;
            }

            if (_authService.IsAuthenticating)
            {
                e.Cancel = true;
                return;
            }
            else if (!_authService.IsAuthenticated)
            {
                Environment.Exit(0);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MainForm_Shown(sender, e)));
                return;
            }

            this.Enabled = false;
            _loginForm.Show(this);
        }

        private void LoginForm_LoginCompleted(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LoginForm_LoginCompleted(sender, e)));
                return;
            }

            _loginForm.Hide();
            this.Activate();
            this.Enabled = true;
            this.Text = $"{Text} - {_authService.GetUserName()}";
        }

        private async void HandleSendCompleted(POSEntry entry)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleSendCompleted(entry)));
                return;
            }

            await RefreshEntryList();
        }

        private void HandleHealthStatusChanged(bool isHealthy)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleHealthStatusChanged(isHealthy)));
                return;
            }

            connectionStatusLabel.BackColor = GetConnectionColor(isHealthy);
            ShowToast(isHealthy ? "Connection to the online service established." : "Connection to the online service lost.");
        }

        private void HandleSyncCompleted(IEnumerable<POSEntry> entries)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleSyncCompleted(entries)));
                return;
            }

            DisplaySyncResult(entries);
        }

        private async Task RefreshEntryList()
        {
            listViewEntries.Items.Clear();
            var entries = await GetFilteredEntries();

            foreach (var entry in entries)
            {
                AddEntryToListView(entry);
            }
        }

        private async Task<List<POSEntry>> GetFilteredEntries()
        {
            return _currentFilter switch
            {
                EntryFilter.Synchronized => await _entryService.GetAllSinchronized(),
                EntryFilter.Dirty => await _entryService.GetAllDirty(),
                _ => await _entryService.GetAll()
            };
        }

        private void AddEntryToListView(POSEntry entry)
        {
            var item = new ListViewItem(entry.ClientId.ToString());
            item.SubItems.Add(entry.Type);
            item.SubItems.Add(entry.Amount.ToString("C"));
            item.SubItems.Add(entry.Description);
            item.SubItems.Add(entry.EntryDate.ToString("g"));
            item.SubItems.Add(entry.Synchronized ? "Yes" : "No");

            listViewEntries.Items.Add(item);
        }

        private Color GetConnectionColor(bool isConnected)
        {
            return isConnected ? Color.Green : Color.Red;
        }

        private void ShowToast(string message)
        {
            var snackBar = new MaterialSnackBar(message, 5000);
            snackBar.Show(this);
        }

        private void DisplaySyncResult(IEnumerable<POSEntry> entries)
        {
            var message = entries.Any(entry => entry.Synchronized)
                ? "Some entries were synchronized, others were not."
                : "Error during synchronization.";

            message = entries.All(entry => entry.Synchronized)
                ? "Synchronization successful."
                : message;

            ShowToast(message);
        }

        private async void AddEntry_Click(object sender, EventArgs e)
        {
            try
            {
                var type = txtType.SelectedItem?.ToString();
                var amount = decimal.Parse(txtAmmount.Text);
                var description = txtDescription.Text;

                await _entryService.Add(type, amount, description);
                ShowToast("Entry added successfully!");

                ResetEntryForm();
                await RefreshEntryList();
            }
            catch (Exception ex)
            {
                ShowToast("Error adding entry: " + ex.Message);
            }
        }

        private async void FilterAll_Click(object sender, EventArgs e)
        {
            _currentFilter = EntryFilter.All;
            await RefreshEntryList();
        }

        private async void FilterSynchronized_Click(object sender, EventArgs e)
        {
            _currentFilter = EntryFilter.Synchronized;
            await RefreshEntryList();
        }

        private async void FilterDirty_Click(object sender, EventArgs e)
        {
            _currentFilter = EntryFilter.Dirty;
            await RefreshEntryList();
        }

        private void ResetEntryForm()
        {
            txtType.SelectedIndex = -1;
            txtAmmount.Clear();
            txtDescription.Clear();
        }
    }
}
