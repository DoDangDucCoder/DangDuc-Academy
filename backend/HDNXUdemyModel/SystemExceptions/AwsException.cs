using System.Runtime.Serialization;

namespace HDNXUdemyModel.Exception
{
    using HDNXUdemyModel.Base;
    using System;

    public class ProjectAwsException : Exception
    {
        public ProjectAwsException()
        {
        }

        public ProjectAwsException(string message) : base($"{ProjectConfig.AppName}: {message}")
        {
        }

        public ProjectAwsException(string message, Exception inner) : base($"{ProjectConfig.AppName}: {message}", inner)
        {
        }

#pragma warning disable SYSLIB0051
        protected ProjectAwsException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore SYSLIB0051
        {
        }
    }
}
