using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.DbModel.ModelCrm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.DbModel.ModelCrm.HR;
using System.Linq;
using Microsoft.ML;
using System.IO;
using Microsoft.ML.Data;

namespace Monica.Core.Service.Crm
{
    public class HrService : IHrService
    {
        private static string _appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        private static string _trainDataPath => Path.Combine(_appPath, "..", "..", "..", "Data", "issues_train.tsv");
        private static string _testDataPath => Path.Combine(_appPath, "..", "..", "..", "Data", "issues_test.tsv");
        private static string _modelPath => Path.Combine(_appPath, "..", "..", "..", "Models", "model.zip");

        private static MLContext _mlContext;
        private static PredictionEngine<ClassificerData, ClassifierResult> _predEngine;
        private static ITransformer _trainedModel;
        static IDataView _trainingDataView;

        private HrDbContext _dbContext;

        public HrService(HrDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task FuzzyTest()
        {
            var comparer = new FuzzyComparer();
            var s = comparer.CalculateFuzzyEqualValue("Владеет опытом промышленной разработки на Go от двух лет, профилирования и отладки Go кода, написания сетевых многопоточных приложений",
                "разработка go, многопоточные протоколы");
            return Task.CompletedTask;
        }

        public async Task<Vacancy> GetAutomaticVacancy(Request request)
        {
            var aiVacancyCreater = new AIVacancyCreater(_dbContext);
            return await aiVacancyCreater.GetVacancy(request);
        }

        public async Task MLTest()
        {
            try
            {
                //var request = new Request { Name = "Разработчик Go", Department = "Отдел аналитической разработки" };
                //request.RequestRequirements = new List<RequestRequirement>();
                //request.RequestRequirements.Add(new RequestRequirement { Text = "разработка go, многопоточные протоколы" });
                //await _dbContext.Requests.AddAsync(request);
                //await _dbContext.SaveChangesAsync();
                var yura = new YuraForever();
                yura.InitData();

                _mlContext = new MLContext(seed: 0);

                var classifierData = ClearData(yura.ClassifierData);
                var fr = classifierData.Where(c => c.Category == "FRONT END ").Take(100);
                var other = classifierData.Where(c => c.Category != "FRONT END ").ToList();
                other.AddRange(fr);

                _trainingDataView = _mlContext.Data.LoadFromEnumerable(other);

                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label")
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Skills", outputColumnName: "SkillsFeaturized"))
                    .AppendCacheCheckpoint(_mlContext);

                var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "SkillsFeaturized"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                _trainedModel = trainingPipeline.Fit(_trainingDataView);

                _mlContext.Model.Save(_trainedModel, _trainingDataView.Schema, "model.zip");

                _predEngine = _mlContext.Model.CreatePredictionEngine<ClassificerData, ClassifierResult>(_trainedModel);

                var prediction = _predEngine.Predict(new ClassificerData { Skills = "Golang" });

                //var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));

                var labelBuffer = new VBuffer<ReadOnlyMemory<char>>();
                _predEngine.OutputSchema["Score"].Annotations.GetValue("SlotNames", ref labelBuffer);
                var labels = labelBuffer.DenseValues().Select(l => l.ToString()).ToArray();

                var scores = labels.ToDictionary(
                    l => l,
                    l => (decimal)prediction.Distances[Array.IndexOf(labels, l)])
                    .OrderByDescending(kv => kv.Value);

            }
            catch (Exception ex)
            {

            }
        } 

        private List<ClassificerData> ClearData(List<ClassificerData> inputData)
        {
            var dict = new[] { "GO РАЗРАБОТЧИК ", "РАЗРАБОТЧИК GO ", "DEVELOPER GO ", "GO РАЗРАБОТЧИК ",
            "DEVELOPER GO DEVELOPER", "DATA ENGINEER ", "DATA ENGINEER DATA ", "DATA ENGINEER DATA ENGINEER ",
            "ENGINEER DATA ", "DATA SCIENTIST ", "DATA SCIENTIST DATA ", "FRONT END ", "FRONT END DEVELOPER ",
            "DATA ENGINEER DATA ENGINEER ", "OF FRONT END OF ", "MIDDLE FRONT END ", "РАЗРАБОТЧИК FRONT END РАЗРАБОТЧИК ",
            "MIDDLE FRONT END DEVELOPER ", "DATA ENGINEER DATA ENGINEER ", "FRONT END РАЗРАБОТЧИК END ", "FRONT END РАЗРАБОТЧИК РАЗРАБОТЧИК РАЗРАБОТЧИК "};

            return inputData.Where(i => dict.Contains(i.Category)).ToList();
        }

        public async Task<List<DiagramData>> GetVacansiesDiagram(int resumeId)
        {
            var result = new List<DiagramData>();
            var resume = await _dbContext.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId);
            if (resume == null) return result;

            var mlContext = new MLContext(seed: 0);

            DataViewSchema modelSchema;

            ITransformer trainedModel = mlContext.Model.Load("model.zip", out modelSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<ClassificerData, ClassifierResult>(trainedModel);

            var prediction = predEngine.Predict(new ClassificerData { Skills = resume.Skills });

            var labelBuffer = new VBuffer<ReadOnlyMemory<char>>();
            predEngine.OutputSchema["Score"].Annotations.GetValue("SlotNames", ref labelBuffer);
            var labels = labelBuffer.DenseValues().Select(l => l.ToString()).ToArray();

            var scores = labels.ToDictionary(
                l => l,
                l => prediction.Distances[Array.IndexOf(labels, l)])
                .OrderByDescending(kv => kv.Value);

            return scores.Select(s => new DiagramData { Arg=s.Key, Val = s.Value }).Take(4)
                .OrderBy(o => o.Val).ToList();
        }

        public async Task<List<DiagramData>> GetSoftSkills(int resumeId)
        {
            var result = new List<DiagramData>();
            var resume = await _dbContext.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId);
            if (resume == null) return result;

            result.Add(new DiagramData { Arg = "Опыт работы", Val = resume.Experience });

            var comparer = new FuzzyComparer();

            var file = File.ReadAllText("University.txt", Encoding.ASCII);
            var lines = file.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None).ToList();
            var universities = new List<(string name, int rate)>();
            lines.ForEach(l => universities.Add((l.Split(".")[1].Trim(), Convert.ToInt32(l.Split(".")[0]))));

            var universityWeights = new List<(string name, double weight)>();
            universities.ForEach(r => universityWeights.Add((r.name, comparer.CalculateFuzzyEqualValue(r.name, resume.Education))));
            var maxWeight = universityWeights.Max(r => r.weight);
            if (maxWeight > 0)
            {
                var universityName = universityWeights.First(r => r.weight == maxWeight).name;
                result.Add(new DiagramData { Arg = "Рейтинг университета", Val = universities.First(u => u.name == universityName).rate });
            }

            return result;
        }
    }

    public class ClassifierResult
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }

        [ColumnName("Score")]
        public float[] Distances;
    }
}
