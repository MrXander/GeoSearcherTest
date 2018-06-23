#region

using System;

#endregion

namespace DataAccess.Exceptions
{
    public class FileDbException : Exception
    {
        public FileDbException(string message)
            : base(message)
        {
        }
    }
}