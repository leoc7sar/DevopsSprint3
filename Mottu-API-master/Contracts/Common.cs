namespace Mottu.Api.Contracts;

public record PageLink(string Rel, string Href, string Method);
public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int Total, IEnumerable<PageLink> Links);
