using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Activities;
using System.Activities.Statements;
using System.Activities.Tracking;


using System.Collections.ObjectModel;

namespace WpfWithWF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AutoResetEvent idleEvent = new AutoResetEvent(false);
        WorkflowApplication workflow;
        ObservableCollection<ApplicationData> _applicationData = new ObservableCollection<ApplicationData>();
        WorkflowCommunicationExtension wce =
            WorkflowCommunicationExtension.GetWorkflowCommunicationExtension();

        public ObservableCollection<ApplicationData> ApplicationData { get { return _applicationData; } }
        public MainWindow()
        {
            InitializeComponent();
            this.grvApplications.ItemsSource = ApplicationData;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteExecuted));
            txtProjectName.Focus();
            wce.OnPropertyChanged += wce_OnPropertyChanged;
        }

        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (grvApplications.SelectedIndex>=0)
                _applicationData.RemoveAt(grvApplications.SelectedIndex );
        }

        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ProjectInformation projectInfo = new ProjectInformation();
            projectInfo.ProjectName = this.txtProjectName.Text;
            projectInfo.ApplicationInfo = _applicationData.ToList<ApplicationData>();
            System.Collections.Generic.Dictionary<string, object> workflowArgs = 
                new System.Collections.Generic.Dictionary<string, object>();
            workflowArgs.Add("ProjectInfo", projectInfo);
            workflow = new WorkflowApplication(new TheWorkflow(),workflowArgs);
            workflow.Completed = WorkflowCompletedCallback;
            workflow.Idle = WorkflowIdleCallback;
            workflow.Extensions.Add(new List<string>());
            workflow.Extensions.Add(wce);
            this.txbOutput.Text += "Starting planning phase...\r\n";
            workflow.Run();
        }

        void wce_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string newValue = e.NewValue.ToString();
            if (newValue != string.Empty)
            {
                this.txbOutput.Dispatcher.Invoke(() =>
            this.txbOutput.Text += newValue + "\r\n");
            }
        }

        private void WorkflowIdleCallback(WorkflowApplicationIdleEventArgs eArgs)
        {
            
            //idleEvent.Set();
            var output = eArgs.GetInstanceExtensions<List<string>>();
            if (output.First() != null)
            {
                this.txbOutput.Dispatcher.Invoke (() =>
                {
                    foreach (string item in output.First())
                    {
                        this.txbOutput.Text += item + "\r\n";
                    }
                });
            }
        }


        
        private void WorkflowCompletedCallback(WorkflowApplicationCompletedEventArgs cArgs)
        {
            var output = cArgs.GetInstanceExtensions<List<string>>();
            if (output.First() != null)
            {
                this.txbOutput.Dispatcher.Invoke(() =>
                {
                    foreach (string item in output.First())
                    {
                        this.txbOutput.Text += item + "\r\n";
                    }
                });
            }
        }


        private void btnAddApplication_Click(object sender, RoutedEventArgs e)
        {
            if (txtApplicationName.Text != string.Empty)
            {
                _applicationData.Add(new ApplicationData()
                {
                    ApplicationName = txtApplicationName.Text,
                    NewApplication = chkNewApp.IsChecked == true ? "Yes" : "No"
                });
            }
        }


        private void btnExectution_Click(object sender, RoutedEventArgs e)
        {
             if (workflow.ResumeBookmark("Execution", null) == BookmarkResumptionResult.Success)
                 this.txbOutput.Text += "Starting execution phase...\r\n";
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txbOutput.Text = string.Empty;
        }

        private void btnPostCutover_Click(object sender, RoutedEventArgs e)
        {
            
            if (workflow.ResumeBookmark("PostCutOver", null)== BookmarkResumptionResult.Success)
                this.txbOutput.Text += "Starting Post Cut Over phase...\r\n";
        }
    }
}
