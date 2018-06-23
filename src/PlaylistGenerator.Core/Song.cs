namespace PlaylistGenerator.Core
{
    public class Song
    {
        public string Artist { get; }
        public string Name { get; }
        public string Path { get; }

        public Song(string artist, string name, string path)
        {
            Artist = artist;
            Name = name;
            Path = path;
        }
    }
}