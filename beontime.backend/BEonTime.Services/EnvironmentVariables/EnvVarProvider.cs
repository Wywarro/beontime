using System;

namespace BEonTime.Services.EnvironmentVariables
{
    public static class EnvVarProvider
    {
        public readonly static string SecretKey = Environment.GetEnvironmentVariable("SecretKey");
    }
}
