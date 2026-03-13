namespace Application.DTOs
{
    public class CreateServerDto
    {
        public string OsName { get; set; }
        public int RamGb { get; set; }
        public int CpuCores { get; set; }
        public int DiskGb { get; set; }
        public bool IsPoweredOn { get; set; }
    }
}