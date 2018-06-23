using NUnit.Framework;
using PlaylistGenerator;
using PlaylistGenerator.Core;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class SongPickerTests
    {
        [Test]
        public void Tests()
        {
            const int PlaylistSize = 500;
            const int ArtistAndSongSpacing = 100;

            var paths = Examples.GetAll();
            var songParser = new SongParser();
            var songs = songParser.ParseSongs(paths);
            for (var i = 0; i < 1; i++)
            {
                var songPicker = new SongPicker(PlaylistSize, ArtistAndSongSpacing);
                var pickedSongs = songPicker.PickSongs(songs);

                Assert.That(pickedSongs.Count, Is.EqualTo(PlaylistSize));

                var artists = new Queue<string>();
                var names = new Queue<string>();

                foreach(var song in pickedSongs)
                {
                    Assert.That(artists.Contains(song.Artist), Is.False);
                    Assert.That(names.Contains(song.Name), Is.False);

                    artists.Enqueue(song.Artist);
                    names.Enqueue(song.Name);

                    if (artists.Count > ArtistAndSongSpacing)
                    {
                        artists.Dequeue();
                    }

                    if (names.Count > ArtistAndSongSpacing)
                    {
                        names.Dequeue();
                    }
                }
            }
        }
    }
}