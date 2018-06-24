using PlaylistGenerator.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace PlaylistGenerator.UI
{
    public class SongPickWorker
    {
        private BackgroundWorker backgroundWorker;

        private string musicDirectory;
        private int playlistSize;
        private int artistAndSongSpacing;

        public EventHandler<SongPickStateChangedEventArgs> StateChanged;

        public SongPickWorker()
        {
            backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            backgroundWorker.ProgressChanged += OnProgressChanged;
            backgroundWorker.RunWorkerCompleted += OnRunWorkerCompleted;
            backgroundWorker.DoWork += OnDoWork;
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var stateMessage = $"{e.UserState} - {e.ProgressPercentage}%";
            StateChanged?.Invoke(this, new SongPickStateChangedEventArgs(stateMessage));
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                StateChanged?.Invoke(this, new SongPickStateChangedEventArgs("Completed with error", e.Error));
            }
            else
            {
                var result = (IEnumerable<Song>)e.Result;
                var songs = result.Select((x, i) => new PickedSong(i + 1, x)).ToImmutableArray();
                var songPickStateChangedEventArgs = new SongPickStateChangedEventArgs($"Completed successfully playlist with {songs.Length} songs generated", songs);
                StateChanged?.Invoke(this, songPickStateChangedEventArgs);
            }
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker.ReportProgress(0, "Initialising parser");
            var songParser = new SongParser();
            backgroundWorker.ReportProgress(10, "Enumerating files");
            var files = Directory.EnumerateFiles(musicDirectory, "*", System.IO.SearchOption.AllDirectories)
                  .Where(x => new[] { ".aac", ".flac", ".mp3", ".ogg", ".wav", ".wma" }.Contains(Path.GetExtension(x)))
                  .ToImmutableList();

            backgroundWorker.ReportProgress(40, "Parsing songs");
            var parsedSongs = songParser.ParseSongs(files);

            backgroundWorker.ReportProgress(70, $"Picking songs from {parsedSongs.Count} found songs");
            var songPicker = new SongPicker(playlistSize, artistAndSongSpacing);
            var pickedSongs = songPicker.PickSongs(parsedSongs);
            backgroundWorker.ReportProgress(100, "Finished picking songs");

            e.Result = pickedSongs;
        }

        public void Execute(string musicDirectory, int playlistSize, int artistAndSongSpacing)
        {
            this.musicDirectory = musicDirectory;
            this.playlistSize = playlistSize;
            this.artistAndSongSpacing = artistAndSongSpacing;

            backgroundWorker.RunWorkerAsync();
        }
    }
}
