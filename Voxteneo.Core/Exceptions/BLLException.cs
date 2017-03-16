using System.Collections.Generic;
using Voxteneo.Core.Helper;

namespace Voxteneo.Core.Exceptions
{
    /// <summary>
    /// Class BLLException
    /// </summary>
    public class BLLException : ExceptionBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="BLLException"/> class from being created.
        /// </summary>
        private BLLException() : base("") { }

        /// <summary>
        /// Initializes a new instance of the BLLException with an exception code (enum)
        /// </summary>
        /// <param name="code">The code.</param>
        public BLLException(object code)
            : base(code.ToString())
        {
            Code = code.ToString();
        }
        public BLLException(string code)
            : base(code)
        {
            Code = code;
        }
        ///// <summary>
        ///// Initializes a new instance of the BLLException with an exception code (enum) and a message
        ///// </summary>
        ///// <param name="code">The code.</param>
        //public BLLException(ExceptionCodes.BLLExceptions code, string message)
        //    : base(message)
        //{
        //    Code = code.ToString();
        //}

        /// <summary>
        /// Initializes a new instance of the BLLException with an exception code (enum) and a message
        /// </summary>
        /// <param name="code">The code.</param>
        public BLLException(object code, string message)
            : base(message)
        {
            Code = code.ToString();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BLLException"/> class with multiple error codes
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="errorCodes">The error codes.</param>
        public BLLException(object code, List<string> errorCodes)
            : base("")
        {
            Code = code.ToString();
            ErrorCodes = errorCodes;
        }
       

    }
}
