using CategoryMapper.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Web;

namespace CategoryMapper.FunctionApp;

public class GetCategoriesByParent
{
    private readonly ICategoryService _service;

    public GetCategoriesByParent(ICategoryService service)
    {
        _service = service;
    }

    [Function("GetCategoriesByParent")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories")] HttpRequestData req)
    {
        var query = HttpUtility.ParseQueryString(req.Url.Query);

        Guid? parentId = null;
        if (Guid.TryParse(query["parentId"], out var parsedParent))
            parentId = parsedParent;

        int page = int.TryParse(query["page"], out var p) ? p : 1;
        int size = int.TryParse(query["size"], out var s) ? s : 50;
        bool includeProducts = bool.TryParse(query["includeProducts"], out var ip) && ip;

        var categories = await _service.GetCategoriesByParentAsync(parentId, page, size, includeProducts);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(categories);
        return response;
    }
}
