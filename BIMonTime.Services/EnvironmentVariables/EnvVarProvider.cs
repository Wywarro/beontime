using System;

namespace BIMonTime.Services.EnvironmentVariables
{
    public static class EnvVarProvider
    {
        public readonly static string SecretKey = Environment.GetEnvironmentVariable("SecretKey");
    }
}
