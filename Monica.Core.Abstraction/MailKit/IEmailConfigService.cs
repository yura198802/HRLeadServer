using System.Threading.Tasks;
using Monica.MailKit.EMailConfigService;

namespace Monica.Core.Abstraction.MailKit
{
    public interface IEmailConfigService
    {
        /// <summary>
        /// ��������� �������� ��� SMTP � ������
        /// </summary>
        /// <returns></returns>
        Task<SendEMailConfiguration> GetConfig();
    }
}
