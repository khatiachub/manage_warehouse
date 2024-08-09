namespace manage_warehouse.Models
{
    public class ProductModel
    {
        public string barcode { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public string? entry_date { get; set; }
        public string? exit_date { get; set; }
        public int? operator_id { get; set; }
        //public int id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? warehouse_name { get; set; }
        public string? unit { get; set; }
        public int? current_balance { get; set; }
    }
}
