namespace Domain
{
    public class ServerEntity
    {
        public Guid Id { get; set; }
        public string OsName { get; set; } = string.Empty;
        public int RamGb { get; set; }
        public int CpuCores { get; set; }
        public int DiskGb { get; set; }

        public ServerState State { get; set; } = ServerState.Available;
        public bool IsPoweredOn { get; set; }
        public DateTime? ReadyAt { get; set; }

        public DateTime? RentedAt { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public void StartPoweringOn(TimeSpan powerOnDelay)
        {
            State = ServerState.PoweringOn;
            ReadyAt = DateTime.UtcNow.Add(powerOnDelay);
            IsPoweredOn = true;
        }

        public void Rent()
        {
            State = ServerState.Rented;
            ReadyAt = null;
            RentedAt = DateTime.UtcNow;
        }

        public void Release()
        {
            State = ServerState.Available;
            ReadyAt = null;
            RentedAt = null;
        }
    }
}