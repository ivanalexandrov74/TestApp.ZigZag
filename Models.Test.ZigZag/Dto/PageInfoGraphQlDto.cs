

namespace ZigZag.Test.Dto;

public class PageInfoGraphQlDto
{
    public bool hasNextPage { get; set; }
    public bool hasPreviousPage { get; set; }
    public string startCursor { get; set; }=string.Empty;
    public string endCursor { get; set; } = string.Empty;
}
