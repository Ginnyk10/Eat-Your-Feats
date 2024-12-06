using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class InventoryItem
    {
        public ObjectId Id { get; set; }
        public ObjectId GameId { get; set; }
        public string ItemName { get; set; }

        public bool IsEquipped { get; set; }
        public DateTime TimeEquipped { get; set; }
    }
}
