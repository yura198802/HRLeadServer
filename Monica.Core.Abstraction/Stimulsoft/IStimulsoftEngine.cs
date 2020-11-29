using System.Threading.Tasks;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Abstraction.Stimulsoft
{
    /// <summary>
    /// Движок для получения данных для стимулсофта
    /// </summary>
    public interface IStimulsoftEngine
    {
        Task<StimulSoftResult> ActionResultData(CommandJson commandJson, string userName);

        /// <summary>
        /// Получить содержимое файла отчета
        /// </summary>
        /// <param name="fileName">Имя файла, который нужно получить</param>
        /// <returns></returns>
        Task<string> GetFileMrt(int id);
    }
}
