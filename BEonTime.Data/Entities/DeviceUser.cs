namespace BEonTime.Data.Entities
{
    public class DeviceUser
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Card { get; set; }
        public int GroupId { get; set; }
        public string Privilege { get; set; }
    }
}