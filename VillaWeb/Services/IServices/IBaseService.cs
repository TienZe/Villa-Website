using System;
using VillaWeb.Models;

namespace VillaWeb.Services.IServices;

public interface IBaseService
{
    APIResponse ResponseModel { get; set; }
    Task<T> SendAsync<T>(APIRequest apiRequest);
}