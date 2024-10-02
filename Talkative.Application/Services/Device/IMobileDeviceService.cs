namespace Talkative.Application.Services.Device;

public interface IMobileDeviceService
{
    public Task AssociatePushTokenWithUser(Guid userId, string expoPushToken);
    public Task DisassociatePushTokenFromUser(Guid userId, string expoPushToken);
}