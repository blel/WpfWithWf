using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace WpfWithWF
{

    public sealed class TriggerActivity : NativeActivity 
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string bookmarkName = context.GetValue(this.BookmarkName);
            if (bookmarkName == string.Empty)
                throw new ArgumentException("BookmarkName cannot be empty.", "BookmarkName");
            context.CreateBookmark(bookmarkName, new BookmarkCallback (OnBookmarkCallback));
        }

        void OnBookmarkCallback(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            System.Windows.MessageBox.Show("Callback");
        }
        protected override bool CanInduceIdle
        {
            get { return true; }
        }



    }



}
