using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Controllers;

public static class ControllerExtensions
{
    public static IActionResult Success<T>(this ControllerBase controller, T data, string msg = "")
        => controller.Ok(ApiResponse<T>.Success(data, msg));

    public static IActionResult Fail<T>(this ControllerBase controller, string msg, T? data = default)
        => controller.Ok(ApiResponse<T>.Fail(msg, data));

    // Optional generic-less overload
    public static IActionResult Fail(this ControllerBase controller, string msg)
        => controller.Ok(ApiResponse<object>.Fail(msg));
}
