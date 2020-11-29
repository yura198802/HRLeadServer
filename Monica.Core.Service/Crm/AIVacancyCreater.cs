using Microsoft.EntityFrameworkCore;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monica.Core.Service.Crm
{
    public class AIVacancyCreater
    {
        private HrDbContext _dbContext;

        public AIVacancyCreater(HrDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Vacancy> GetVacancy(Request request)
        {
            var allRequirements = await _dbContext.Requirements.ToListAsync();
            var comparer = new FuzzyComparer();
            var vacancyResult = new Vacancy
            {
                Name = request.Name,
                Requirements = new List<Requirement>() 
            };
            var acceptedRequirements = new List<Requirement>();

            foreach (var req in request.RequestRequirements)
            {
                var requirementWeights = new List<(Requirement requirement, double weight)>();

                allRequirements.ForEach(r => requirementWeights.Add((r, comparer.CalculateFuzzyEqualValue(TextPrepare(req.Text), TextPrepare(r.Text)))));

                var maxWeight = requirementWeights.Max(r => r.weight);
                if (maxWeight > 0.17)
                {
                    var requirement = requirementWeights.First(r => r.weight == maxWeight).requirement;
                    vacancyResult.Requirements.Add(requirement);
                    acceptedRequirements.Add(requirement);
                }
                else
                {
                    vacancyResult.Requirements.Add(new Requirement() { Text = req.Text });
                }
            }

            var allVacancies = await _dbContext.Vacancies
                    .Include(v => v.CategoryVacancy)
                    .Include(v => v.Requirements)
                    .Include(v => v.Works)
                    .Include(v => v.Offers)
                    .ToListAsync();

            Vacancy vacancy = null;

            var vacancies = GetVacanciesByName(allVacancies, request.Name);
            if (vacancies.Any())
            {
                if (acceptedRequirements.Any())
                {
                    vacancy = GetVacancyByRequirements(vacancies.Select(v => v.vacancy).ToList(), acceptedRequirements);
                }
                if (vacancy == null)
                {
                    var maxWeight = vacancies.Max(v => v.weight);
                    vacancy = vacancies.First(v => v.weight == maxWeight).vacancy;
                }
            }
            else
            {
                if (acceptedRequirements.Any())
                {
                    vacancy = GetVacancyByRequirements(allVacancies, acceptedRequirements);
                }
            }

            if (vacancy != null)
            {
                vacancyResult.CategoryVacancy = vacancy.CategoryVacancy;
                vacancyResult.Name = vacancy.Name;
                vacancyResult.Description = vacancy.Description;
                vacancyResult.Works = vacancy.Works;
                vacancyResult.Offers = vacancy.Offers;
            }

            _dbContext.Add(vacancyResult);
            await _dbContext.SaveChangesAsync();
            return vacancyResult;
        }

        private List<(Vacancy vacancy, double weight)> GetVacanciesByName(List<Vacancy> vacancies, string name)
        {
            var comparer = new FuzzyComparer();
            var vacancyWeight = new List<(Vacancy vacancy, double weight)>();
            vacancies.ForEach(v => vacancyWeight.Add((v, comparer.CalculateFuzzyEqualValue(TextPrepare(v.Name), TextPrepare(name)))));

            return vacancyWeight.Where(v => v.weight > 0.4).ToList();
        }

        private Vacancy GetVacancyByRequirements(List<Vacancy> vacancies, List<Requirement> requirements)
        {
            var vacancyReqCount = new List<(Vacancy vacancy, double count)>();
            vacancies.ForEach(v => vacancyReqCount.Add(GetVacancyWeight(v, requirements)));

            var maxCount = vacancyReqCount.Max(v => v.count);
            if (maxCount > 0)
            {
                return vacancyReqCount.First(v => v.count == maxCount).vacancy;
            }

            return null;
        }

        private (Vacancy vacancy, double count) GetVacancyWeight(Vacancy vacancy, List<Requirement> requirements)
        {
            int count = 0;

            foreach (var vacReq in vacancy.Requirements)
            {
                foreach (var req in requirements)
                {
                    if (vacReq.Id == req.Id)
                    {
                        count++;
                        break;
                    }
                }
            }

            return (vacancy, count);
        }

        private string TextPrepare(string text)
        {
            return text.ToLower()
                .Replace("опыт", "")
                .Replace("работы", "")
                .Replace("имеет", "")
                .Replace("знает", "")
                .Replace("обладает", "")
                .Replace("использует", "");
        }
    }
}
