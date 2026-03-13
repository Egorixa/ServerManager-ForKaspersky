namespace Application.DTOs
{
    public class ServerSearchFilterDto
    {
        public string OsName { get; set; }
        public int? MinRamGb { get; set; }
        public int? MinCpuCores { get; set; }
        public int? MinDiskGb { get; set; }
    }
}