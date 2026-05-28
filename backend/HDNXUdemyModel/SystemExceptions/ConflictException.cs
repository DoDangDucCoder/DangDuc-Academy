using HDNXUdemyModel.Base;
using System.Runtime.Serialization;

namespace HDNXUdemyModel.SystemExceptions
{
    public class ProjectConflictException : System.Exception
    {
        public ProjectConflictException()
        {
        }

        public ProjectConflictException(string message) : base($"{ProjectConfig.AppName}: {message}")
        {
        }

        public ProjectConflictException(string message, System.Exception inner) : base($"{ProjectConfig.AppName}: {message}", inner)
        {
        }

#pragma warning disable SYSLIB0051
        protected ProjectConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore SYSLIB0051
        {
        }
    }
}
