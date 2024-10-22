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
}