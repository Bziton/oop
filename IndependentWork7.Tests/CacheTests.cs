using IndependentWork7;

namespace IndependentWork7.Tests;

public class CacheTests
{
    [Fact]
    public void Add_NewItem_ItemIsStored()
    {
        Cache<string, int> cache = new Cache<string, int>(3);

        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
        Assert.Equal(1, cache.Get("a"));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutGrowing()
    {
        Cache<string, int> cache = new Cache<string, int>(3);

        cache.Add("a", 1);
        cache.Add("a", 100);

        Assert.Equal(100, cache.Get("a"));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Get_ExistingKeys_ReturnsCorrectValues()
    {
        Cache<int, string> cache = new Cache<int, string>(3);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three");

        Assert.Equal("one", cache.Get(1));
        Assert.Equal("two", cache.Get(2));
        Assert.Equal("three", cache.Get(3));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        Cache<string, int> cache = new Cache<string, int>(2);
        cache.Add("a", 1);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("b"));
    }

    [Fact]
    public void Add_OverCapacity_RemovesOldestItem()
    {
        Cache<string, int> cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
        Assert.Throws<KeyNotFoundException>(() => cache.Get("a"));
    }

    [Fact]
    public void Add_OverCapacitySeveralTimes_RemovesInFifoOrder()
    {
        Cache<int, int> cache = new Cache<int, int>(3);
        for (int i = 1; i <= 5; i++)
        {
            cache.Add(i, i * 10);
        }

        Assert.False(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
        Assert.True(cache.ContainsKey(4));
        Assert.True(cache.ContainsKey(5));
    }

    [Fact]
    public void Add_OverCapacity_GetDoesNotChangeOrder()
    {
        Cache<string, int> cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);
        cache.Get("a");

        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
    }

    [Fact]
    public void ContainsKey_ReturnsTrueOnlyForAddedKeys()
    {
        Cache<string, string> cache = new Cache<string, string>(2);
        cache.Add("key", "value");

        Assert.True(cache.ContainsKey("key"));
        Assert.False(cache.ContainsKey("other"));
    }

    [Fact]
    public void Count_EmptyCache_IsZero()
    {
        Cache<string, int> cache = new Cache<string, int>(5);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_IncreasesWithAdd()
    {
        Cache<string, int> cache = new Cache<string, int>(5);
        cache.Add("a", 1);
        cache.Add("b", 2);

        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Count_NeverExceedsCapacity()
    {
        Cache<int, int> cache = new Cache<int, int>(3);
        for (int i = 0; i < 10; i++)
        {
            cache.Add(i, i);
        }

        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void Constructor_InvalidCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<string, int>(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<string, int>(-1));
    }
}
