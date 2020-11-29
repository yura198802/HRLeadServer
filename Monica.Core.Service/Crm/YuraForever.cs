using Dapper;
using Microsoft.ML.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Monica.Core.Service.Crm
{
    public class YuraForever
    {
        public List<ClassificerData> ClassifierData { get; set; }

        public void InitData()
        {
            using (var connection = new MySqlConnection("Server=84.201.188.15;Port=3306;Database=parse;User Id=develop;Password=$zXcvbnm512$;TreatTinyAsBoolean=true;charset=utf8;"))
            {
                var candidates = connection.Query<Resume>("SELECT * FROM candidates c");
                Console.WriteLine(candidates.FirstOrDefault());

                var words = connection.Query<Word>("SELECT * FROM word c");
                Console.WriteLine(words.FirstOrDefault());

                using (var connection1 = new MySqlConnection("Server = 84.201.188.15; Port = 3306; Database = hackatonfinal_gpb; User Id = develop; Password =$zXcvbnm512$; TreatTinyAsBoolean = true; charset = utf8; "))
                {
                    var vacancyes = connection1.Query<YuraVacancy>("SELECT c.Name AS Category, v.Name, v.NameEn, v.NameRus FROM vacancies v  JOIN categoryvacancies c ON v.CategoryVacancyId = c.Id ORDER BY v.id");
                    Console.WriteLine(vacancyes.FirstOrDefault());


                    List<ClassificerData> classificerDatas = new List<ClassificerData>();

                    foreach (var discrpt in vacancyes)
                    {
                        foreach (var candidate in candidates)
                        {
                            if (discrpt == null || discrpt.NameRus == null) continue;
                            string[] mathesStrRus = discrpt.NameRus.Split(new[] { '/', ' ', '(', ')', ',', '*', ':', '.', ';', '-' });
                            string[] mathesStrEn = discrpt.NameEn.Split(new[] { '/', ' ', '(', ')', ',', '*', ':', '.', ';', '-' });

                            string patternRus = string.Join("|", mathesStrRus.Select(w => Regex.Escape(w.ToUpper())).Where(f => f != ""));
                            string patternEn = string.Join("|", mathesStrEn.Select(w => Regex.Escape(w.ToUpper())).Where(f => f != ""));

                            Regex regexRus = new Regex(patternRus);
                            Regex regexEn = new Regex(patternEn);

                            var resultRus = regexRus.Matches(candidate.DesiredPosition.ToUpper());
                            var resultEn = regexEn.Matches(candidate.DesiredPosition.ToUpper());

                            if (resultRus.Count >= 2 && resultRus[0].Value != resultRus[1].Value)
                            {
                                var clDt = new ClassificerData();
                                foreach (var resRus in resultRus)
                                {

                                    if (resRus.ToString() != "")
                                    {

                                        clDt.Category += resRus.ToString() + " ";
                                        clDt.Skills = candidate.Skills;


                                    }
                                }
                                classificerDatas.Add(clDt);
                            }
                            if (resultEn.Count >= 2 && resultEn[0].Value != resultEn[1].Value)
                            {
                                var clDt = new ClassificerData();
                                foreach (var resEn in resultEn)
                                {
                                    if (resEn.ToString() != "")
                                    {

                                        clDt.Category += resEn.ToString() + " ";
                                        clDt.Skills = candidate.Skills;

                                    }
                                }
                                classificerDatas.Add(clDt);
                            }


                        }
                    }
                    Console.WriteLine(classificerDatas.Count);
                    var disttinctArr = classificerDatas.Select(f => f.Category).Distinct();
                    Console.WriteLine(disttinctArr.Count().ToString());
                    ClassifierData = classificerDatas;
                }



            }

        }
    }

    public class ClassificerData
    {
        [ColumnName("Label")]
        public string Category { get; set; }

        [ColumnName("Skills")]
        public string Skills { get; set; }

        [ColumnName("Veсtor")]
        public string Veсtor { get; set; }//Результирующий вектор
    }

    public class YuraVacancy
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string NameRus { get; set; }
        public string NameEn { get; set; }
    }

    public class Word
    {
        public int Sysid { get; set; }
        public string Name { get; set; }
        public bool IsFirst { get; set; }
        public string Vector { get; set; }
        public List<double> VectorDouble { get; set; }
    }

    public class Resume
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string DesiredPosition { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public string Skills { get; set; }
        public string About { get; set; }
    }
}
