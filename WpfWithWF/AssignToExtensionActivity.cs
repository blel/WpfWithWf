using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace WpfWithWF
{

    public sealed class AssignToExtensionActivity : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<List<string>> Text { get; set; }
        public InArgument<bool> DoFlush { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            bool doFlush = context.GetValue(this.DoFlush);
            List<string> text = context.GetValue(this.Text);
            if (doFlush)
                context.GetExtension<List<string>>().Clear();
            context.GetExtension<List<string>>().AddRange(text);
        }
    }
}
