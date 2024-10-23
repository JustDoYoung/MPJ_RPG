using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


namespace WebPacket
{
	[Serializable]
	public class TestPacketReq
	{
		public string userId;
		public string token;
	}

	[Serializable]
	public class TestPacketRes
	{
		public bool success;
	}

	[Serializable]
	public class LoginAccountPacketReq
	{
		public string userId;
		public string token;
	}

    [Serializable]
    public class LoginAccountPacketRes
    {
        public EProviderType providerType { get; set; }
		public bool success;
		public long accountDbId;
		public string jwt;
    }
    [Serializable]
    public class UpdateRankingPacketReq
    {
		public string jwt;
		public int score;
    }
    [Serializable]
    public class UpdateRankingPacketRes
    {
		public bool success;
    }

    [Serializable]
    public class GetRankersPacketReq
    {
        public string jwt = string.Empty;
    }
    [Serializable]
    public class GetRankersPacketRes
    {
        public List<RankerData> rankerDatas= new List<RankerData>();
    }
    [Serializable]
    public class RankerData
    {
        public string username  = string.Empty;
        public int score;
    }
}