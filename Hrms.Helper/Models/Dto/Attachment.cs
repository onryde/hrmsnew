namespace Hrms.Helper.Models.Dto
{
    public class AttachmentDto
    {
        public string AttachmentId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public double Size { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public bool IsActive { get; set; }
        public bool IsNew { get; set; }
    }
}