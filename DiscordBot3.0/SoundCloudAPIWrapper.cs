#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using RestSharp.Deserializers;
using System.IO;
using System.Net;
using NAudio.Wave;

namespace DiscordBot3._0
{

    [DeserializeAs(Name = "tracks")]
    internal class _tracks
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public int user_id { get; set; }
        public int duration { get; set; }
        public bool commentable { get; set; }
        public string state { get; set; }
        public string sharing { get; set; }
        public string tag_list { get; set; }
        public string permalink { get; set; }
        public string description { get; set; }
        public bool streamable { get; set; }
        public bool downloadable { get; set; }
        public string genre { get; set; }
        public string release { get; set; }
        public string purchase_url { get; set; }
        public int label_id { get; set; }
        public string label_name { get; set; }
        public string isrc { get; set; }
        public string video_url { get; set; }
        public string track_type { get; set; }
        public string key_signature { get; set; }
        public int bpm { get; set; }
        public string title { get; set; }
        public string release_year { get; set; }
        public string release_month { get; set; }
        public string release_day { get; set; }
        public string original_format { get; set; }
        public string original_content_size { get; set; }
        public string license { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
        public string artwork_url { get; set; }
        public string waveform_url { get; set; }
        public _user user { get; set; }
        public string stream_url { get; set; }
        public string download_url { get; set; }
        public int playback_count { get; set; }
        public int download_count { get; set; }
        public int favoritings_count { get; set; }
        public int comment_count { get; set; }
        public _created_with created_with { get; set; }
        public string attachments_uri { get; set; }
    }

    public class track
    {
        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<track, string> ToStringOverRide { private get; set; }

        _tracks tracks;
        user _user;
        created_with _created_with;

        internal track(_tracks DataIn)
        {
            tracks = DataIn;
            _user = new user(DataIn.user);
            _created_with = new created_with(DataIn.created_with);
        }

        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }

        public int id { get { return tracks.id; } }
        public string created_at { get { return tracks.created_at; } }
        public int user_id { get { return tracks.user_id; } }
        public int duration { get { return tracks.duration; } }
        public bool commentable { get { return tracks.commentable; } }
        public string state { get { return tracks.state; } }
        public string sharing { get { return tracks.sharing; } }
        public string tag_list { get { return tracks.tag_list; } }
        public string permalink { get { return tracks.permalink; } }
        public string description { get { return tracks.description; } }
        public bool streamable { get { return tracks.streamable; } }
        public bool downloadable { get { return tracks.downloadable; } }
        public string genre { get { return tracks.genre; } }
        public string release { get { return tracks.release; } }
        public string purchase_url { get { return tracks.purchase_url; } }
        public int label_id { get { return tracks.label_id; } }
        public string label_name { get { return tracks.label_name; } }
        public string isrc { get { return tracks.isrc; } }
        public string video_url { get { return tracks.video_url; } }
        public string track_type { get { return tracks.track_type; } }
        public string key_signature { get { return tracks.key_signature; } }
        public int bpm { get { return tracks.bpm; } }
        public string title { get { return tracks.title; } }
        public string release_year { get { return tracks.release_year; } }
        public string release_month { get { return tracks.release_month; } }
        public string release_day { get { return tracks.release_day; } }
        public string original_format { get { return tracks.original_format; } }
        public string original_content_size { get { return tracks.original_content_size; } }
        public string license { get { return tracks.license; } }
        public string uri { get { return tracks.uri; } }
        public string permalink_url { get { return tracks.permalink_url; } }
        public string artwork_url { get { return tracks.artwork_url; } }
        public string waveform_url { get { return tracks.waveform_url; } }
        public user user { get { return _user; } }
        public string stream_url { get { return tracks.stream_url; } }
        public string download_url { get { return tracks.download_url; } }
        public int playback_count { get { return tracks.playback_count; } }
        public int download_count { get { return tracks.download_count; } }
        public int favoritings_count { get { return tracks.favoritings_count; } }
        public int comment_count { get { return tracks.comment_count; } }
        public created_with created_with { get { return _created_with; } }
        public string attachments_uri { get { return tracks.attachments_uri; } }
    }

    public class trackResults
    {
        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<trackResults, string> ToStringOverRide { private get; set; }

        List<_tracks> tracks;

        internal trackResults(List<_tracks> DataIn)
        {
            tracks = DataIn;
        }

        public track this[int Index]
        {
            get
            {
                return tracks != null && Index < tracks.Count() ?
                new track(tracks[Index])
                : null;
            }
        }

        public int Count { get { return tracks != null ? tracks.Count : 0; } }

        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }


    }

    [DeserializeAs(Name = "user")]
    internal class _user
    {
        public int id { get; set; }
        public string permalink { get; set; }
        public string username { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
        public string avatar_url { get; set; }
    }

    public class user
    {
        /// <summary>
        /// an exposed point to override the to string method with a function or lambda
        /// </summary>
        public static Func<user, string> ToStringOverRide { private get; set; }

        _user _user;


        internal user(_user DataIn)
        {
            _user = DataIn;
        }
        
        public int id { get { return _user.id; } }
        public string permalink { get { return _user.permalink; } }
        public string username { get { return _user.username; } }
        public string uri { get { return _user.uri; } }
        public string permalink_url { get { return _user.permalink_url; } }
        public string avatar_url { get { return _user.avatar_url; } }

        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }
    }

    [DeserializeAs(Name = "created_with")]
    internal class _created_with
    {
        public int id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
    }

    public class created_with
    {
        public static Func<created_with, string> ToStringOverRide { private get; set; }

        _created_with _created_with;


        internal created_with(_created_with DataIn)
        {
            _created_with = DataIn;
        }

        public int id { get { return _created_with.id; } }
        public string name { get { return _created_with.name; } }
        public string uri { get { return _created_with.uri; } }
        public string permalink_url { get { return _created_with.permalink_url; } }
        public override string ToString()
        {
            if (ToStringOverRide != null)
                return ToStringOverRide.Invoke(this);
            return base.ToString();
        }
    }

    public class SoundCloudClient
    {
        static RestClient _Client = new RestClient("http://api.soundcloud.com");

        public string _ClientID;

        public SoundCloudClient(string ClientID)
        {
            _ClientID = ClientID;
        }

        public async Task<trackResults> SearchTrack(string Name)
        {
            RestRequest RestRequest = new RestRequest("/tracks", Method.GET);
            RestRequest.AddParameter("client_id", _ClientID, ParameterType.QueryString);
            RestRequest.AddParameter("q", Name, ParameterType.QueryString);

            IRestResponse<List<_tracks>> Response = await _Client.ExecuteGetTaskAsync<List<_tracks>>(RestRequest);

            if (Response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, Response.ErrorException);
            }

            return new trackResults(Response.Data);
        }

        //https://api.soundcloud.com/tracks/13158665/stream

        public async Task<Stream> GetStream(int ID, int channelCount, int rate = 48000)
        {

            string url = String.Format("http://api.soundcloud.com/tracks/{0}/stream?client_id={1}", ID, _ClientID);
            Stream S = new MemoryStream();
            var OutFormat = new WaveFormat(rate, 16, channelCount);
            using (Stream stream = WebRequest.Create(url)
                .GetResponse().GetResponseStream())
            {
                byte[] buffer = new byte[OutFormat.AverageBytesPerSecond];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    S.Write(buffer, 0, read);
                }
            }

            return S;
        }

    }
}
#endif
