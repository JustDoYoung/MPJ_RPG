using Facebook.Unity;
using fbg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using AccessToken = Facebook.Unity.AccessToken;
using Debug = UnityEngine.Debug;

public class AuthResult
{
    public EProviderType providerType;
    public string uniqueId;
    public string token;
}

public class AuthManager
{
    const string FACEBOOK_APPID = "901425398578279";
    Action<AuthResult> _onLoginSuccess;

    #region Facebook
    public void TryFacebookLogin(Action<AuthResult> callback)
    {
        _onLoginSuccess = callback;

        if(FB.IsInitialized == false)
            FB.Init(FACEBOOK_APPID, onInitComplete: OnFacebookInitComplete);

        FacebookLogin();
    }

    private void OnFacebookInitComplete()
    {
        if (FB.IsInitialized == false) return;

        UnityEngine.Debug.Log("OnFacebookInitComplete");
        FB.ActivateApp();
        FacebookLogin();
    }

    void FacebookLogin()
    {
        UnityEngine.Debug.Log("FacebookLogin");
        List<string> permissions = new List<string>() { "gaming_profile", "email" };
        FB.LogInWithReadPermissions(permissions, FacebookAuthCallback);
    }

    void FacebookAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // 페이스북 토큰 획득
            AccessToken token = Facebook.Unity.AccessToken.CurrentAccessToken;

            AuthResult authResult = new AuthResult()
            {
                providerType = EProviderType.Facebook,
                uniqueId = token.UserId,
                token = token.TokenString,
            };

            _onLoginSuccess?.Invoke(authResult);
        }
        else
        {
            if (result.Error != null)
                Debug.Log($"FacebookAuthCallback Failed (ErrorCode: {result.Error})");
            else
                Debug.Log("FacebookAuthCallback Failed");
        }
    }
    #endregion

    #region Guest
    public void TryGuestLogin(Action<AuthResult> onLoginSucess)
    {
        _onLoginSuccess = onLoginSucess;

        AuthResult result = new AuthResult()
        {
            providerType = EProviderType.Guest,
            uniqueId = SystemInfo.deviceUniqueIdentifier,
            token = ""
        };

        _onLoginSuccess?.Invoke(result);
    }
    #endregion
}
