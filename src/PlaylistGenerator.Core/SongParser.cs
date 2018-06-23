using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlaylistGenerator.Core
{
    public class SongParser
    {
        private readonly Regex songRegex;

        public SongParser()
        {
            songRegex = new Regex("(?<artist>.*) - (?<song>.*?)$");
        }

        public IReadOnlyList<Song> ParseSongs(IEnumerable<string> paths)
        {
            return paths.Select(ParseFileName).ToArray();
        }

        private Song ParseFileName(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var match = songRegex.Match(fileName);
            if (!match.Success)
                throw new System.Exception($"Cannot resolve artist and song name from path: {path}.");

            return new Song(match.Groups["artist"].Value, match.Groups["song"].Value, path);
        }
    }
}
