﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace WpfWithWF
{

    public sealed class InputBoxActivity : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        public OutArgument<System.Windows.MessageBoxResult> UserClickedButton { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);
            context.SetValue(UserClickedButton, System.Windows.MessageBox.Show(
                text, "FIB->FIS Handover", System.Windows.MessageBoxButton.YesNo));
        }
    }
}
