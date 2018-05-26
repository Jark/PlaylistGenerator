using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistGenerator
{
    public class SongPersister
    {
        public void PersistSongs(StreamWriter writer, IReadOnlyList<Song> songs)
        {
            writer.WriteLine("[Playlist]");
            writer.WriteLine("SequencePlaylist=False");
            writer.WriteLine("SequencePoint=0");
            writer.WriteLine($"ElementsCount={songs.Count}");

            for(var i = 0; i < songs.Count; i++)
            {
                var song = songs[i];
                int elementNumber = i + 1;
                writer.WriteLine($"ElementName{elementNumber}={song.Artist} - {song.Name}");
                writer.WriteLine($"ElementType{elementNumber}=AUDIO");
                writer.WriteLine($"ElementPath{elementNumber}={song.Path}");
            }
            writer.WriteLine();
        }
    }
}
