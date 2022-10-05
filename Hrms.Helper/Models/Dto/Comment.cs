using System;

namespace Hrms.Helper.Models.Dto
{

    public class CommentDto
    {
        public string CommentId { get; set; }
        public string Comment { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public bool IsCreator { get; set; }
    }
}