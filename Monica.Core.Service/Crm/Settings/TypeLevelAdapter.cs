using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto;

namespace Monica.Core.Service.Crm.Settings
{
    public class TypeLevelAdapter : ITypeLevelAdapter
    {
        /// <summary>
        /// Словарь для сопоставления перечисления уровня орг cо строкой
        /// </summary>
        public static Dictionary<TypeLevels, string> TypeLevelsDic = new Dictionary<TypeLevels, string>()
        {
            {TypeLevels.Ufk ,"Управление Федерального казначейства" },
            {TypeLevels.Mf,"Минестерство финансов" },
            {TypeLevels.Fo,"Финансовый орган" },
            {TypeLevels.GAdm,"Главный Администратор доходов" },
            {TypeLevels.Adm,"Администратор доходов" } };
        public TypeLevelAdapter() { }
        public async Task<List<TypeLevelDto>> GetListTypeLevels()
        {
            var result = new List<TypeLevelDto>();
            foreach (var level in TypeLevelsDic)
            {
                result.Add(new TypeLevelDto() { Id = (int)level.Key, Caption = level.Value });
            }
            return result;
        }
        public string GetTypeLevelById(int id)
        {
            return TypeLevelsDic[(TypeLevels)id];
        }
        public TypeLevels GetTypeLevelEnumById(int id)
        {
            return (TypeLevels)id;
        }
    }

}
