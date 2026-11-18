namespace IndependentWork7.Tests;

public class CacheTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_NonPositiveCapacity_ThrowsArgumentOutOfRangeException(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, string>(capacity));
    }

    [Fact]
    public void Add_NewItem_IncreasesCount()
    {
        var cache = new Cache<int, string>(3);

        cache.Add(1, "one");

        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutChangingCount()
    {
        var cache = new Cache<int, string>(3);
        cache.Add(1, "one");

        cache.Add(1, "uno");

        Assert.Equal(1, cache.Count);
        Assert.Equal("uno", cache.Get(1));
    }

    [Fact]
    public void Add_ExistingKey_DoesNotRefreshItemAge()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        cache.Add(1, "uno");
        cache.Add(3, "three");

        Assert.False(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
    }

    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        var cache = new Cache<string, int>(3);
        cache.Add("a", 10);
        cache.Add("b", 20);

        Assert.Equal(20, cache.Get("b"));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<int, string>(3);

        Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
    }

    [Fact]
    public void Get_DoesNotChangeEvictionOrder()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Get("a");
        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var cache = new Cache<int, string>(3);
        cache.Add(1, "one");

        Assert.True(cache.ContainsKey(1));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        var cache = new Cache<int, string>(3);
        cache.Add(1, "one");

        Assert.False(cache.ContainsKey(2));
    }

    [Fact]
    public void Count_EmptyCache_ReturnsZero()
    {
        var cache = new Cache<int, string>(3);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_AfterOverflow_EqualsCapacity()
    {
        var cache = new Cache<int, string>(3);

        for (int i = 1; i <= 10; i++)
        {
            cache.Add(i, $"value{i}");
        }

        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void Add_OverCapacity_RemovesOldestItem()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        cache.Add(3, "three");

        Assert.False(cache.ContainsKey(1));
        Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
    }

    [Fact]
    public void Add_OverCapacity_KeepsNewerItems()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        cache.Add(3, "three");

        Assert.Equal("two", cache.Get(2));
        Assert.Equal("three", cache.Get(3));
    }

    [Fact]
    public void Add_ManyItems_RemovesInInsertionOrder()
    {
        var cache = new Cache<int, string>(3);

        for (int i = 1; i <= 6; i++)
        {
            cache.Add(i, $"value{i}");
        }

        Assert.False(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.False(cache.ContainsKey(3));
        Assert.True(cache.ContainsKey(4));
        Assert.True(cache.ContainsKey(5));
        Assert.True(cache.ContainsKey(6));
    }
}