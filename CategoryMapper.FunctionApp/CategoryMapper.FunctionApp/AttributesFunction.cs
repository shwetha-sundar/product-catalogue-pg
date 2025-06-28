using CategoryMapper.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Web;

namespace CategoryMapper.FunctionApp;

public class GetAttributes
{
    private readonly IAttributeService _attributeService;

    public GetAttributes(IAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [Function("GetAttributes")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "attributes")] HttpRequestData req)
    {
        var query = HttpUtility.ParseQueryString(req.Url.Query);

        string[] categoryIds = query["categoryIds"]?.Split(',') ?? [];
        string[] linkTypes = query["linkTypes"]?.Split(',').Select(lt => lt.ToLowerInvariant()).ToArray() ?? [];
        string keyword = query["keyword"];
        bool notApplicable = query["notApplicable"] == "true";
        int page = int.TryParse(query["page"], out var p) ? p : 1;
        int size = int.TryParse(query["size"], out var s) ? s : 50;

        var attributes = await _attributeService.GetAttributesAsync(
            new List<Guid>(categoryIds.Select(id => Guid.Parse(id))), 
            linkTypes.ToList(), 
            keyword, 
            notApplicable, 
            page, 
            size);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(attributes);
        return response;
    }
}
