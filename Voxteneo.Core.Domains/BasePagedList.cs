using System.Collections.Generic;

namespace Voxteneo.Core.Domains
{
    public class BasePagedList<T> where T : class
    {
        public List<T> Records { get; set; }
        public int TotalRecordCount { get; set; }
        public int PageSize { get; set; }
        public int PageCurrent { get; set; }
        public bool IsLast { get; set; }
        public string Result { get; set; }
    }
}