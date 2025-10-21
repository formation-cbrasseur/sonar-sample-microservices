namespace Play.Inventory.Service.Dtos
{
    public record AddItemDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryDto(Guid CatalogItemId, string Name, string Description, int Quantity);
}
