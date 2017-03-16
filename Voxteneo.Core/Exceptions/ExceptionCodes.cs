using System.ComponentModel;

namespace Voxteneo.Core.Exceptions
{
    public partial class VxExceptionCodes
    {
        /// <summary>
        /// Enum ExceptionCodes for user defined error codes
        /// </summary>
        public enum BaseExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception
        }

        public enum BLLExceptions
        {
            [Description("An unknown error occured")]
            UnhandledException,
            [Description("Operation not allowed")]
            OperationNotAllowed,
            [Description("The data received is not valid")]
            InvalidData,
            [Description("User not found.")]
            UserNotFound,
            CannotSendEmail,
            NullData,
           
        }

        /// <summary>
        /// Security Exceptions for wcf responses
        /// </summary>
        public enum SecurityExceptions
        {
            AccessDenied,
            AuthorizationDenied,
            AuthenticationFailure
        }
    }
}