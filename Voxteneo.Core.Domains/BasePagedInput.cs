namespace Voxteneo.Core.Domains
{
    public class BasePagedInput
    {
        public int PageSize { get; set; }
        public int PageCurrent { get; set; }
        public string Sorting { get; set; }
        public string SortingType { get; set; }
        public string Schema { get; set; }
    }
}