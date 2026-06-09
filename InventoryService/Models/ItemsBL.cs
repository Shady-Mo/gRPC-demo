namespace InventoryService.Models {
    public class ItemsBL {
        List<Item> items = [];

        public ItemsBL() {
            items.AddRange(new List<Item> {
                new Item { Id = 1, Quantity = 5 },
                new Item { Id = 2, Quantity = 10 },
                new Item { Id = 3, Quantity = 15 },
                new Item { Id = 4, Quantity = 20 },
                new Item { Id = 5, Quantity = 25 },
                new Item { Id = 6, Quantity = 30 },
                new Item { Id = 7, Quantity = 35 }
            });
        }

        public List<Item> GetAllItems() {
            return items;
        }

        public Item GetItemById(int id) {
            return items.FirstOrDefault(i => i.Id == id);
        }

        public List<Item> GetRequestedItems(List<int> requestedItemsIds) {
            return items.Where(i => requestedItemsIds.Contains(i.Id)).ToList();
        }
    }
}
