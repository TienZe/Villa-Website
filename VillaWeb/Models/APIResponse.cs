﻿using System.Net;

namespace VillaWeb.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public Dictionary<string, List<string>> ErrorMessages { get; set; } = new();
    public object? Result { get; set; }
}