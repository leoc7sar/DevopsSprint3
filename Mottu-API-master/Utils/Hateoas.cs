using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Mottu.Api.Contracts;

namespace Mottu.Api.Utils;

public static class Hateoas
{
  public static IEnumerable<PageLink> PaginateLinks(HttpContext ctx, string routeName, int page, int pageSize, int total)
  {
    var links = new List<PageLink>();
    string Base(string r, int p) => $"{ctx.Request.Scheme}://{ctx.Request.Host}/api/v1/{r}?page={p}&pageSize={pageSize}";
    links.Add(new("self", Base(routeName, page), "GET"));
    if ((page - 1) * pageSize > 0) links.Add(new("prev", Base(routeName, page - 1), "GET"));
    if (page * pageSize < total) links.Add(new("next", Base(routeName, page + 1), "GET"));
    return links;
  }
}
