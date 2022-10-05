using Newtonsoft.Json;

namespace Hrms.Helper.Models.Contracts
{

    public class BaseRequest
    {
        [JsonIgnore]
        public string UserId { get; set; }
        [JsonIgnore]
        public string LoginToken { get; set; }
        [JsonIgnore]
        public long UserIdNum { get; set; }
        [JsonIgnore]
        public string UserRole { get; set; }
        [JsonIgnore]
        public long CompanyIdNum { get; set; }
        [JsonIgnore]
        public string UserName { get; set; }
    }
}
