using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface ITrainingRepo : IBaseRepository<Training>
    {
        TrainingListResponse GetTrainings(TrainingFilterRequest request);
        TrainingListResponse GetTrainingCalendar(GetHolidaysRequset request);
        BaseResponse UpdateTraining(UpdateTrainingRequest request);
        BaseResponse UpdateNomineeResponse(TrainingNomineeRequest request);
        BaseResponse ConfirmTraining(TrainingActionRequest request);
        BaseResponse FillAttendance(TrainingAttendanceRequest request);
        BaseResponse AddMoreNominees(AddNomineesRequest request);
        BaseResponse CompleteTraining(TrainingActionRequest request);
        BaseResponse SubmitTrainingFeedback(SubmitFeedbackRequest request);
        TrainingDetailsResponse GetTrainingDetails(TrainingActionRequest request);
        BaseResponse CancelTraining(TrainingCancellationActionRequest request);
        BaseResponse StartTraining(TrainingActionRequest request);
        BaseResponse CloseFeedbackForTraining(TrainingActionRequest request);
    }
}
