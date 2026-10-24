using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Paging;

namespace IpDeputyApi.Tests;

public class PagingTests
{
    private static readonly List<Teacher> Teachers =
    [
        new() { Id = 1, Name = "Олег", Surname = "Сидоренко", Patronymic = "Іванович", Email = "oleg@fit.edu" },
        new() { Id = 2, Name = "Анна", Surname = "Бойко", Patronymic = "Петрівна", Email = null },
        new() { Id = 3, Name = "Олена", Surname = "Бойко", Patronymic = "Олегівна", Email = "olena@fit.edu" }
    ];

    private static readonly IReadOnlyList<Expression<Func<Teacher, string?>>> SearchFields =
    [
        x => x.Surname,
        x => x.Name,
        x => x.Patronymic,
        x => x.Email
    ];

    private static readonly SortMap<Teacher> Sorts = new SortMap<Teacher>()
        .Add("surname", x => x.Surname)
        .Add("name", x => x.Name)
        .Default(query => query.OrderByDescending(x => x.Id));

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-5, 20)]
    [InlineData(10, 10)]
    [InlineData(7, 10)]
    [InlineData(20, 20)]
    [InlineData(35, 50)]
    [InlineData(100, 100)]
    [InlineData(1000, 100)]
    public void GetPageSize_SnapsToAllowedValues(int requested, int expected)
    {
        Assert.Equal(expected, new PageRequest { PageSize = requested }.GetPageSize());
    }

    [Theory]
    [InlineData(0, 1, 10, 1, 0)]
    [InlineData(25, 2, 10, 2, 10)]
    [InlineData(25, 9, 10, 3, 20)]
    [InlineData(25, 0, 10, 1, 0)]
    [InlineData(20, 3, 10, 2, 10)]
    public void PageWindow_ClampsPage(int total, int page, int pageSize, int expectedPage, int expectedSkip)
    {
        var window = PageWindow.Calculate(total, page, pageSize);

        Assert.Equal(expectedPage, window.Page);
        Assert.Equal(expectedSkip, window.Skip);
        Assert.Equal(pageSize, window.PageSize);
    }

    [Theory]
    [InlineData("descend", true)]
    [InlineData("DESCEND", true)]
    [InlineData("ascend", false)]
    [InlineData(null, false)]
    public void IsDescending(string? order, bool expected)
    {
        Assert.Equal(expected, new PageRequest { Order = order }.IsDescending);
    }

    [Fact]
    public void SortMap_SortsByKeyWithIdTieBreaker()
    {
        var ids = Sorts.Apply(Teachers.AsQueryable(), "surname", false).Select(x => x.Id).ToList();

        Assert.Equal([2, 3, 1], ids);
    }

    [Fact]
    public void SortMap_Descending()
    {
        var ids = Sorts.Apply(Teachers.AsQueryable(), "name", true).Select(x => x.Id).ToList();

        Assert.Equal([3, 1, 2], ids);
    }

    [Fact]
    public void SortMap_UnknownKeyUsesDefault()
    {
        var ids = Sorts.Apply(Teachers.AsQueryable(), "password", false).Select(x => x.Id).ToList();

        Assert.Equal([3, 2, 1], ids);
    }

    [Fact]
    public void SortMap_IdKeyIsAlwaysAvailable()
    {
        Assert.Contains("id", Sorts.Keys);
        Assert.Equal([3, 2, 1], Sorts.Apply(Teachers.AsQueryable(), "id", true).Select(x => x.Id).ToList());
    }

    [Theory]
    [InlineData("бойко", new[] { 2, 3 })]
    [InlineData("  БОЙКО   олена ", new[] { 3 })]
    [InlineData("олег", new[] { 1, 3 })]
    [InlineData("fit.edu", new[] { 1, 3 })]
    [InlineData("бойко fit", new[] { 3 })]
    [InlineData("невідомо", new int[0])]
    [InlineData("", new[] { 1, 2, 3 })]
    [InlineData(null, new[] { 1, 2, 3 })]
    public void ApplySearch_EveryWordMustMatchSomeField(string? search, int[] expected)
    {
        var ids = Teachers.AsQueryable().ApplySearch(search, SearchFields).Select(x => x.Id).OrderBy(x => x).ToArray();

        Assert.Equal(expected, ids);
    }

    [Fact]
    public void SplitWords_NormalizesAndDeduplicates()
    {
        Assert.Equal(["олег", "іван"], SearchExtensions.SplitWords(" Олег  ІВАН олег "));
    }
}
