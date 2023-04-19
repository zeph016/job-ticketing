using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;

namespace fgciitjo
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;
        public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
        {
            _localStorageService = localStorageService;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //Lazy Loader defer auth to load very fast
            await Task.Delay(2000);
            //Get From localstorage
            string authToken = await _localStorageService.GetItemAsStringAsync("token");
            //Default values
            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;
            //

            //if Authentication Token exists from localstorage
            if (!string.IsNullOrEmpty(authToken))
            {
                try {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _http.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", authToken.Replace("\"",""));
                } catch {
                    //Set to unauthorized
                    await _localStorageService.RemoveItemAsync("token");
                    identity = new ClaimsIdentity();
                }
            }

            //Set to unauthorized
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);
            //

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch(base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

         //Identity or Decode the authToken authorization
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonbytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonbytes);
            var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
            return claims;
        }
    }
}