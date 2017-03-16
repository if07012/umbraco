using System;

namespace Voxteneo.Core.Exceptions
{
    public class FileExtensionException : Exception
    {
        public FileExtensionException(string message)
            : base(message)
        {

        }
    }
}