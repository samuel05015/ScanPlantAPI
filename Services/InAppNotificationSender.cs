using System.Threading.Tasks;
using ScanPlantAPI.Models;
using ScanPlantAPI.Services.Interfaces;

namespace ScanPlantAPI.Services
{
    /// <summary>
    /// Implementação simples de envio in-app (nenhum envio externo, apenas marcação de disponibilidade)
    /// </summary>
    public class InAppNotificationSender : INotificationSender
    {
        public Task SendAsync(Notification notification)
        {
            // Canal in-app: nada a fazer aqui. A disponibilização ocorre via API.
            return Task.CompletedTask;
        }
    }
}
