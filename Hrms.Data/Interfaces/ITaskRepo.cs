using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface ITaskRepo : IBaseRepository<Task>
    {
        TaskDetailsResponse GetTaskDetails(TaskActionRequest request);
        GetTasksResponse UpdateTask(UpdateTaskRequest request);
        BaseResponse DeleteTask(TaskActionRequest request);
        BaseResponse ToggleStartTask(TaskActionRequest request);
        BaseResponse ToggleCompleteTask(TaskActionRequest request);
        BaseResponse ToggleVerifyTask(TaskActionRequest request);
        BaseResponse ToggleIrrelevant(TaskActionRequest request);
        BaseResponse AddCommentToTask(AddTaskCommentRequest request);
        BaseResponse DeleteCommentOnTask(TaskCommentActionRequest request);
        GetTaskCommentsResponse GetTaskComments(TaskActionRequest request);
    }
}