using GroceryList.Application.DTOs;
using GroceryList.Application.Exceptions;
using GroceryList.Application.Interfaces;
using GroceryList.Application.Services;
using GroceryList.Domain.Entities;
using Moq;

namespace GroceryList.Application.Tests;

/// <summary>
/// Unit tests for GroceryItemService (user stories 3 to 7).
/// The key thing these tests prove: a user can never read or change an item that is not theirs.
/// </summary>
public class GroceryItemServiceTests
{
    private readonly Mock<IGroceryItemRepository> _repository = new();
    private readonly GroceryItemService _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _otherUserId = Guid.NewGuid();

    public GroceryItemServiceTests()
    {
        _sut = new GroceryItemService(_repository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyTheItemsOfThatUser()
    {
        var items = new List<GroceryItem> { new() { Id = Guid.NewGuid(), UserId = _userId, Name = "Milk" } };
        _repository.Setup(r => r.GetAllByUserAsync(_userId)).ReturnsAsync(items);

        var result = await _sut.GetAllAsync(_userId);

        Assert.Single(result);
        Assert.Equal("Milk", result[0].Name);
    }

    [Fact]
    public async Task AddAsync_WithAValidName_CreatesTheItemUntaggedForThatUser()
    {
        var result = await _sut.AddAsync(_userId, new CreateGroceryItemRequest("Bread"));

        Assert.Equal("Bread", result.Name);
        Assert.False(result.IsInBasket);
        _repository.Verify(r => r.AddAsync(It.Is<GroceryItem>(i =>
            i.UserId == _userId && i.Name == "Bread" && !i.IsInBasket)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddAsync_WithAnEmptyName_ThrowsValidation(string name)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.AddAsync(_userId, new CreateGroceryItemRequest(name)));
    }

    [Fact]
    public async Task DeleteAsync_WithAnItemOwnedByTheUser_DeletesIt()
    {
        var item = new GroceryItem { Id = Guid.NewGuid(), UserId = _userId, Name = "Milk" };
        _repository.Setup(r => r.GetByIdAsync(item.Id, _userId)).ReturnsAsync(item);

        await _sut.DeleteAsync(_userId, item.Id);

        _repository.Verify(r => r.DeleteAsync(item));
    }

    [Fact]
    public async Task DeleteAsync_WithAnItemOwnedByAnotherUser_ThrowsNotFound()
    {
        var itemId = Guid.NewGuid();
        // The repository already filters by owner, so it returns null for another user's item.
        _repository.Setup(r => r.GetByIdAsync(itemId, _otherUserId)).ReturnsAsync((GroceryItem?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(_otherUserId, itemId));

        _repository.Verify(r => r.DeleteAsync(It.IsAny<GroceryItem>()), Times.Never);
    }

    [Fact]
    public async Task SetInBasketAsync_Tagging_SetsIsInBasketTrue()
    {
        var item = new GroceryItem { Id = Guid.NewGuid(), UserId = _userId, Name = "Milk", IsInBasket = false };
        _repository.Setup(r => r.GetByIdAsync(item.Id, _userId)).ReturnsAsync(item);

        var result = await _sut.SetInBasketAsync(_userId, item.Id, isInBasket: true);

        Assert.True(result.IsInBasket);
        _repository.Verify(r => r.UpdateAsync(It.Is<GroceryItem>(i => i.IsInBasket)));
    }

    [Fact]
    public async Task SetInBasketAsync_Untagging_SetsIsInBasketFalse()
    {
        var item = new GroceryItem { Id = Guid.NewGuid(), UserId = _userId, Name = "Milk", IsInBasket = true };
        _repository.Setup(r => r.GetByIdAsync(item.Id, _userId)).ReturnsAsync(item);

        var result = await _sut.SetInBasketAsync(_userId, item.Id, isInBasket: false);

        Assert.False(result.IsInBasket);
    }

    [Fact]
    public async Task SetInBasketAsync_OnAnotherUsersItem_ThrowsNotFound()
    {
        var itemId = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(itemId, _otherUserId)).ReturnsAsync((GroceryItem?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.SetInBasketAsync(_otherUserId, itemId, isInBasket: true));
    }
}
