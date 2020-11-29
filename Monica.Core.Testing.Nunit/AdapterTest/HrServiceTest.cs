using Microsoft.EntityFrameworkCore;
using Module.Testing.Nunit;
using Monica.Core.Abstraction.Crm;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monica.Core.Testing.Nunit.AdapterTest
{
    public class HrServiceTest : BaseServiceTest<IHrService>
    {

        [Test]
        public async Task FuzzyTest()
        {
            try
            {
                await Service.FuzzyTest();
            }
            catch (Exception ex)
            {

            }
        }

        [Test]
        public async Task AutomaticVacancyTest()
        {
            var dbContext = AutoFac.Resolve<HrDbContext>();
            try
            {
                var request = dbContext.Requests.Include(r => r.RequestRequirements).First(r=>r.Id==2);
                var vacancy = await Service.GetAutomaticVacancy(request);
            }
            catch (Exception ex)
            {

            }
        }

        [Test]
        public async Task MLTest()
        {
            try
            {
                await Service.MLTest();
            }
            catch (Exception ex)
            {

            }
        }

        [Test]
        public async Task GetVacanciesDiagram()
        {
            try
            {
                await Service.GetSoftSkills(10364);
                var r = await Service.GetVacansiesDiagram(10364);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
