using System.Net;
using System.Text;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Infrastructures;
using VillaWeb.Models;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class BaseService : IBaseService
{
    protected IHttpClientFactory HttpClient { get; set; }
    protected ITokenProvider TokenProvider { get; set; }
    public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider)
    {
        HttpClient = httpClient;
        TokenProvider = tokenProvider;

    }
    public async Task<T?> SendAsync<T>(APIRequest apiRequest, bool withBearerToken = true)
    {
        try {
            var client = HttpClient.CreateClient("VillaAPI");
            HttpRequestMessage message = new() {
                RequestUri = new Uri(apiRequest.Url)
            };

            var tokenDTO = TokenProvider.GetToken();
            if (tokenDTO is not null) {
                message.Headers.Add("Authorization", "Bearer " + tokenDTO.AccessToken);
            }
                
            if (apiRequest.Data is not null) {
                // Has data, need to serilize this data to the body of the request

                if (apiRequest.ContentType == ContentTypes.MultipartFormData) {
                    var formData = new MultipartFormDataContent();

                    foreach (var prop in apiRequest.Data.GetType().GetProperties()) {
                        var propValue = prop.GetValue(apiRequest.Data);

                        if (propValue is null) continue;

                        if (propValue is IFormFile file) {
                            formData.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        } else {
                            formData.Add(new StringContent(propValue.ToString()!), prop.Name);
                        }
                    }

                    message.Content = formData;
                } else {
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(apiRequest.Data), 
                        Encoding.UTF8, "application/json"
                    );
                }
                
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