using System.Activities;
using System.Windows;

namespace WpfWithWF
{

    public sealed class WfMessageBox : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var text = context.GetValue(Text);
            MessageBox.Show(text);
            var wce =  context.GetExtension<WorkflowCommunicationExtension>();
            wce.CommunicationContent = text;
            
        }
    }
}
