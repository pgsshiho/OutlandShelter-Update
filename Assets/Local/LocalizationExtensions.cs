using UnityEngine.Localization.Settings;

public static class LocalizationExtensions
{
    public static string Localize(this string key, string table = "En", params object[] args)
    {
        // args가 없으면 Smart String 해석을 시도하지 않도록 GetLocalizedString의 오버로드를 구분합니다.
        if (args == null || args.Length == 0)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString(table, key, arguments: args);
    }
}