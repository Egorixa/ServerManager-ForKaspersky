using Domain;

namespace Application.DTOs
{
    public class ServerDto
    {
        public Guid Id { get; set; }
        public string OsName { get; set; }
        public int RamGb { get; set; }
        public int CpuCores { get; set; }
        public int DiskGb { get; set; }
        public ServerState State { get; set; }
        public bool IsPoweredOn { get; set; }
        public DateTime? ReadyAt { get; set; }
    }
}