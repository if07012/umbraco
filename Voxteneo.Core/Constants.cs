

namespace Voxteneo.Core
{
    public class Constants
    {
        public const string CommaValue = ",";
        public const string DotValue = ".";

        public const string DashValue = "-";
        public const string DateFormatString = "{0:dd/MM/yyyy}";

        public const string TimeFormatString = "{0:HH\\:mm}";
        public const string DefaultFormatDateString = "dd/MM/yyyy";
        public const string DefaultFormatTimeString = "hh:mm tt";
        public const string DefaultFormatDecimal = "0.#####";
        public const string JavaScriptDefaultFormatDateTimeString = "DD/MM/YYYY HH:mm:ss";
        public const string JavaScriptDefaultFormatDateString = "DD/MM/YYYY";

        public const string DateTimeFormatString = "{0:dd/MM/yyyy hh\\:mm}";

        #region Regular Expression
        /// <summary>
        /// Regular expression for alpha numeric characters.
        /// </summary>
        public const string RegExpAlphaNumeric = @"([a-zA-Z0-9\s]+$)";

        /// <summary>
        /// Regular expression for numeric characters.
        /// </summary>
        public const string RegExpNumeric = "([0-9]*)";

        /// <summary>
        /// Regular expression for decimal characters.
        /// </summary>
        public const string RegExpDecimal = @"([0-9\,]*)";

        /// <summary>
        /// Regular expression for phne number characters.
        /// </summary>
        public const string RegExpPhoneNumber = @"(^[0-9\+\(\)\-\s_]+$)";
        public const string RegExpShortPhoneNumber = @"[^0-9]+";

        #endregion

        /// <summary>
        /// File name character limit to display.
        /// </summary>
        public const int FileNameLengthLimit = 40;

        public const string BackSlash = "/";

        #region password generator
        public const int PasswordLength = 8;
        public const int NonAlfaNumericNumber = 1;
        #endregion

        public const string Asterix = "*";
    }
}
