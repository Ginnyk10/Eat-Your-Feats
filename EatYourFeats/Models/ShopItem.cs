using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class ShopItem
    {
        public ObjectId Id { get; set; }
        public string ItemName { get; set; }
        public int ItemPrice { get; set; }
        public string ItemEffect { get; set; }

        public string ItemDescription { get; set; }
    }
}
