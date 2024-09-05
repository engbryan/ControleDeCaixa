using EntryControl.Core.Services;
using EntryControl.Forms;
using EntryControl.Repositories;
using EntryControl.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Forms;

namespace EntryControl.Client.Flows
{
    public class DefaultApplicationFlow : IApplicationFlow
    {
        private LoginForm _loginForm;
        private MainForm _mainForm;
        private readonly IEntryService _entryService;
        private readonly ISendService _sendService;
        private readonly IEntryRepository _entryRepository;
        private readonly ILogger<Application> _logger;

        public DefaultApplicationFlow(LoginForm loginForm,
                                        MainForm mainForm,
                                        IEntryService entryService,
                                        ISendService sendService,
                                        IEntryRepository entryRepository,
                                        ILogger<Application> logger)
        {
            _loginForm = loginForm;
            _mainForm = mainForm;
            _entryService = entryService;
            _sendService = sendService;
            _entryRepository = entryRepository;
            _logger = logger;

            //_loginForm.LoginCompleted += OnLoginCompleted;
            //_entryService.OnEntryAdded += _ => _sendService.Send(_);
            //_sendService.OnSendCompleted+=_ => _entryRepository.MarkAsSynchronized(_.EntryId);
        }

        public void Start()
        {
            _logger.LogInformation("Iniciando aplicação.");

            Application.EnableVisualStyles();
            Application.Run(_mainForm);
        }

        //private void OnLoginCompleted(object sender, EventArgs e)
        //{
        //    _mainForm.Invoke(new Action(_mainForm.Show));
        //    _loginForm.BeginInvoke(() => _loginForm.Hide());
        //}

        public void End()
        {
            _logger.LogInformation("Finalizando aplicação.");

            Application.Exit();
        }

    }
}
