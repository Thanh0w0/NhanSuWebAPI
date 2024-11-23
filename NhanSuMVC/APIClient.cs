using System.Net.Http.Headers;

namespace NhanSuMVC
{
    public class APIClient
    {
        public static HttpClient WebApiClient = new HttpClient();
        static APIClient()
        {
            // URL của API chính xác
            WebApiClient.BaseAddress = new Uri("https://localhost:7299/");
            WebApiClient.DefaultRequestHeaders.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
