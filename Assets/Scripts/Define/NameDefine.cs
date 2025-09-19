public class NameDefine 
{
    public const string JsonListTxtName = "JsonList.txt";
    public const string JsonVersionTxtName = "JsonVersion.txt";
    public const string UITypeDefineScriptName = "UITypeDefine.cs";
    public const string UserSaveInfo = "userSaveInfo.json";

    public static string CurrentPlatformName
    {
        get 
        {
            string platform;

#if UNITY_STANDALONE
            platform = "Windows";
#elif UNITY_ANDROID
            platform = "Android";
#endif
            return platform;
        } 
    }
}
