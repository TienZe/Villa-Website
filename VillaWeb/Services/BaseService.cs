using System.Net;
using System.Text;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Infrastructures;
using VillaWeb.Models;
using VillaWeb.Models.Dto;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class BaseService : IBaseService
{
    protected IHttpClientFactory HttpClient { get; set; }
    protected ITokenProvider TokenProvider { get; set; }
    protected ISignInService SignInService { get; set; }
    protected readonly string _villaApiUrl;
    public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, IConfiguration configuration, ISignInService signInService)
    {
        HttpClient = httpClient;
        TokenProvider = tokenProvider;
        SignInService = signInService;

        _villaApiUrl = configuration["ServiceUrls:VillaAPI"];

    }
    public async Task<T?> SendAsync<T>(APIRequest apiRequest, bool withBearerToken = true)
    {
        try {
            var client = HttpClient.CreateClient("VillaAPI");

            var messageFactory = () => {
                HttpRequestMessage message = new() {
                    RequestUri = new Uri(apiRequest.Url)
                };

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
                            Encoding.UTF8, 
                            "application/json"
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

                return message;
            };

            HttpResponseMessage apiResponse;
            if (withBearerToken) {
                apiResponse = await SendWithRefreshTokenAsync(client, messageFactory);
            } else {
                apiResponse = await client.SendAsync(messageFactory());
            }
            
            string apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseObject = JsonConvert.DeserializeObject<T>(apiContent);

            return apiResponseObject;
        } catch (UnauthorizedException) {
            throw;
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

    public async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient client, Func<HttpRequestMessage> requestMessageFactory)
    {
        // Send requeset with bearer access token
        var message = requestMessageFactory();

        var tokenDTO = TokenProvider.GetToken();
        if (tokenDTO is not null) {
            message.Headers.Add("Authorization", "Bearer " + tokenDTO.AccessToken);
        }

        var response = await client.SendAsync(message);
        if (response.IsSuccessStatusCode) {
            return response;
        }

        // If the response is not success, we need to check the response status code
        // If the status code is 401 - Unauthorized, we need to refresh the token
        if (response.StatusCode == HttpStatusCode.Unauthorized) {
            var newToken = await InvokeRefreshTokenEndpoint(client, tokenDTO);
            
            if (newToken is null) {
                // Sign out the user
                await SignInService.SignOutAsync();

                throw new UnauthorizedException();
            } else {
                // Sign in the user with the new token
                await SignInService.SignInAsync(newToken);

                // Retry the request with the new token
                var newMessage = requestMessageFactory();
                newMessage.Headers.Add("Authorization", "Bearer " + newToken.AccessToken);

                var newResponse = await client.SendAsync(newMessage);
                return newResponse;
            }
        } 

        return response;
    }

    /// <summary>
    /// Invoke the refresh API endpoint to get a new token
    /// </summary>
    /// <param name="client"></param>
    /// <param name="tokenDTO"></param>
    /// <returns>The new TokenDTO</returns>
    private async Task<TokenDTO> InvokeRefreshTokenEndpoint(HttpClient client, TokenDTO tokenDTO)
    {
        HttpRequestMessage message = new() {
            RequestUri = new Uri(_villaApiUrl + "/api/UserAuth/refresh"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonConvert.SerializeObject(tokenDTO), Encoding.UTF8, "application/json")
        };
        
        var response = await client.SendAsync(message);
        var apiContent = await response.Content.ReadAsStringAsync();
        var apiResponseObject = JsonConvert.DeserializeObject<APIResponse>(apiContent);

        if (apiResponseObject is not null && apiResponseObject.IsSuccess) {
            var newToken = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(apiResponseObject.Result));
            return newToken;
        }

        return null;
    }
}