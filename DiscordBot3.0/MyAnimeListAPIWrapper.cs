using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using RestSharp.Deserializers;

namespace DiscordBot3._0
{
    /// <summary>
    /// the class the data to be deserialized to for Anime searches
    /// </summary>
    [DeserializeAs(Name = "entry")]
    internal class _Anime
    {
        public int id { get; set; }
        public string title { get; set; }
        public string english { get; set; }
        public string synonyms { get; set; }
        public int episodes { get; set; }
        public string score { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string synopsis { get; set; }
        public string image { get; set; }
    }

    /// <summary>
    /// a rapper to protect the deserlized data For Animes Searches
    /// </summary>
    public class Anime
    {
        /// <summary>
        /// the ID of the anime
        /// </summary>
        public int id { get { return A.id; } }
        /// <summary>
        /// the proper tite
        /// </summary>
        public string title { get { return A.title; } }
        /// <summary>
        /// the english title
        /// </summary>
        public string english { get { return A.english; } }
        /// <summary>
        /// other names
        /// </summary>
        public string synonyms { get { return A.synonyms; } }
        /// <summary>
        /// number of episodes
        /// </summary>
        public int episodes { get { return A.episodes; } }
        /// <summary>
        /// the average score users of the site gave it
        /// </summary>
        public string score { get { return A.score; } }
        /// <summary>
        /// the type
        /// </summary>
        public string type { get { return A.type; } }
        /// <summary>
        /// status
        /// </summary>
        public string status { get { return A.status; } }
        /// <summary>
        /// the date it was shown
        /// </summary>
        public string start_date { get { return A.start_date; } }
        /// <summary>
        /// the date it ended
        /// </summary>
        public string end_date { get { return A.end_date; } }
        /// <summary>
        /// the basic plot
        /// </summary>
        public string synopsis { get { return A.synopsis; } }
        /// <summary>
        /// a url to the image
        /// </summary>
        public string image { get { return A.image; } }

        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<Anime, string> ToStringOverRide { private get; set; }

        /// <summary>
        /// data source
        /// </summary>
        _Anime A;

        internal Anime(_Anime Get)
        {
            A = Get;
        }

        /// <summary>
        /// an overide to the to string. calls Func<MangaResult, string> ToStringOverRide unless null or defualts
        /// </summary>
        /// <returns>Func<MangaResult, string> ToStringOverRide return or the base return</returns>
        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }
    }

    /// <summary>
    /// an indexable class that will allow you to view the anime search results nicely
    /// </summary>
    public class AnimeResult
    {
        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<AnimeResult, string> ToStringOverRide { private get; set; }

        private List<_Anime> _Animes;

        /// <summary>
        /// the count of values in the list
        /// </summary>
        public int Count { get { return _Animes != null ? _Animes.Count : 0; } }

        /// <summary>
        /// allows you yo grab values from the list. will return null if index is nonexistant
        /// </summary>
        /// <param name="Index">the index</param>
        /// <returns>A useable Anime to pull from</returns>
        public Anime this[int Index]
        {
            get
            {
                return _Animes != null && Index < _Animes.Count() ?
                new Anime(_Animes[Index])
                : null;
            }
        }

        internal AnimeResult(List<_Anime> ResultsIn)
        {
            _Animes = ResultsIn;
        }

        /// <summary>
        /// an overide to the to string. calls Func<MangaResult, string> ToStringOverRide unless null or defualts
        /// </summary>
        /// <returns>Func<MangaResult, string> ToStringOverRide return or the base return</returns>
        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }
    }

    /// <summary>
    /// the class the data to be deserialized to for Manga searches
    /// </summary>
    [DeserializeAs(Name = "entry")]
    internal class _Manga
    {
        public int id { get; set; }
        public string title { get; set; }
        public string english { get; set; }
        public string synonyms { get; set; }
        public int chapters { get; set; }
        public int volumes { get; set; }
        public string score { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string synopsis { get; set; }
        public string image { get; set; }
    }

    /// <summary>
    /// a rapper to protect the deserlized data for managa
    /// </summary>
    public class Manga
    {
        /// <summary>
        /// the ID
        /// </summary>
        public int id { get { return M.id; } }
        /// <summary>
        /// the proper title
        /// </summary>
        public string title { get { return M.title; } }
        /// <summary>
        /// the english name
        /// </summary>
        public string english { get { return M.english; } }
        /// <summary>
        /// other names
        /// </summary>
        public string synonyms { get { return M.synonyms; } }
        /// <summary>
        /// the number of chapters
        /// </summary>
        public int chapters { get { return M.chapters; } }
        /// <summary>
        /// the number of valumes
        /// </summary>
        public int volumes { get { return M.volumes; } }
        /// <summary>
        /// the average score by sites users
        /// </summary>
        public string score { get { return M.score; } }
        /// <summary>
        /// the type 
        /// </summary>
        public string type { get { return M.type; } }
        /// <summary>
        /// Is it discontiued? still going
        /// </summary>
        public string status { get { return M.status; } }
        /// <summary>
        /// the date it started
        /// </summary>
        public string start_date { get { return M.start_date; } }
        /// <summary>
        /// the date it ended
        /// </summary>
        public string end_date { get { return M.end_date; } }
        /// <summary>
        /// the Basic plot
        /// </summary>
        public string synopsis { get { return M.synopsis; } }
        /// <summary>
        /// a url to a image to use
        /// </summary>
        public string image { get { return M.image; } }

        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<Manga, string> ToStringOverRide { private get; set; }

        /// <summary>
        /// the source class
        /// </summary>
        _Manga M;

        internal Manga(_Manga Get)
        {
            M = Get;
        }

        /// <summary>
        /// an overide to the to string. calls Func<MangaResult, string> ToStringOverRide unless null or defualts
        /// </summary>
        /// <returns>Func<MangaResult, string> ToStringOverRide return or the base return</returns>
        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }
    }

    /// <summary>
    /// an indexable class that will allow you to view the manga search results nicely
    /// </summary>
    public class MangaResult
    {
        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<MangaResult, string> ToStringOverRide { private get; set; }

        /// <summary>
        /// the ture list of mangas
        /// </summary>
        private List<_Manga> _Mangas;

        /// <summary>
        /// the count of animes you could have
        /// </summary>
        public int Count { get { return _Mangas != null ? _Mangas.Count : 0; } }

        /// <summary>
        /// allows you yo grab values from the list. will return null if index is nonexistant
        /// </summary>
        /// <param name="Index">the index</param>
        /// <returns>A useable Manga to pull from</returns>
        public Manga this[int Index]
        {
            get
            {
                return _Mangas != null && Index < _Mangas.Count() ?
                new Manga(_Mangas[Index])
                : null;
            }
        }

        /// <summary>
        /// internal constructor mine
        /// </summary>
        /// <param name="ResultsIn"></param>
        internal MangaResult(List<_Manga> ResultsIn)
        {
            _Mangas = ResultsIn;
        }

        /// <summary>
        /// an overide to the to string. calls Func<MangaResult, string> ToStringOverRide unless null or defualts
        /// </summary>
        /// <returns>Func<MangaResult, string> ToStringOverRide return or the base return</returns>
        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }
    }

    /// <summary>
    /// The client class
    /// </summary>
    class MyAnimeListClient
    {
        /// <summary>
        /// a static client
        /// </summary>
        static RestClient _Client = new RestClient("http://myanimelist.net");

        /// <summary>
        /// the constructor
        /// </summary>
        /// <param name="Account">Your account</param>
        /// <param name="Password">Your Password</param>
        public MyAnimeListClient(string Account, string Password)
        {
            _Client.Authenticator = new HttpBasicAuthenticator(Account, Password);
            _Client.PreAuthenticate = false;
        }

        /// <summary>
        /// Seaches for an anime
        /// </summary>
        /// <param name="Name">The name to seach</param>
        /// <returns>Anime list In the AnimeResult</returns>
        public async Task<AnimeResult> SearchAnime(string Name)
        {
            RestRequest RestRequest = new RestRequest("/api/anime/search.xml", Method.GET);
            RestRequest.AddParameter("q", Name, ParameterType.QueryString);

            IRestResponse<List<_Anime>> Response = await _Client.ExecuteGetTaskAsync<List<_Anime>>(RestRequest);

            if (Response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, Response.ErrorException);
            }

            return new AnimeResult(Response.Data);
        }

        /// <summary>
        /// Seaches for an manga
        /// </summary>
        /// <param name="Name">The name to search</param>
        /// <returns>Anime list In the AnimeResult</returns>
        public async Task<MangaResult> SearchManga(string Name)
        {
            RestRequest RestRequest = new RestRequest("/api/manga/search.xml", Method.GET);
            RestRequest.AddParameter("q", Name, ParameterType.QueryString);

            IRestResponse<List<_Manga>> Response = await _Client.ExecuteGetTaskAsync<List<_Manga>>(RestRequest);

            if (Response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, Response.ErrorException);
            }

            return new MangaResult(Response.Data);
        }

    }
}
