using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using PlaylistGenerator.Core;

namespace PlaylistGenerator.UI
{
    public class SaveToFileWorker
    {
        private BackgroundWorker backgroundWorker;

        public EventHandler<SaveToFileCompletedEventArgs> Completed;
        private string fileName;
        private ImmutableArray<Song> songs;

        public SaveToFileWorker()
        {
            backgroundWorker = new BackgroundWorker();

            backgroundWorker.RunWorkerCompleted += OnRunWorkerCompleted;
            backgroundWorker.DoWork += OnDoWork;
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Completed?.Invoke(this, new SaveToFileCompletedEventArgs(e.Error));
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            var enc1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            using (var writer = new StreamWriter(fileName, false, enc1252))
            {
                var fileSaver = new SongPersister();
                fileSaver.PersistSongs(writer, songs);
            }
        }

        public void Execute(string fileName, IEnumerable<PickedSong> pickedSongs)
        {
            this.fileName = fileName;
            songs = pickedSongs.Select(x => x.GetSong()).ToImmutableArray();

            backgroundWorker.RunWorkerAsync();
        }
    }
}
