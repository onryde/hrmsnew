using System.Collections.Generic;

namespace Hrms.Helper.Settings
{
    public class ApplicationConstants
    {
        public const string ErrorMessage = "Project= KUBOTA | Method= {0} | Other Data={1} | Reference Code= {2}";
        public const string UserSystemErrorMessage = "Unexpected server error occured. Please contact administrator with error code - {0}";
        public static List<string> AllowedExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".doc" };
        public static List<string> ImageExtensions = new List<string> { ".png", ".jpg", ".jpeg" };
        public static List<string> LogoExtensions = new List<string> { ".png" };
        public static List<string> UploadExtensions = new List<string> { ".tsv" };
    }

    public static class NotificationTemplates
    {
        public static int NewTask = 1;
        public static int DeleteTask = 2;
        public static int UpdateTask = 3;
        public static int ChangeAssignedTask = 4;
        public static int CompleteTask = 5;
        public static int VerifyTask = 6;
        public static int AddTaskCommentAssigned = 7;
        public static int AddTaskCommentAddedBy = 8;

        public static int AddAppraisal = 9;
        public static int UpdateAppraisal = 10;
        public static int DeleteAppraisal = 11;
        public static int SelfFilledAppraisal = 12;
        public static int RmFilledAppraisal = 13;
        public static int RatingAddedAppraisal = 14;

        public static int EmployeeSelfUpdate = 15;
        public static int EmployeeHrUpdate = 16;
        public static int EmployeeVerified = 17;

        public static int TicketUpdate = 18;
        public static int TicketStarted = 19;
        public static int TicketClosed = 20;
        public static int TicketUpdateOwner = 21;
        public static int TicketAddComment = 22;

        public static int SelfFilledObjective = 23;
        public static int L2FilledObjective = 24;
        public static int HrFilledObjective = 25;

        public static int InitiateResgination = 26;
        public static int RMApprovedResignation = 27;
        public static int RMApprovedResignationToEmp = 28;
        public static int L2ApprovedResignation = 29;
        public static int L2ApprovedResginationToRM = 33;
        public static int RMRejectedResignation = 30;
        public static int RMRejectedResignationToEmp = 31;
        public static int L2RejectedResignation = 32;
        public static int HrApprovedResgination = 34;
        public static int L2RejectedResignationToRM = 35;
        public static int HrApprovedResginationToRM = 36;
        public static int HrApprovedResginationToL2 = 37;
        public static int HrRejectedResginationToRM = 38;
        public static int HrRejectedResginationToL2 = 39;
        public static int HrRejectedResgination = 40;
        public static int ExitProcessingEmp = 41;
        public static int ExitProcessingRM = 42;
        public static int ExitProcessingL2 = 43;
        public static int ClearanceCompletedEmp = 44;
        public static int ClearanceCompletedRM = 45;
        public static int ClearanceCompletedL2 = 46;
        public static int ResignationRevokedToEmp = 47;
        public static int ResignationRevokedToRM = 48;
        public static int ResignationRevokedToL2 = 49;

        public static int EmpTrainingConfirmation = 50;
        public static int RMTrainingConfirmation = 51;
        public static int EmpTrainingCompleted = 52;
    }

    public enum EventLogs
    {
        Login = 1,
        Logout = 2,
        ForgotPassword = 3,

        GetAllAnnouncements,
        GetAnnouncementToShow,
        GetAnnouncementDetails,
        UpdateAnnouncement,
        DeleteAnnouncement,

        GetAllAppraisals,
        GetAppraisalDetails,
        UpdateAppraisal,
        DeleteAppraisal,

        GetCompanySettings,
        UpdateCompanyDetails,
        UpdateHolidaysSettings,
        UpdateDepartments,
        UpdateLocations,
        UpdateTicketCategory,
        UpdateGrades,
        UpdateDesignations,
        UpdateProductLines,
        UpdateTicketFaq,
        UpdateCategories,
        UpdateRegions,
        UpdateTeams,
        UpdateDocumentTypes,
        UpdateAnnouncementTypes,
        UpdateCompany,
        UpdateCompanySettings,
        UpdateAppraisalQuestions,

        GetDashboardAnnouncements,
        GetEmployeeBirthdays,
        GetHolidaysThisYear,

        GetRecentNotifications,
        GetAllNotifications,
        MarkReadNotifications,

        GetTaskDetails,
        GetTaskComments,
        UpdateTask,
        DeleteTask,
        ToggleStartTask,
        ToggleCompleteTask,
        ToggleVerifyTask,
        ToggleIrrelevant,
        DeleteCommentOnTask,
        AddCommentToTask,

        GetAllTickets,
        GetTicketDetails,
        UpdateTicket,
        StartTicket,
        CloseTicket,
        ReopenTicket,
        UndoStartTicket,
        UndoCloseTicket,
        UndoReopenTicket,
        AddComment,
        DeleteComment,

        ChangedPassword,
        CheckIfEmployeeExist,
        GetEmployeesReportingTo,
        GetEmployeeAccount,
        GetEmployeeOrgChart,
        GetEmployeePersonal,
        GetEmployeeCompany,
        GetEmployeeStatutory,
        GetEmployeeDocuments,
        GetEmployeeBanks,
        GetEmployeeCareers,
        GetEmployeeContacts,
        GetEmployeePreviousCompany,
        GetEmployeeFamily,
        GetEmployeeEducation,
        GetEmployeeLanguage,
        GetEmployeeReference,
        GetEmployeeTasks,
        GetEmployeeAssets,
        GetAllEmployees,
        GetEmployeeAppraisalDetails,
        GetEmployeeTickets,
        GetEmployeeAnnouncements,
        GetEmployeeAppraisalsAsManager,
        GetAllAppraisalsPendingWithHr,
        GetEmployeeDataVerification,
        CreateNewEmployee,
        UpdateEmployeeAccount,
        UpdateEmployeePersonal,
        UpdateEmployeeCompany,
        UpdateEmployeeStatutory,
        UpdateEmployeePreviousCompany,
        UpdateEmployeeDocument,
        DeleteEmployeeDocument,
        UpdateEmployeeContact,
        UpdateEmployeeFamily,
        UpdateEmployeeCompensation,
        UpdateEmployeeBank,
        UpdateEmployeeCareer,
        UpdateEmployeeEducation,
        UpdateEmployeeLanguage,
        UpdateEmployeeAssets,
        UpdateEmployeeReference,
        DeleteEmployee,
        ToggleEmployeeLogin,
        SaveAppraisalAnswers,
        SaveAppraisalTrainings,
        SaveAppraisalInternalAnswers,
        SubmitAppraisalAnswers,
        UpdateAppraisalFeedback,
        SaveAppraisalRating,
        VerifyEmployeeDataUpdate,

        SubmitObjective
    }

    public static class EventLogActions
    {
        public static EventLogInfo Login = new EventLogInfo(EventLogs.Login, "{0} ({1}) logged into the application.");
        public static EventLogInfo Logout = new EventLogInfo(EventLogs.Logout, "{0} ({1}) logged out from the application.");
        public static EventLogInfo ForgotPassword = new EventLogInfo(EventLogs.ForgotPassword, "{0} ({1}) requested for reset pasword.");

        public static EventLogInfo GetAllAnnouncements = new EventLogInfo(EventLogs.GetAllAnnouncements, "{0} ({1}) retrieved announcements from the application.");
        public static EventLogInfo GetAnnouncementToShow = new EventLogInfo(EventLogs.GetAnnouncementToShow, "{0} ({1}) retrieved announcements from the application.");
        public static EventLogInfo GetAnnouncementDetails = new EventLogInfo(EventLogs.GetAnnouncementDetails, "{0} ({1}) got the details of the announcement {2}");
        public static EventLogInfo UpdateAnnouncement = new EventLogInfo(EventLogs.UpdateAnnouncement, "{0} ({1}) updated the announcement {2}.");
        public static EventLogInfo DeleteAnnouncement = new EventLogInfo(EventLogs.DeleteAnnouncement, "{0} ({1}) deleted the announcement {2}.");

        public static EventLogInfo GetAllAppraisals = new EventLogInfo(EventLogs.GetAllAppraisals, "{0} ({1}) retrieved all appraisals.");
        public static EventLogInfo GetAppraisalDetails = new EventLogInfo(EventLogs.GetAppraisalDetails, "{0} ({1}) got the details of the appraisal {2}.");
        public static EventLogInfo UpdateAppraisal = new EventLogInfo(EventLogs.UpdateAppraisal, "{0} ({1}) updated the appraisal {2}.");
        public static EventLogInfo DeleteAppraisal = new EventLogInfo(EventLogs.DeleteAppraisal, "{0} ({1}) deleted the appraisal {2}.");

        public static EventLogInfo GetCompanySettings = new EventLogInfo(EventLogs.GetCompanySettings, "{0} ({1}) gor the company settings.");
        public static EventLogInfo UpdateCompanyDetails = new EventLogInfo(EventLogs.UpdateCompanyDetails, "{0} ({1}) updated the company details.");
        public static EventLogInfo UpdateHolidaysSettings = new EventLogInfo(EventLogs.UpdateHolidaysSettings, "{0} ({1}) updated the company holiday settings.");
        public static EventLogInfo UpdateDepartments = new EventLogInfo(EventLogs.UpdateDepartments, "{0} ({1}) updated the company department settings.");
        public static EventLogInfo UpdateLocations = new EventLogInfo(EventLogs.UpdateLocations, "{0} ({1}) updated the company location settings.");
        public static EventLogInfo UpdateTicketCategory = new EventLogInfo(EventLogs.UpdateTicketCategory, "{0} ({1}) updated the company ticket category settings.");
        public static EventLogInfo UpdateGrades = new EventLogInfo(EventLogs.UpdateGrades, "{0} ({1}) updated the company grade settings.");
        public static EventLogInfo UpdateDesignations = new EventLogInfo(EventLogs.UpdateDesignations, "{0} ({1}) updated the company designation settings.");
        public static EventLogInfo UpdateProductLines = new EventLogInfo(EventLogs.UpdateProductLines, "{0} ({1}) updated the company product line settings.");
        public static EventLogInfo UpdateTicketFaq = new EventLogInfo(EventLogs.UpdateTicketFaq, "{0} ({1}) updated the company ticket faq settings.");
        public static EventLogInfo UpdateCategories = new EventLogInfo(EventLogs.UpdateCategories, "{0} ({1}) updated the company category settings.");
        public static EventLogInfo UpdateRegions = new EventLogInfo(EventLogs.UpdateRegions, "{0} ({1}) updated the company region settings.");
        public static EventLogInfo UpdateTeams = new EventLogInfo(EventLogs.UpdateTeams, "{0} ({1}) updated the company team settings.");
        public static EventLogInfo UpdateDocumentTypes = new EventLogInfo(EventLogs.UpdateDocumentTypes, "{0} ({1}) updated the company document type settings.");
        public static EventLogInfo UpdateAnnouncementTypes = new EventLogInfo(EventLogs.UpdateAnnouncementTypes, "{0} ({1}) updated the company announcement type settings.");
        public static EventLogInfo UpdateAssets = new EventLogInfo(EventLogs.UpdateCompany, "{0} ({1}) updated the company asset settings.");
        public static EventLogInfo UpdateAppraisalQuestions = new EventLogInfo(EventLogs.UpdateAppraisalQuestions, "{0} ({1}) updated the appraisal questions.");

        public static EventLogInfo GetDashboardAnnouncements = new EventLogInfo(EventLogs.GetDashboardAnnouncements, "{0} ({1}) got dashboard announcements.");
        public static EventLogInfo GetEmployeeBirthdays = new EventLogInfo(EventLogs.GetEmployeeBirthdays, "{0} ({1}) got employee birthdays.");
        public static EventLogInfo GetHolidaysThisYear = new EventLogInfo(EventLogs.GetHolidaysThisYear, "{0} ({1}) got holidays applicable.");

        public static EventLogInfo GetRecentNotifications = new EventLogInfo(EventLogs.GetRecentNotifications, "{0} ({1}) got recent notification.");
        public static EventLogInfo GetAllNotifications = new EventLogInfo(EventLogs.GetAllNotifications, "{0} ({1}) got all notifications.");
        public static EventLogInfo MarkReadNotifications = new EventLogInfo(EventLogs.MarkReadNotifications, "{0} ({1}) marked notifications as read.");

        public static EventLogInfo GetTaskDetails = new EventLogInfo(EventLogs.GetTaskDetails, "{0} ({1}) got details of the task {2}.");
        public static EventLogInfo GetTaskComments = new EventLogInfo(EventLogs.GetTaskComments, "{0} ({1}) got the comments for the task {2}.");
        public static EventLogInfo UpdateTask = new EventLogInfo(EventLogs.UpdateTask, "{0} ({1}) updated the task {2}.");
        public static EventLogInfo DeleteTask = new EventLogInfo(EventLogs.DeleteTask, "{0} ({1}) deleted the task {2}.");
        public static EventLogInfo ToggleStartTask = new EventLogInfo(EventLogs.ToggleStartTask, "{0} ({1}) started/ undo started the task {2}.");
        public static EventLogInfo ToggleCompleteTask = new EventLogInfo(EventLogs.ToggleCompleteTask, "{0} ({1}) completed/ undo completed the task {2}.");
        public static EventLogInfo ToggleVerifyTask = new EventLogInfo(EventLogs.ToggleVerifyTask, "{0} ({1}) verified/ undo verified the task {2}.");
        public static EventLogInfo ToggleIrrelevant = new EventLogInfo(EventLogs.ToggleIrrelevant, "{0} ({1}) logged into the application.");
        public static EventLogInfo DeleteCommentOnTask = new EventLogInfo(EventLogs.DeleteCommentOnTask, "{0} ({1}) deleted a comment in the task {2}.");
        public static EventLogInfo AddCommentToTask = new EventLogInfo(EventLogs.AddCommentToTask, "{0} ({1}) added a comment to the task {2}.");

        public static EventLogInfo GetAllTickets = new EventLogInfo(EventLogs.GetAllTickets, "{0} ({1}) retrieved all tickets.");
        public static EventLogInfo GetTicketDetails = new EventLogInfo(EventLogs.GetTicketDetails, "{0} ({1}) got the details of the ticket {2}.");
        public static EventLogInfo UpdateTicket = new EventLogInfo(EventLogs.UpdateTicket, "{0} ({1}) updated the ticket {2}.");
        public static EventLogInfo StartTicket = new EventLogInfo(EventLogs.StartTicket, "{0} ({1}) started the ticket {2}.");
        public static EventLogInfo CloseTicket = new EventLogInfo(EventLogs.CloseTicket, "{0} ({1}) closed the ticket {2}.");
        public static EventLogInfo ReopenTicket = new EventLogInfo(EventLogs.ReopenTicket, "{0} ({1}) reopened the ticket {2}.");
        public static EventLogInfo UndoStartTicket = new EventLogInfo(EventLogs.UndoStartTicket, "{0} ({1}) undo start the ticket {2}.");
        public static EventLogInfo UndoCloseTicket = new EventLogInfo(EventLogs.UndoCloseTicket, "{0} ({1}) undo close the ticket {2}.");
        public static EventLogInfo UndoReopenTicket = new EventLogInfo(EventLogs.UndoReopenTicket, "{0} ({1}) undo reopen the ticket {2}.");
        public static EventLogInfo AddCommentToTicket = new EventLogInfo(EventLogs.AddComment, "{0} ({1}) added a comment in the ticket {2}.");
        public static EventLogInfo DeleteCommentToTicket = new EventLogInfo(EventLogs.DeleteComment, "{0} ({1}) deleted a comment in the ticket {2}.");

        public static EventLogInfo ChangedPassword = new EventLogInfo(EventLogs.ChangedPassword, "{0} ({1}) changed the application password.");
        public static EventLogInfo CheckIfEmployeeExist = new EventLogInfo(EventLogs.CheckIfEmployeeExist, "{0} ({1}) checked if the employee exist {2}.");
        public static EventLogInfo GetEmployeesReportingTo = new EventLogInfo(EventLogs.GetEmployeesReportingTo, "{0} ({1}) logged into the application.");
        public static EventLogInfo GetEmployeeAccount = new EventLogInfo(EventLogs.GetEmployeeAccount, "{0} ({1}) got employee account details for {2} ({3}).");
        public static EventLogInfo GetEmployeeOrgChart = new EventLogInfo(EventLogs.GetEmployeeOrgChart, "{0} ({1}) got employee organization chart details for {2} ({3}).");
        public static EventLogInfo GetEmployeePersonal = new EventLogInfo(EventLogs.GetEmployeePersonal, "{0} ({1}) got employee personal details for {2} ({3}).");
        public static EventLogInfo GetEmployeeCompany = new EventLogInfo(EventLogs.GetEmployeeCompany, "{0} ({1}) got employee company details for {2} ({3}).");
        public static EventLogInfo GetEmployeeStatutory = new EventLogInfo(EventLogs.GetEmployeeStatutory, "{0} ({1}) got employee statutory details for {2} ({3}).");
        public static EventLogInfo GetEmployeeDocuments = new EventLogInfo(EventLogs.GetEmployeeDocuments, "{0} ({1}) got employee documents details for {2} ({3}).");
        public static EventLogInfo GetEmployeeBanks = new EventLogInfo(EventLogs.GetEmployeeBanks, "{0} ({1}) got employee banks details for {2} ({3}).");
        public static EventLogInfo GetEmployeeCareers = new EventLogInfo(EventLogs.GetEmployeeCareers, "{0} ({1}) got employee Careers details for {2} ({3}).");
        public static EventLogInfo GetEmployeeContacts = new EventLogInfo(EventLogs.GetEmployeeContacts, "{0} ({1}) got employee contacts details for {2} ({3}).");
        public static EventLogInfo GetEmployeePreviousCompany = new EventLogInfo(EventLogs.GetEmployeePreviousCompany, "{0} ({1}) got employee previous company details for {2} ({3}).");
        public static EventLogInfo GetEmployeeFamily = new EventLogInfo(EventLogs.GetEmployeeFamily, "{0} ({1}) got employee family details for {2} ({3}).");
        public static EventLogInfo GetEmployeeEducation = new EventLogInfo(EventLogs.GetEmployeeEducation, "{0} ({1}) got employee education details for {2} ({3}).");
        public static EventLogInfo GetEmployeeLanguage = new EventLogInfo(EventLogs.GetEmployeeLanguage, "{0} ({1}) got employee language details for {2} ({3}).");
        public static EventLogInfo GetEmployeeReference = new EventLogInfo(EventLogs.GetEmployeeReference, "{0} ({1}) got employee reference details for {2} ({3}).");
        public static EventLogInfo GetEmployeeTasks = new EventLogInfo(EventLogs.GetEmployeeTasks, "{0} ({1}) got employee task details for {2} ({3}).");
        public static EventLogInfo GetEmployeeAssets = new EventLogInfo(EventLogs.GetEmployeeAssets, "{0} ({1}) got employee account assets for {2} ({3}).");
        public static EventLogInfo GetAllEmployees = new EventLogInfo(EventLogs.GetAllEmployees, "{0} ({1}) got all employees list.");
        public static EventLogInfo GetEmployeeAppraisalDetails = new EventLogInfo(EventLogs.GetEmployeeAppraisalDetails, "{0} ({1}) got employee appraisal details for {2} ({3}).");
        public static EventLogInfo GetEmployeeTickets = new EventLogInfo(EventLogs.GetEmployeeTickets, "{0} ({1}) got employee ticket details for {2} ({3}).");
        public static EventLogInfo GetEmployeeAnnouncements = new EventLogInfo(EventLogs.GetEmployeeAnnouncements, "{0} ({1}) got employee announcements for {2} ({3}).");
        public static EventLogInfo GetEmployeeAppraisalsAsManager = new EventLogInfo(EventLogs.GetEmployeeAppraisalsAsManager, "{0} ({1}) got appraisals as manager.");
        public static EventLogInfo GetAllAppraisalsPendingWithHr = new EventLogInfo(EventLogs.GetAllAppraisalsPendingWithHr, "{0} ({1}) got appraisals pending with HR.");
        public static EventLogInfo GetEmployeeDataVerification = new EventLogInfo(EventLogs.GetEmployeeDataVerification, "{0} ({1}) get employee verification details for {2} ({3}).");
        public static EventLogInfo CreateNewEmployee = new EventLogInfo(EventLogs.CreateNewEmployee, "{0} ({1}) created a new employee {2} ({3}).");
        public static EventLogInfo UpdateEmployeeAccount = new EventLogInfo(EventLogs.UpdateEmployeeAccount, "{0} ({1}) update employee account details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeePersonal = new EventLogInfo(EventLogs.UpdateEmployeePersonal, "{0} ({1}) update employee personal details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeCompany = new EventLogInfo(EventLogs.UpdateEmployeeCompany, "{0} ({1}) update employee company details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeStatutory = new EventLogInfo(EventLogs.UpdateEmployeeStatutory, "{0} ({1}) update employee statutory details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeePreviousCompany = new EventLogInfo(EventLogs.UpdateEmployeePreviousCompany, "{0} ({1}) update employee previous company details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeDocument = new EventLogInfo(EventLogs.UpdateEmployeeDocument, "{0} ({1}) update employee document details for {2} ({3}).");
        public static EventLogInfo DeleteEmployeeDocument = new EventLogInfo(EventLogs.DeleteEmployeeDocument, "{0} ({1}) deleted employee document details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeContact = new EventLogInfo(EventLogs.UpdateEmployeeContact, "{0} ({1}) update employee contact details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeFamily = new EventLogInfo(EventLogs.UpdateEmployeeFamily, "{0} ({1}) update employee family details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeCompensation = new EventLogInfo(EventLogs.UpdateEmployeeCompensation, "{0} ({1}) update employee compensation details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeBank = new EventLogInfo(EventLogs.UpdateEmployeeBank, "{0} ({1}) update employee bank details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeCareer = new EventLogInfo(EventLogs.UpdateEmployeeCareer, "{0} ({1}) update employee Career details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeEducation = new EventLogInfo(EventLogs.UpdateEmployeeEducation, "{0} ({1}) update employee education details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeLanguage = new EventLogInfo(EventLogs.UpdateEmployeeLanguage, "{0} ({1}) update employee language details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeAssets = new EventLogInfo(EventLogs.UpdateEmployeeAssets, "{0} ({1}) update employee assets details for {2} ({3}).");
        public static EventLogInfo UpdateEmployeeReference = new EventLogInfo(EventLogs.UpdateEmployeeReference, "{0} ({1}) update employee reference details for {2} ({3}).");
        public static EventLogInfo DeleteEmployee = new EventLogInfo(EventLogs.DeleteEmployee, "{0} ({1}) deleted the employee {2} ({3}).");
        public static EventLogInfo ToggleEmployeeLogin = new EventLogInfo(EventLogs.ToggleEmployeeLogin, "{0} ({1}) toggled employee login for {2} ({3}).");
        public static EventLogInfo SaveAppraisalAnswers = new EventLogInfo(EventLogs.SaveAppraisalAnswers, "{0} ({1}) saved appraisal answers for {2} ({3}).");
        public static EventLogInfo SaveAppraisalTrainings = new EventLogInfo(EventLogs.SaveAppraisalTrainings, "{0} ({1}) saved appraisal trainings for {2} ({3}).");
        public static EventLogInfo SaveAppraisalInternalAnswers = new EventLogInfo(EventLogs.SaveAppraisalInternalAnswers, "{0} ({1}) save appraisal internal answers for {2} ({3}).");
        public static EventLogInfo SubmitAppraisalAnswers = new EventLogInfo(EventLogs.SubmitAppraisalAnswers, "{0} ({1}) submit appraisal answers for {2} ({3}).");
        public static EventLogInfo UpdateAppraisalFeedback = new EventLogInfo(EventLogs.UpdateAppraisalFeedback, "{0} ({1}) update appraisal feedback for {2} ({3}).");
        public static EventLogInfo SaveAppraisalRating = new EventLogInfo(EventLogs.SaveAppraisalRating, "{0} ({1}) saved appraisal rating for {2} ({3}).");
        public static EventLogInfo VerifyEmployeeDataUpdate = new EventLogInfo(EventLogs.VerifyEmployeeDataUpdate, "{0} ({1}) verified employee section for {2} ({3}) - {4}.");

        public static EventLogInfo SubmitObjective = new EventLogInfo(EventLogs.SubmitObjective, "{0} ({1}) submit appraisal answers for {2} ({3}).");
    }

    public class EventLogInfo
    {
        public EventLogInfo(EventLogs action, string template)
        {
            this.ActionId = action;
            this.Template = template;
        }
        public string Template { get; set; }
        public EventLogs ActionId { get; set; }
    }
}
