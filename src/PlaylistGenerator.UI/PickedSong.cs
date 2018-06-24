using PlaylistGenerator.Core;

namespace PlaylistGenerator.UI
{
    public class PickedSong
    {
        private readonly Song song;

        public int Number { get; }
        public string Artist { get; }
        public string Name { get; }
        public string Path { get; }

        public PickedSong(int number, Song song)
        {
            this.song = song;

            Number = number;
            Artist = song.Artist;
            Name = song.Name;
            Path = song.Path;
        }

        public Song GetSong() => song;
    }
}