using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Activities;
using System.Activities.Tracking;


using System.Collections.ObjectModel;

namespace WpfWithWF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields
        WorkflowApplication _workflow;
        
        readonly ObservableCollection<ApplicationData> _applicationData = new ObservableCollection<ApplicationData>();

        readonly WorkflowCommunicationExtension _wce =
            WorkflowCommunicationExtension.GetWorkflowCommunicationExtension();
        
        #endregion

        #region Properties

        public ObservableCollection<ApplicationData> ApplicationData { get { return _applicationData; } }
        
        #endregion

        #region Constructor
        
        public MainWindow()
        {
            InitializeComponent();
            grvApplications.ItemsSource = ApplicationData;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteExecuted));
            txtProjectName.Focus();
            _wce.OnPropertyChanged += wce_OnPropertyChanged;
        }
        
        #endregion

        #region Methods
        
        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (grvApplications.SelectedIndex>=0)
                _applicationData.RemoveAt(grvApplications.SelectedIndex );
        }

        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var projectInfo = new ProjectInformation
            {
                ProjectName = txtProjectName.Text,
                ApplicationInfo = _applicationData.ToList()
            };
            var workflowArgs = 
                new Dictionary<string, object> {{"ProjectInfo", projectInfo}};
           
            _workflow = new WorkflowApplication(new TheWorkflow(),workflowArgs)
            {
                Completed = WorkflowCompletedCallback,
                Idle = WorkflowIdleCallback
            };
           
            _workflow.Extensions.Add(new List<string>());
            
            _workflow.Extensions.Add(_wce);

            var trackingParticipant = new EtwTrackingParticipant();
           
            var trackingProfile = new TrackingProfile
            {
                Name = "SampleTrackingProfile",
                ActivityDefinitionId = "FIB-->FIS"
            };
           
            trackingProfile.Queries.Add(new WorkflowInstanceQuery {States = {"*"}});
            
            _workflow.Extensions.Add(trackingParticipant);

            txbOutput.Text += "Starting planning phase...\r\n";
           
            _workflow.Run();
        }

        void wce_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var newValue = e.NewValue.ToString();
            if (newValue != string.Empty)
            {
                txbOutput.Dispatcher.Invoke(() =>
            txbOutput.Text += newValue + "\r\n");
            }
        }

        private void WorkflowIdleCallback(WorkflowApplicationIdleEventArgs eArgs)
        {
            
            //idleEvent.Set();
            var output = eArgs.GetInstanceExtensions<List<string>>();
            var enumerable = output as List<string>[] ?? output.ToArray();
            if (enumerable.First() != null)
            {
                txbOutput.Dispatcher.Invoke (() =>
                {
                    foreach (var item in enumerable.First())
                    {
                        txbOutput.Text += item + "\r\n";
                    }
                });
            }
        }


        
        private void WorkflowCompletedCallback(WorkflowApplicationCompletedEventArgs cArgs)
        {
            var output = cArgs.GetInstanceExtensions<List<string>>();
            var enumerable = output as List<string>[] ?? output.ToArray();
            if (enumerable.First() != null)
            {
                txbOutput.Dispatcher.Invoke(() =>
                {
                    foreach (string item in enumerable.First())
                    {
                        txbOutput.Text += item + "\r\n";
                    }
                });
            }
        }


        private void btnAddApplication_Click(object sender, RoutedEventArgs e)
        {
            if (txtApplicationName.Text != string.Empty)
            {
                _applicationData.Add(new ApplicationData
                {
                    ApplicationName = txtApplicationName.Text,
                    NewApplication = chkNewApp.IsChecked == true ? "Yes" : "No"
                });
            }
        }


        private void btnExectution_Click(object sender, RoutedEventArgs e)
        {
             if (_workflow.ResumeBookmark("Execution", null) == BookmarkResumptionResult.Success)
                 txbOutput.Text += "Starting execution phase...\r\n";
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txbOutput.Text = string.Empty;
        }

        private void btnPostCutover_Click(object sender, RoutedEventArgs e)
        {
            
            if (_workflow.ResumeBookmark("PostCutOver", value: null)== BookmarkResumptionResult.Success)
                txbOutput.Text += "Starting Post Cut Over phase...\r\n";
        }


        #endregion

    }
}
