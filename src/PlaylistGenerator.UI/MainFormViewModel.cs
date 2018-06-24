using Eto.Forms;
using PlaylistGenerator.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PlaylistGenerator.UI
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand SaveCommand { get; }
        public RelayCommand OpenSelectMusicDirectoryCommand { get; }
        public RelayCommand PickSongsCommand { get; }

        private readonly SongPickWorker songPickWorker;
        private readonly SaveToFileWorker saveToFileWorker;

        private ObservableCollection<PickedSong> _pickedSongs;
        public ObservableCollection<PickedSong> PickedSongs
        {
            get { return _pickedSongs; }
            set { _pickedSongs = value; OnPropertyChanged(); }
        }

        private string _musicDirectory;
        public string MusicDirectory
        {
            get { return _musicDirectory; }
            set { _musicDirectory = value; OnPropertyChanged(); }
        }

        private int _playlistSize;
        public int PlaylistSize
        {
            get { return _playlistSize; }
            set { _playlistSize = value; OnPropertyChanged(); }
        }

        private int _artistAndSongSpacing;
        public int ArtistAndSongSpacing
        {
            get { return _artistAndSongSpacing; }
            set { _artistAndSongSpacing = value; OnPropertyChanged(); }
        }

        private string _stateMessage;
        public string StateMessage
        {
            get { return _stateMessage; }
            set { _stateMessage = value; OnPropertyChanged(); }
        }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { _isProcessing = value; OnPropertyChanged(); }
        }

        public MainFormViewModel()
        {
            SaveCommand = new RelayCommand(SaveToFile, () => PickedSongs.Count > 0 && !IsProcessing);
            OpenSelectMusicDirectoryCommand = new RelayCommand(OpenSelectMusicDirectory, () => !IsProcessing);
            PickSongsCommand = new RelayCommand(PickSongs, () => !string.IsNullOrWhiteSpace(MusicDirectory) && !IsProcessing);

            PickedSongs = new ObservableCollection<PickedSong>();
            PlaylistSize = 20;
            ArtistAndSongSpacing = 0;

            songPickWorker = new SongPickWorker();
            songPickWorker.StateChanged += OnSongPickWorkerStateChanged;

            saveToFileWorker = new SaveToFileWorker();
            saveToFileWorker.Completed += OnSaveToFileWorkerCompleted;
        }

        private void OpenSelectMusicDirectory()
        {
            var openFileDialog = new SelectFolderDialog();
            openFileDialog.Title = "Music directory selection";

            var result = openFileDialog.ShowDialog(Application.Instance.MainForm);
            if (result == DialogResult.Ok)
            {
                var directory = openFileDialog.Directory;
                if (Directory.Exists(directory))
                {
                    MusicDirectory = directory;
                    return;
                }

                ShowError($"Cannot resolve directory {directory}, check that you have sufficient rights to that directory and that the directory exists.");
            }
        }

        private void PickSongs()
        {
            IsProcessing = true;
            try
            {
                songPickWorker.Execute(MusicDirectory, PlaylistSize, ArtistAndSongSpacing);
            }
            catch (Exception ex)
            {
                ShowError($"Error starting the picking songs process: {ex.Message}");
                IsProcessing = false;
            }
        }

        private void SaveToFile()
        {
            IsProcessing = true;
            try
            {
                var saveToFileDialog = new SaveFileDialog();
                saveToFileDialog.Filters.Clear();
                saveToFileDialog.Filters.Add(new FileFilter("DirEttore playlist", ".dpl"));

                var result = saveToFileDialog.ShowDialog(Application.Instance.MainForm);
                if (result != DialogResult.Ok)
                {
                    IsProcessing = false;
                    return;
                }

                var fileName = saveToFileDialog.FileName;
                StateMessage = $"Saving playlist to {fileName}";
                saveToFileWorker.Execute(fileName, PickedSongs);
            }
            catch (Exception ex)
            {
                ShowError($"Error saving the playlist: {ex.Message}");
                StateMessage = "Error saving the playlist";
                IsProcessing = false;
            }
        }

        private void OnSongPickWorkerStateChanged(object sender, SongPickStateChangedEventArgs e)
        {
            StateMessage = e.Message;
            if (e.Error != null)
            {
                ShowError($"Song pick process failed: {e.Error.Message}.");
                PickedSongs.Clear();
            }
            else if (!e.IsProcessing)
            {
                PickedSongs = new ObservableCollection<PickedSong>(e.Result);
            }
            IsProcessing = e.IsProcessing;
        }

        private void OnSaveToFileWorkerCompleted(object sender, SaveToFileCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ShowError($"Error while trying to save to file: {e.Error.Message}");
                StateMessage = $"Error while trying to save to file";

                return;
            }
            else
                StateMessage = $"File successfully saved";

            IsProcessing = false;
        }

        private void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxType.Error);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            OpenSelectMusicDirectoryCommand.UpdateCanExecute();
            PickSongsCommand.UpdateCanExecute();
            SaveCommand.UpdateCanExecute();
        }
    }
}
