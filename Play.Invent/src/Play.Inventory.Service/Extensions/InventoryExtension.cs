using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Extensions
{
    public static class InventoryExtension
    {
        public static InventoryDto AsDto(this InventoryEntity item, string name, string description)
        {
            return new InventoryDto(item.Id, name, description, item.Quantity);
        }
    }
}
