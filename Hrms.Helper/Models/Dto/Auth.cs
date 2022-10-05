namespace Hrms.Helper.Models.Dto
{
    public class EmployeeTokenDto
    {
        public string Name { get; set; }
        public long EmployeeId { get; set; }
        public string Guid { get; set; }
        public string Role { get; set; }
        public long CompanyId { get; set; }
    }
}