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

public class UpdateRankingPacketReq
{
    public string jwt { get; set; } = string.Empty ;
    public int score { get; set; }
}

public class UpdateRankingPacketRes
{
    public bool success { get; set; } = false;
}

public class RankerData
{
    public string username { get; set; } = string.Empty;
    public int score { get; set; }
}

public class GetRankersPacketReq
{
    public string jwt { get; set; } = string.Empty;
}

public class GetRankersPacketRes
{
    public  List<RankerData> rankerDatas { get; set; } = new List<RankerData>();
}
