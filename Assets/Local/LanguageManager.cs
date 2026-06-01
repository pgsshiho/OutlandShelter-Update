using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Legacy 드롭다운을 위해 추가
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public Dropdown dropdown; // 드롭다운 컴포넌트 연결용

    private void Start()
    {
        // 게임이 시작될 때 현재 설정된 언어에 맞춰 드롭다운 표시를 동기화합니다.
        StartCoroutine(InitDropdownCoroutine());
    }

    private IEnumerator InitDropdownCoroutine()
    {
        // Localization 시스템이 안전하게 로드될 때까지 대기
        yield return LocalizationSettings.InitializationOperation;

        // 현재 선택된 언어가 목록에서 몇 번째 인덱스인지 찾음
        var currentLocale = LocalizationSettings.SelectedLocale;
        int index = LocalizationSettings.AvailableLocales.Locales.IndexOf(currentLocale);

        if (index >= 0 && dropdown != null)
        {
            // 드롭다운 이벤트를 발생시키지 않고 값만 현재 언어 인덱스로 세팅
            dropdown.SetValueWithoutNotify(index);
        }
    }

    // 드롭다운에서 항목을 골랐을 때 실행할 함수
    public void OnDropdownValueChanged(int index)
    {
        StartCoroutine(SetLocaleCoroutine(index));
    }

    private IEnumerator SetLocaleCoroutine(int localeId)
    {
        yield return LocalizationSettings.InitializationOperation;

        // 인덱스 번호에 맞는 언어로 변경 (0 = 첫 번째 언어, 1 = 두 번째 언어)
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
    }
}