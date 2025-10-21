using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Extensions;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IRepository<InventoryEntity> _inventoryRepository;
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        public InventoryController(IRepository<InventoryEntity> inventoryRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            _inventoryRepository = inventoryRepository;
            _catalogItemRepository = catalogItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var inventoryItemEntities = await _inventoryRepository.GetAllAsync(item => item.UserId == userId);
            var catalogItemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await _catalogItemRepository.GetAllAsync(item => catalogItemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(AddItemDto addItem)
        {
            var inventoryItem = await _inventoryRepository.GetAsync(item => item.UserId == addItem.UserId
                && item.CatalogItemId == addItem.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = addItem.UserId,
                    CatalogItemId = addItem.CatalogItemId,
                    Quantity = addItem.Quantity,
                };

                await _inventoryRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += addItem.Quantity;
                await _inventoryRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}
