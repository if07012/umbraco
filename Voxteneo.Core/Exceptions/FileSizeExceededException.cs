using System;

namespace Voxteneo.Core.Exceptions
{
    public class FileSizeExceededException : Exception
    {
        public FileSizeExceededException(string message) : base(message)
        {
            
        }
    }
}