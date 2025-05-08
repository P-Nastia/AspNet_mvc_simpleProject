
using Web1.SMTP;

namespace Web1.Interfaces;

public interface ISMTPService
{
    Task<bool> SendMessage(Message message);
}
