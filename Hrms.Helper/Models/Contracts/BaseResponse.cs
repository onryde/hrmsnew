using Newtonsoft.Json;

namespace Hrms.Helper.Models.Contracts
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public int MgAccess { get; set; }
        public int HrAccess { get; set; }
        public int EmpAccess { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string RefreshToken { get; set; }
    }
}
