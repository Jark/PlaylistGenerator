using NUnit.Framework;
using PlaylistGenerator;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class SongParserTests
    {
        [Test]
        public void CanParseExamplesSuccesfully()
        {
            var paths = Examples.GetAll();
            
            var songParser = new SongParser();
            var songs = songParser.ParseSongs(paths);
            var mismatchedFileNames = songs.Select(x => $"\\{x.Artist} - {x.Name}.mp3").Where(x => !paths.Any(y => y.EndsWith(x))).ToArray();

            Assert.That(songs.Count, Is.EqualTo(paths.Length));
            Assert.That(songs.Select(x => x.Path).All(x => paths.Contains(x)), Is.True);
            
            Assert.That(mismatchedFileNames.Length, Is.EqualTo(0));

            Assert.Pass();
        }
    }
}