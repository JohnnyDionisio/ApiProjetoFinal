namespace APItoPFinal.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public int ExpiraEmMinutos { get; set; } = 60;
    }
}
