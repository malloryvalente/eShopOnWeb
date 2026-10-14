using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification
{
    [Fact]
    public void ReturnsAllCatalogItems()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null);

        var result = spec.Evaluate(GetTestCollection());

        Assert.NotNull(result);
        Assert.Equal(4, result.ToList().Count);
    }

    [Fact]
    public void Returns2CatalogItemsWithSameBrandAndTypeId()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, 1, 1);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.ToList().Count);
    }

    [Theory]
    [InlineData(null, null, null, null, 6)]
    [InlineData(null, null, null, 10, 1)] // $10.00 is excluded: max is exclusive
    [InlineData(null, null, 10, 15, 2)] // $10.00 included (min inclusive), $15.00 excluded
    [InlineData(null, null, 15, 20, 2)] // $15.00 included, $20.00 excluded
    [InlineData(null, null, 20, null, 1)]
    [InlineData(1, null, null, null, 3)] // price cleared, brand still applied
    [InlineData(2, 1, 10, 20, 1)]
    public void ReturnsExpectedItemsWithPriceRange(int? brandId, int? typeId, int? minPrice, int? maxPrice, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, brandId, typeId, minPrice, maxPrice);

        var result = spec.Evaluate(GetPricedCollection()).ToList();

        Assert.Equal(expectedCount, result.Count);
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(null, null, null, 10)]
    [InlineData(null, null, 10, 15)]
    [InlineData(null, null, 15, 20)]
    [InlineData(null, null, 20, null)]
    [InlineData(1, null, 10, 20)]
    [InlineData(2, 1, 10, 20)]
    public void MatchesTheSameItemsAsCatalogFilterSpecification(int? brandId, int? typeId, int? minPrice, int? maxPrice)
    {
        var items = GetPricedCollection();
        var countSpec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId, minPrice, maxPrice);
        var pageSpec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 0, brandId, typeId, minPrice, maxPrice);

        Assert.Equal(countSpec.Evaluate(items).ToList(), pageSpec.Evaluate(items).ToList());
    }

    private List<CatalogItem> GetPricedCollection()
    {
        return new List<CatalogItem>()
        {
            new CatalogItem(1, 1, "Item A", "Item A", 5.00m, "TestUri"),
            new CatalogItem(1, 1, "Item B", "Item B", 10.00m, "TestUri"),
            new CatalogItem(2, 1, "Item C", "Item C", 14.99m, "TestUri"),
            new CatalogItem(1, 2, "Item D", "Item D", 15.00m, "TestUri"),
            new CatalogItem(2, 2, "Item E", "Item E", 19.99m, "TestUri"),
            new CatalogItem(1, 2, "Item F", "Item F", 20.00m, "TestUri"),
        };
    }

    private List<CatalogItem> GetTestCollection()
    {
        var catalogItemList = new List<CatalogItem>();

        catalogItemList.Add(new CatalogItem(1, 1, "Item 1", "Item 1", 1.00m, "TestUri1"));
        catalogItemList.Add(new CatalogItem(1, 1, "Item 1.5", "Item 1.5", 1.50m, "TestUri1"));
        catalogItemList.Add(new CatalogItem(2, 2, "Item 2", "Item 2", 2.00m, "TestUri2"));
        catalogItemList.Add(new CatalogItem(3, 3, "Item 3", "Item 3", 3.00m, "TestUri3"));

        return catalogItemList;
    }
}
