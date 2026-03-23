using UnityEngine.Localization.Settings;

public static class LocalizationExtensions
{
    // string 키 값에서 바로 번역문을 가져오는 확장 메서드
    public static string Localize(this string key, string table = "En", params object[] args)
    {
        // GetLocalizedString의 인자로 args를 넘기면 Smart String이 처리됩니다.
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, key, arguments: args);
    }
}