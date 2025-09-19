using System.Threading.Tasks;
using ScanPlantAPI.Models;

namespace ScanPlantAPI.Services.Interfaces
{
    /// <summary>
    /// Abstração do canal de envio de notificações
    /// </summary>
    public interface INotificationSender
    {
        /// <summary>
        /// Envia (ou disponibiliza) a notificação para o usuário no canal configurado
        /// </summary>
        Task SendAsync(Notification notification);
    }
}
