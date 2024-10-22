namespace AccountServer.Data
{
    public class FacebookResponseJsonData
    {
        public FacebookTokenData data { get; set; } = new FacebookTokenData();
    }

    public class FacebookTokenData
    {
        public long app_id { get; set; }
        public string application { get; set; } = string.Empty;
        public long expires_at { get; set; }
        public bool is_valid { get; set; }
        public string user_id { get; set; } = string.Empty;
    }
}
