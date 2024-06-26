using System.Net;
using System.Text;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class BaseService : IBaseService
{
    public APIResponse ResponseModel { get; set; }
    protected IHttpClientFactory HttpClient { get; set; }
    public BaseService(IHttpClientFactory httpClient)
    {
        HttpClient = httpClient;
        ResponseModel = new();
    }
    public async Task<T?> SendAsync<T>(APIRequest apiRequest)
    {
        try {
            var client = HttpClient.CreateClient("VillaAPI");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);

            if (apiRequest.Token is not null) {
                message.Headers.Add("Authorization", "Bearer " + apiRequest.Token);
            }
                
            if (apiRequest.Data is not null) {
                // Has data, need to serilize this data to the body of the request
                message.Content = new StringContent(
                        JsonConvert.SerializeObject(apiRequest.Data), 
                        Encoding.UTF8, "application/json"
                    );
            }

            switch (apiRequest.ApiType) {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = await client.SendAsync(message);
            
            string apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseObject = JsonConvert.DeserializeObject<T>(apiContent);

            return apiResponseObject;
        } catch (Exception e) {
            var dto = new APIResponse() {
                StatusCode = HttpStatusCode.InternalServerError,
                IsSuccess = false,
                ErrorMessages = new Dictionary<string, List<string>>() {
                    { "", new List<string>() {"The API Gateway occured an error!"} }
                },
            };
            var res = JsonConvert.SerializeObject(dto);
            var apiResponseObject = JsonConvert.DeserializeObject<T>(res);
            return apiResponseObject;
        }
    }
}