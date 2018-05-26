using System;
using System.Collections.Generic;

namespace PlaylistGenerator
{
    public class SongPicker
    {
        private readonly Random random;
        private readonly int playlistSize;
        private readonly int artistAndSongSpacing;

        public SongPicker(int playlistSize, int artistAndSongSpacing)
        {
            this.random = new Random();

            this.playlistSize = playlistSize;
            this.artistAndSongSpacing = artistAndSongSpacing;
        }

        public IReadOnlyList<Song> PickSongs(IReadOnlyList<Song> allSongs)
        {
            // https://softwareengineering.stackexchange.com/questions/194480/id-like-to-write-an-ultimate-shuffle-algorithm-to-sort-my-mp3-collection
            var playedSongNames = new LinkedList<string>();
            var playedArtists = new LinkedList<string>();

            var playList = new List<Song>();
            var recursionCount = 0;
            while (playList.Count < playlistSize)
            {
                var song = allSongs[random.Next(allSongs.Count)];
                if (playedArtists.Contains(song.Artist) || playedSongNames.Contains(song.Name))
                {
                    recursionCount++;
                    if (recursionCount >= playlistSize)
                        throw new Exception("Could not pick the next song, try reducing the artist and song spacing.");
                    continue;
                }
                recursionCount = 0;

                playedArtists.AddLast(song.Artist);
                playedSongNames.AddLast(song.Name);
                playList.Add(song);
                if (playedArtists.Count > artistAndSongSpacing)
                    playedArtists.RemoveFirst();
                if (playedSongNames.Count > artistAndSongSpacing)
                    playedSongNames.RemoveFirst();
            }

            return playList;
        }
    }
}
