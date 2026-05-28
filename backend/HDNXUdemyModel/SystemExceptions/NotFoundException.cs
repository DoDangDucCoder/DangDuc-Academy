using HDNXUdemyModel.Base;
using System.Runtime.Serialization;

namespace HDNXUdemyModel.SystemExceptions
{
    using System;

    public class ProjectNotFoundException : Exception
    {
        public ProjectNotFoundException()
        {
        }

        public ProjectNotFoundException(string message) : base($"{ProjectConfig.AppName}: {message}")
        {
        }

        public ProjectNotFoundException(string message, Exception inner) : base($"{ProjectConfig.AppName}: {message}", inner)
        {
        }

#pragma warning disable SYSLIB0051
        protected ProjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore SYSLIB0051
        {
        }
    }
}
