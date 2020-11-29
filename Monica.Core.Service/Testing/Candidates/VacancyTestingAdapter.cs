using System;
using System.Collections.Generic;
using System.Linq;
using Monica.Core.Abstraction.Testing.Vacancy;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.ModelParametrs.ModelsArgs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.HR;
using Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates;
using Monica.Core.DbModel.ModelDto.HR.Testing.Candidates;

namespace Monica.Core.Service.Testing.Candidates
{
    public class VacancyTestingAdapter : IVacancyTestingAdapter
    {
        private HrDbContext _hrDbContext;

        public VacancyTestingAdapter( HrDbContext hrDbContext)
        {
            _hrDbContext = hrDbContext;
        }

        public async Task<VacancyTestingDto> GetQuestionsAsync(int candidateId)
        {
            var questionsByTags = new List<QuestionCrm>();
            try
            {
                //if (IsYetTesting(candidateId))
                //    throw new Exception("Пользователь уже проходил тестирование!");
                var candidate = await _hrDbContext.Resumes.FirstOrDefaultAsync(x => x.Id == candidateId);
                if (string.IsNullOrWhiteSpace(candidate.Skills))
                    throw new Exception("Отсутствуют теги скилов для кандидата!");
                var cadidateTags = candidate.Skills.Split(',');
                var questions = await _hrDbContext.Questions.Include(x=>x.Answers).Include(x=>x.Type).ToListAsync();
                foreach (var tag in cadidateTags)
                {
                    foreach (var question in questions)
                    {
                        if (question?.Tags?.ToLower()?.Split(',')?.Contains(tag.ToLower()) ?? false)
                        {
                            if(questionsByTags.Select(s=>s.Id).Contains(question.Id))
                                continue;
                            questionsByTags.Add(question);
                        }
                            
                    }
                }
                var dtos = GetQuestionDtosByCrms(questionsByTags);
                return new VacancyTestingDto()
                {
                    CandidateId = candidateId,
                    Questions = dtos
                };
            }
            catch (Exception e)
            {
                return new VacancyTestingDto()
                    {CandidateId = candidateId, Questions = new List<VacancyTestingQuestionDto>()};
            }
        }
        public async Task<ResultCrmDb> SaveResultsAsync(VacancyTestingInArgs args)
        {
            var result = new ResultCrmDb();
            try
            {
                List< ResultTestingCrm > resultCrm = new List<ResultTestingCrm>();
                var candidate = await _hrDbContext.Resumes.FirstOrDefaultAsync(r => r.Id == args.CandidateId);
                foreach (var dto in args.Results)
                {
                    resultCrm.Add(await GetResultTestingCrm(dto, candidate));
                }

                await _hrDbContext.ResultsTesting.AddRangeAsync(resultCrm);
                await _hrDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("",e.Message);
            }
            return result;
        }
        private bool IsYetTesting(int candidateId)
        {
            try
            {
                if (_hrDbContext.ResultsTesting.Any(x => x.User.Id == candidateId))
                    return true;
            }
            catch (Exception e)
            {
            }

            return false;
        }
        private async Task<ResultTestingCrm> GetResultTestingCrm(ResultTestingDto dto, Resume candidate)
        {
            var questions = await _hrDbContext.Questions.Include(x=>x.Answers).Include(x=>x.Type).ToListAsync();
            return new ResultTestingCrm()
            {
                Question = questions.FirstOrDefault(q => q.Id == dto.QuestionId),
                User = candidate,
                Value = dto.Value,
                Time = dto.Time
            };
        }
        private List<VacancyTestingQuestionDto> GetQuestionDtosByCrms(List<QuestionCrm> crms)
        {
            List<VacancyTestingQuestionDto> dtos = new List<VacancyTestingQuestionDto>();
            foreach (var crm in crms)
            {
                dtos.Add(GetQuestionDto(crm));
            }
            return dtos;
        }
        private VacancyTestingQuestionDto GetQuestionDto(QuestionCrm crm)
        {
            List<VacancyTestingAnswerDto> answers = new List<VacancyTestingAnswerDto>();
            foreach (var crmAnswer in crm.Answers)
            {
                answers.Add(GetAnswerDtoDto(crmAnswer));
            }
            return new VacancyTestingQuestionDto()
            {
                Id = crm.Id,
                Question = crm.Question,
                Description = crm.Description,
                Type = crm.Type.Type,
                Tags = crm.Tags.Split(',').ToList(),
                Answers = answers
            };
        }
        private VacancyTestingAnswerDto GetAnswerDtoDto(TestingAnswerCrm crm)
        {
            return new VacancyTestingAnswerDto()
            {
                Val = crm.Num,
                Value = crm.Value
            };
        }
    }
}