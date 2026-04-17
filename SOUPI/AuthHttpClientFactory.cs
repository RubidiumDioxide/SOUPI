using System.Net;


namespace SOUPI 
{
    public class AuthHttpClientFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHttpClientFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public HttpClient CreateClient(Uri baseAddress)
        {
            var cookieContainer = new CookieContainer();

            var cookies = _httpContextAccessor.HttpContext?.Request.Cookies;
            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    cookieContainer.Add(baseAddress, new Cookie(cookie.Key, cookie.Value));
                }
            }

            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = baseAddress,
            };

            return client;
        }
    }
}

