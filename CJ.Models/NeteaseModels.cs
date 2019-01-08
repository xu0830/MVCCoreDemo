using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Models
{
    public class NeteaseApiModel<T>
    {
        public int code { get; set; }
        public T result { get; set; }
    }

    public class NeteaseSongsModel
    {
        public List<Song> songs { get; set; }
        public int songCount { get; set; }
    }

    public class NeteaseAlbumsModel
    {
        public List<Album> albums { get; set; }
        public int albumCount { get; set; }
    }

    public class NeteaseArtistModel
    {
        public List<Artist> artists { get; set; }
        public int artistCount { get; set; }
    }

    public class NeteaseRecommendSong
    {
        public int code { get; set; }
        public List<Song> recommend { get; set; }
    }

    public class Song
    {
        public string name { get; set; }
        public int id { get; set; }
        public List<Artist> artists { get; set; }
        public Album album { get; set; }
        public int mvid { get; set; }
    }

    public class Artist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string img1v1Url { get; set; }
    }

    public class Album
    {
        public int id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
        public Artist artist { get; set; }
        public string publishTime { get; set; }
        public string type { get; set; }
    }


}
