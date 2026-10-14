using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CatalogFilterSpecification
{
    [Theory]
    [InlineData(null, null, 5)]
    [InlineData(1, null, 3)]
    [InlineData(2, null, 2)]
    [InlineData(null, 1, 2)]
    [InlineData(null, 3, 1)]
    [InlineData(1, 3, 1)]
    [InlineData(2, 3, 0)]
    public void MatchesExpectedNumberOfItems(int? brandId, int? typeId, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId);

        var result = spec.Evaluate(GetTestItemCollection()).ToList();

        Assert.Equal(expectedCount, result.Count());
    }

    [Theory]
    [InlineData(null, null, null, null, 6)]
    [InlineData(null, null, null, 10, 1)] // $10.00 is excluded: max is exclusive
    [InlineData(null, null, 10, 15, 2)] // $10.00 included (min inclusive), $15.00 excluded
    [InlineData(null, null, 15, 20, 2)] // $15.00 included, $20.00 excluded
    [InlineData(null, null, 20, null, 1)]
    [InlineData(null, null, 25, null, 0)]
    [InlineData(1, null, null, null, 3)] // price cleared, brand still applied
    [InlineData(1, null, 10, 20, 2)]
    [InlineData(2, null, 10, 20, 2)]
    [InlineData(2, 1, 10, 20, 1)]
    [InlineData(1, 2, 15, null, 0)]
    public void MatchesExpectedNumberOfItemsWithPriceRange(int? brandId, int? typeId, int? minPrice, int? maxPrice, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId, minPrice, maxPrice);

        var result = spec.Evaluate(GetPricedItemCollection()).ToList();

        Assert.Equal(expectedCount, result.Count);
    }

    public List<CatalogItem> GetPricedItemCollection()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Name", 5.00m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Name", 10.00m, "FakePath"),
                new CatalogItem(2, 1, "Description", "Name", 14.99m, "FakePath"),
                new CatalogItem(1, 2, "Description", "Name", 15.00m, "FakePath"),
                new CatalogItem(2, 2, "Description", "Name", 19.99m, "FakePath"),
                new CatalogItem(1, 2, "Description", "Name", 20.00m, "FakePath"),
            };
    }

    public List<CatalogItem> GetTestItemCollection()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(2, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(3, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(1, 2, "Description", "Name", 0, "FakePath"),
                new CatalogItem(2, 2, "Description", "Name", 0, "FakePath"),
            };
    }
}
