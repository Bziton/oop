using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class InMemoryRepositoryTests
{
    private static User CreateUser(string username = "oleh")
    {
        return new User(username, username + "@example.com", true);
    }

    [Fact]
    public void Add_NewItem_AssignsIdAndStoresItem()
    {
        var repository = new InMemoryRepository<User>();
        var user = CreateUser();

        repository.Add(user);

        Assert.Equal(1, user.Id);
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void Add_MultipleItems_AssignsUniqueIds()
    {
        var repository = new InMemoryRepository<User>();
        var first = CreateUser("oleh");
        var second = CreateUser("anna");

        repository.Add(first);
        repository.Add(second);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Add_DuplicateId_ThrowsInvalidOperationException()
    {
        var repository = new InMemoryRepository<User>();
        repository.Add(CreateUser("oleh"));
        var duplicate = CreateUser("anna");
        duplicate.Id = 1;

        Assert.Throws<InvalidOperationException>(() => repository.Add(duplicate));
    }

    [Fact]
    public void Add_Null_ThrowsArgumentNullException()
    {
        var repository = new InMemoryRepository<User>();

        Assert.Throws<ArgumentNullException>(() => repository.Add(null!));
    }

    [Fact]
    public void GetById_ExistingId_ReturnsItem()
    {
        var repository = new InMemoryRepository<User>();
        var user = CreateUser();
        repository.Add(user);

        var result = repository.GetById(user.Id);

        Assert.Same(user, result);
    }

    [Fact]
    public void GetById_NonExistingId_ReturnsNull()
    {
        var repository = new InMemoryRepository<User>();
        repository.Add(CreateUser());

        var result = repository.GetById(99);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_EmptyRepository_ReturnsEmptyCollection()
    {
        var repository = new InMemoryRepository<User>();

        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void GetAll_WithItems_ReturnsAllItems()
    {
        var repository = new InMemoryRepository<User>();
        repository.Add(CreateUser("oleh"));
        repository.Add(CreateUser("anna"));
        repository.Add(CreateUser("ivan"));

        List<User> result = repository.GetAll().ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, u => u.Username == "anna");
    }

    [Fact]
    public void Update_ExistingItem_ReplacesData()
    {
        var repository = new InMemoryRepository<User>();
        var user = CreateUser();
        repository.Add(user);
        var updated = new User("oleh_new", "new@example.com", false) { Id = user.Id };

        repository.Update(updated);

        var result = repository.GetById(user.Id);
        Assert.NotNull(result);
        Assert.Equal("oleh_new", result!.Username);
        Assert.Equal("new@example.com", result.Email);
        Assert.False(result.IsActive);
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void Update_NonExistingItem_ThrowsKeyNotFoundException()
    {
        var repository = new InMemoryRepository<User>();
        var user = CreateUser();
        user.Id = 99;

        Assert.Throws<KeyNotFoundException>(() => repository.Update(user));
    }

    [Fact]
    public void Delete_ExistingId_RemovesItem()
    {
        var repository = new InMemoryRepository<User>();
        var user = CreateUser();
        repository.Add(user);

        repository.Delete(user.Id);

        Assert.Null(repository.GetById(user.Id));
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void Delete_NonExistingId_ThrowsKeyNotFoundException()
    {
        var repository = new InMemoryRepository<User>();

        Assert.Throws<KeyNotFoundException>(() => repository.Delete(99));
    }
}