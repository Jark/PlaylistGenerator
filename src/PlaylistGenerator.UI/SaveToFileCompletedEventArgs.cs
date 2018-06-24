using System;

namespace PlaylistGenerator.UI
{
    public class SaveToFileCompletedEventArgs : EventArgs
    {
        public Exception Error { get; }

        public SaveToFileCompletedEventArgs(Exception error)
        {
            Error = error;
        }
    }
}