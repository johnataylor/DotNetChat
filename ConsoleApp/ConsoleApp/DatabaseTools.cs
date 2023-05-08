namespace DotNetChat
{
    public class DatabaseTools
    {
        [Tool(Description = "Provides information about an artist. Input is the name of the artist.")]
        public static Task<string> ArtistInfoAsync(string artistName)
        {
            switch (artistName)
            {
                case "Bob Dylan":
                    return Task.FromResult(artistName + " was born May 24, 1941.");
                case "Paul Simon":
                    return Task.FromResult(artistName + " was born October 13, 1941.");
                case "Leonard Cohen":
                    return Task.FromResult(artistName + " was born September 21, 1934.");
                default:
                    return Task.FromResult("nothing is known about " + artistName);
            }
        }

        [Tool(Description = "Provides information about the albums by the artist. Input is the name of the artist.")]
        public static Task<string> ArtistAlbumInfoAsync(string artistName)
        {
            switch (artistName)
            {
                case "Bob Dylan":
                    return Task.FromResult(artistName + " made albums: Shot of Love, Saved, Slow Train a Comin'");
                case "Paul Simon":
                    return Task.FromResult(artistName + " made the album Gracelands. I'm sure its no coincidence but Elvis Presley lived in a house that went by the same name.");
                case "Leonard Cohen":
                    return Task.FromResult(artistName + " made albums I'm Your Man and Various Positions");
                default:
                    return Task.FromResult("I don't know about any ablbums made by " + artistName);
            }
        }
    }
}
