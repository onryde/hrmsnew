using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Hrms.Helper.StaticClasses
{
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }

    public class JsonByteContent : ByteArrayContent
    {
        public JsonByteContent(object obj) :
            base(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)))
        { }
    }
}
