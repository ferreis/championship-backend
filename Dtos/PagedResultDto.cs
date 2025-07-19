// Dtos/PagedResultDto.cs
using System.Collections.Generic;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}