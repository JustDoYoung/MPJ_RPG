using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WebPacket;
using static Define;

public class UI_LoginScene : UI_Scene
{
    enum Buttons
    {
        FacebookButton,
        GuestButton
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.FacebookButton).gameObject.BindEvent(OnClickBindEventButton);

        return true;
    }

    private void OnClickBindEventButton(PointerEventData data)
    {
        Managers.Auth.TryFacebookLogin((result) => { OnLoginSuccess(result, EProviderType.Facebook); });
    }


    void OnLoginSuccess(AuthResult result, EProviderType providerType)
    {
        LoginAccountPacketReq req = new LoginAccountPacketReq()
        {
            userId = result.uniqueId,
            token = result.token,
        };

        string url = "";

        switch (providerType)
        {
            case EProviderType.Guest:
                url = "guest";
                break;
            case EProviderType.Facebook:
                url = "facebook";
                break;
            case EProviderType.Google:
                url = "google";
                break;
            default:
                break;
        }

        Managers.Web.SendPostRequest<LoginAccountPacketRes>($"api/account/login/{url}", req, (res) =>
        {
            if (res.success == true)
            {
                Debug.Log("Login Success");
                Debug.Log($"AccountDbId : {res.accountDbId}");
                Debug.Log($"JWT : {res.jwt}");

                Managers.Jwt = res.jwt;

                //Ranking test
                //UpdateRank();
                GetRankers();

                //todo : game server
            }
            else
            {
                Debug.Log("Login Fail");
            }
        });
    }

    void GetRankers()
    {
        GetRankersPacketReq req = new GetRankersPacketReq()
        {
            jwt = Managers.Jwt
        };

        Managers.Web.SendPostRequest<GetRankersPacketRes>($"api/ranking/getrankers", req, (res) =>
        {
            if (res.rankerDatas.Count > 0)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    //temp
    void UpdateRank()
    {
        UpdateRankingPacketReq req = new UpdateRankingPacketReq()
        {
            jwt = Managers.Jwt,
            score = 100
        };

        Managers.Web.SendPostRequest<UpdateRankingPacketRes>($"api/ranking/update", req, (res) =>
        {
            if (res.success == true)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }
}
