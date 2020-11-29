using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto;

namespace Monica.Core.Abstraction.Crm.Settings
{
    public interface ITypeLevelAdapter
    {
        /// <summary>
        /// Получить список типов уровней орг (для отображения вместо цифр строки на web-интерфейсе)
        /// </summary>
        /// <returns></returns>
        Task<List<TypeLevelDto>> GetListTypeLevels();
        /// <summary>
        /// Получить строковое отображения для уровня орг по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetTypeLevelById(int id);
        /// <summary>
        /// Получить уровень орг по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TypeLevels GetTypeLevelEnumById(int id);
    }
}
