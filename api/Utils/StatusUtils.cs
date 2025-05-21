namespace api.Utils
{
    public static class StatusUtils
    {
        public static string NormalizeStatus(string status)
        {
            return status == "Error" ? "Unhealthy" : status;
        }
    }
}
