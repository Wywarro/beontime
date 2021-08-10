namespace Beontime.Domain.Entities
{
    public class UserOnDeviceEntity
    {
        public int DeviceUserId { get; set; }
        public string Name { get; set; } = "";
        public int Card { get; set; }
    }
}