using PlaylistGenerator.Core;
using System;
using System.Collections.Generic;

namespace PlaylistGenerator.UI
{
    public class SongPickStateChangedEventArgs : EventArgs
    {
        public Exception Error { get; }

        public IEnumerable<PickedSong> Result { get; }

        public string Message { get; }

        public bool IsProcessing { get; }

        private SongPickStateChangedEventArgs(string stateMessage, bool isProcessing)
        {
            Message = stateMessage;
            IsProcessing = isProcessing;
        }

        public SongPickStateChangedEventArgs(string stateMessage)
            : this(stateMessage, true)
        {

        }

        public SongPickStateChangedEventArgs(string stateMessage, Exception error)
            : this(stateMessage, false)
        {
            Error = error;
        }

        public SongPickStateChangedEventArgs(string stateMessage, IEnumerable<PickedSong> result)
            : this(stateMessage, false)
        {
            Result = result;
        }
    }
}