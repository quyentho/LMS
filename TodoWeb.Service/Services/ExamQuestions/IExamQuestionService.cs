using TodoWeb.Application.Dtos.ExamQuestionModel;

namespace TodoWeb.Service.Services.ExamQuestions
{
    public interface IExamQuestionService
    {
        public IEnumerable<ExamQuestionViewModel> GetExamQuestions(int? ExamQuestionId, int? ExamId, int? QuestionId);
        public int CreateExamQuestion(ExamQuestionCreateModel newExamQuestion);
        public int UpdateExamQuestion(ExamQuestionUpdateModel updateExamQuestion);
        public int DeleteExamQuestion(int examQuestionId);

    }
}
