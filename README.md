# Playlist generator for DirEttore

Generates DirEttore playlists while observing a distance between the last artist and song.

## Usage

```
Usage: Playlist generator for DirEttore (.dpl files) [options]

Options:
  -?|-h|--help                       Show help information
  -v|--version                       Show version information
  -d|--directory <musicdirectory>    Required, this needs to point to the directory that is being searched for mp3 files
  -o|--output <playlistoutput>       Optional, if specified this will be the file to which the new playlist will be written, needs to end in .dpl. If not specified a random file name will be generated.
  -ps|--size <playlistsize>          Optional, this will determine the size of the playlist to generate, set to 200 by default.
  -as|--spacing <artistsongspacing>  Optional, this will determine the distance observed between artists and songs, set to 100 by default.

This app can generate playlists for the DirEttore software which observe a distance between artists and songs played.
Only files with an .mp3 extension will be considered for the playlist.

```

## Example: 

`.\PlaylistGenerator.exe  -d "C:\temp\Music" -o "C:\temp\test2.dpl"`
