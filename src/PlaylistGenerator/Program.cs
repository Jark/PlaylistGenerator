using Microsoft.Extensions.CommandLineUtils;
using PlaylistGenerator.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlaylistGenerator
{
    class Program
    {
        private const int DefaultPlaylistSize = 200;
        private const int DefaultArtistSongSpacing = 100;

        static void Main(string[] args)
        {
            // documentation https://github.com/anthonyreilly/ConsoleArgs/

            var app = new CommandLineApplication();

            // This should be the name of the executable itself.
            // the help text line "Usage: ConsoleArgs" uses this
            app.Name = "Playlist generator for DirEttore (.dpl files)";
            app.Description = $"Program to generate long playlists for the DirEttore software based on a directory structure.";
            app.ExtendedHelpText = $"{Environment.NewLine}This app can generate playlists for the DirEttore software which observe a distance between artists and songs played.{Environment.NewLine}Only files with an .mp3 extension will be considered for the playlist.";

            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", () => {
                return string.Format("Version {0}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            });

            var musicDirectory = app.Option("-d|--directory <musicdirectory>",
                    "Required, this needs to point to the directory that is being searched for mp3 files",
                    CommandOptionType.SingleValue);

            var outputOption = app.Option("-o|--output <playlistoutput>",
                   $"Optional, if specified this will be the file to which the new playlist will be written, needs to end in .dpl. If not specified a random file name will be generated.",
                   CommandOptionType.SingleValue);

            var playListSizeOption = app.Option("-ps|--size <playlistsize>",
                   $"Optional, this will determine the size of the playlist to generate, set to {DefaultPlaylistSize} by default.",
                   CommandOptionType.SingleValue);

            var artistSongSpacingOption = app.Option("-as|--spacing <artistsongspacing>",
                  $"Optional, this will determine the distance observed between artists and songs, set to {DefaultArtistSongSpacing} by default.",
                  CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                // TODO: add sweepers directory option
                // TODO: add option to define pattern (JINGLE / SWEEPERS / MUSIC, etc.)
                var outputFileName = outputOption.HasValue() ? outputOption.Value() : $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.dpl";

                if (!musicDirectory.HasValue() || string.IsNullOrWhiteSpace(musicDirectory.Value()))
                {
                    Console.Error.WriteLine("The directory from which songs are sourced needs to be specified.");
                    app.ShowHint();
                    return -1;
                }

                if (outputOption.HasValue() && Path.GetExtension(outputOption.Value()) != ".dpl")
                {
                    Console.Error.WriteLine("The output filename needs to end in '.dpl'.");
                    app.ShowHint();
                    return -1;
                }

                int playListSize = DefaultPlaylistSize;
                if (playListSizeOption.HasValue() && !int.TryParse(playListSizeOption.Value(), out playListSize))
                {
                    Console.Error.WriteLine("Invalid playlist size specified.");
                    app.ShowHint();
                    return -1;
                }

                int artistSongSpacing = DefaultArtistSongSpacing;
                if (artistSongSpacingOption.HasValue() && !int.TryParse(artistSongSpacingOption.Value(), out artistSongSpacing))
                {
                    Console.Error.WriteLine("Invalid artist and song spacing specified.");
                    app.ShowHint();
                    return -1;
                }

                Console.WriteLine("Source of music files: {0}", musicDirectory.Value());
                Console.WriteLine("Playlist to write to: {0}", outputFileName);

                Console.WriteLine("Searching for mp3 files...");
                var allFiles = Directory.EnumerateFiles(musicDirectory.Value(), "*.mp3", SearchOption.AllDirectories).ToArray();
                Console.WriteLine($"Found {allFiles.Length} to consider for playlist generation.");

                var songParser = new SongParser();
                Console.WriteLine($"Splitting mp3 files into artist and song name...");
                var songs = songParser.ParseSongs(allFiles);

                Console.WriteLine($"Generating playlist of size {playListSize} and not repeating artist or song name for the last {artistSongSpacing} songs.");
                var songPicker = new SongPicker(playListSize, artistSongSpacing);
                var pickedSongs = songPicker.PickSongs(songs);

                Console.WriteLine($"Saving {pickedSongs.Count} songs to directory {outputFileName}.");
                var enc1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
                using (var writer = new StreamWriter(outputFileName, false, enc1252))
                {
                    var songPersister = new SongPersister();
                    songPersister.PersistSongs(writer, pickedSongs);
                }
                Console.WriteLine($"Saving finished.");

                return 0;
            });

            try
            {
                Console.WriteLine("Executing Playlist generator...");
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.Error.WriteLine("ERROR: {0}", ex.Message);
            }
        }
    }
}
