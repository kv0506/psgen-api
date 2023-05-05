using System.Net;
using CSharpExtensions;
using Microsoft.Azure.Functions.Worker.Http;
using PsGenApi.ServiceModel;
using PsGenApi.Services;

namespace PsGenApi;

public class FunctionBase
{
    protected async Task<T?> ReadRequestBody<T>(HttpRequestData req)
    {
        var requestContent = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
        return requestContent.IsNotNullOrWhiteSpace() ? SerializationService.Deserialize<T>(requestContent) : default;
    }

    protected async Task<HttpResponseData> Success<T>(HttpRequestData req, T apiResponse)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(apiResponse).ConfigureAwait(false);
        return response;
    }

    protected async Task<HttpResponseData> HttpMethodNotSupportedError(HttpRequestData req)
    {
        return await Error(req, "Http Method not supported");
    }

    protected async Task<HttpResponseData> InvalidRequestPayloadError(HttpRequestData req)
    {
        return await Error(req, "Invalid request payload");
    }

    protected async Task<HttpResponseData> Error(HttpRequestData req, string error)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new ApiResponse { IsSuccess = false, Message = error }).ConfigureAwait(false);
        return response;
    }

    protected async Task<HttpResponseData> AuthError(HttpRequestData req, string error)
    {
        var response = req.CreateResponse(HttpStatusCode.Unauthorized);
        await response.WriteAsJsonAsync(new ApiResponse { IsSuccess = false, Message = error }).ConfigureAwait(false);
        return response;
    }
}