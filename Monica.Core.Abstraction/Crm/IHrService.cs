using Monica.Core.DbModel.ModelCrm.HR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Monica.Core.Abstraction.Crm
{
    public interface IHrService
    {
        Task FuzzyTest();

        Task MLTest();

        Task<Vacancy> GetAutomaticVacancy(Request request);

        Task<List<DiagramData>> GetVacansiesDiagram(int resumeId);

        Task<List<DiagramData>> GetSoftSkills(int resumeId);
    }

    public class DiagramData
    {
        public string Arg { get; set; }
        public string ValPerc => $"{(Val * 100).ToString("n2")}%";

        public double Val { get; set; }
    }
}
