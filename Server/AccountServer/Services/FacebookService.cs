using AccountServer.Data;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AccountServer.Services
{
    public class FacebookService
    {
        HttpClient _httpClient;

        // {app_id}|{app_secret} 
        readonly string _accessToken = "901425398578279|_TvqNYFsGDd4yNUnLu-uaq54O_o"; // TODO Secret

        public FacebookService()
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("https://graph.facebook.com/") };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FacebookTokenData?> GetUserTokenData(string inputToken)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"debug_token?input_token={inputToken}&access_token={_accessToken}");

            if (!response.IsSuccessStatusCode)
                return null;

            string resultStr = await response.Content.ReadAsStringAsync();

            FacebookResponseJsonData? result = JsonConvert.DeserializeObject<FacebookResponseJsonData>(resultStr);
            if (result == null)
                return null;

            return result.data;
        }
    }
}
