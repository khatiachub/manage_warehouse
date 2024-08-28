namespace manage_warehouse.Models
{
    public class CompanyRegisterModel
    {
        public int? id {  get; set; }
        public string company_name { get; set; }
        public string address { get; set; }
        public int? mobile { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string? email { get; set; }
        public int role_id { get; set; }
    }
}
