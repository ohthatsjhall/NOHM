#if !NET_2_0 && !NET_4_6
namespace System.IO
{
    /// <summary>Stub for the missing "InvalidDataException" in Unity builds.</summary>
    public class InvalidDataException : System.Exception
    //public class InvalidDataException : System.SystemException
    {

        public InvalidDataException() : base() { }

        public InvalidDataException(string exText) : base(exText) { }

        public InvalidDataException(string exText, Exception ex) : base(exText, ex) { }
    }
}
#endif
// © 2016-2017 crosstales LLC (https://www.crosstales.com)