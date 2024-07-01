using System;
using VillaWeb.Models;

namespace VillaWeb.Services.IServices;

public interface IBaseService
{
    Task<T?> SendAsync<T>(APIRequest apiRequest, bool withBearerToken = true);
}