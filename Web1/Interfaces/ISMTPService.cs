
using Web1.SMTP;

namespace Web1.Interfaces;

public interface ISMTPService
{
    Task<bool> SendMessageAsync(Message message);
}
