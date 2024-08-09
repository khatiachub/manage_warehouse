namespace manage_warehouse.Models
{
    public class UserRegisterModel
    {
        public string name { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public int mobile { get; set; }
        public string password { get; set; }
        public int role_id { get; set; }
        public int? id { get; set; }
        public int company_id { get; set; }
        public int? warehouse_id { get; set; }
    }
}
