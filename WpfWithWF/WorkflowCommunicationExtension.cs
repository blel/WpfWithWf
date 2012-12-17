using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WpfWithWF
{
    public class PropertyChangedEventArgs: EventArgs
    {
        public object OldValue {get; set;}
        public object NewValue {get; set;}
    }

    
    public delegate void PropertyChanged(object sender, PropertyChangedEventArgs e);
    
    
    public class WorkflowCommunicationExtension
    {
        
        
        private static WorkflowCommunicationExtension _singletonInstance;
        private object _communicationContent;

        private WorkflowCommunicationExtension() { }
        
        public event PropertyChanged OnPropertyChanged;
        public object CommunicationContent
        {
            get { return _communicationContent; }
            set
            {
                if (value != _communicationContent)
                {
                    
                    OnPropertyChanged(this, new PropertyChangedEventArgs()
                    {
                        OldValue = _communicationContent,
                        NewValue = value
                    });
                    _communicationContent = value;
                }
            }
        }

        public static WorkflowCommunicationExtension GetWorkflowCommunicationExtension()
        {
            if (_singletonInstance == null)
            {
                _singletonInstance = new WorkflowCommunicationExtension();
            }
            return _singletonInstance;
        }



    }
}
