using AccountDB;

public class LoginAccountPacketReq
{
    public string userId { get; set; } = String.Empty;
    public string token { get; set; } = string.Empty;
}

public class LoginAccountPacketRes
{
    public ProviderType providerType { get; set; }
    public bool success { get; set; } = false;
    public long accountDbId { get; set; }
    public string jwt { get; set; } = string.Empty;
}