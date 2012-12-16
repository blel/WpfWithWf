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

        public ObservableCollection<ApplicationData> ApplicationData { get { return _applicationData; } }
        public MainWindow()
        {
            
            InitializeComponent();
            this.grvApplications.ItemsSource = ApplicationData;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteExecuted));
            txtProjectName.Focus();

            

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
            System.Collections.Generic.Dictionary<string, object> workflowArgs = new System.Collections.Generic.Dictionary<string, object>();
            workflowArgs.Add("ProjectInfo", projectInfo);

            
            workflow = new WorkflowApplication(new TheWorkflow(),workflowArgs);
            workflow.Completed = WorkflowCompletedCallback;
            workflow.Idle = delegate(WorkflowApplicationIdleEventArgs eArgs) //anonymous method
            {
                idleEvent.Set();
            };

            workflow.Run();
            

        }

        private void WorkflowCompletedCallback(WorkflowApplicationCompletedEventArgs cArgs)
        {
            List<string> output = new List<string>();
            output = (List<string>)cArgs.Outputs["Output"];
            foreach (string item in output)
            {
                txbOutput.Text =item ;
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

        private void lstNewApplications_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void mniDelete_Click(object sender, RoutedEventArgs e)
        {
            //var q = lstNewApplications.SelectedItem;
        }

        private void btnExectution_Click(object sender, RoutedEventArgs e)
        {
            EventArgs args = new EventArgs();
            workflow.ResumeBookmark("Execution", "anything");
        }



  

    }
}
