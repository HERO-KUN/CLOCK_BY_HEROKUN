# Clock by HEROKUN with Unity

<img src="https://github.com/HERO-KUN/CLOCK_BY_UNITY/blob/master/Screenshots/default_preferences.png" width="100%" title="main screen with default preferences"/>

<img src="https://github.com/HERO-KUN/CLOCK_BY_UNITY/blob/master/Screenshots/decorated.png" width="100%" title="main screen with default preferences"/>

#####Wallpaper Engine에 붙히는 배경화면용 시계 프로그램.
#####시계, 배경, 배경의 밝기효과, 제목, 부제목이 전부입니다.
#####시계와 배경을 제외한 요소들은 선택적요소이며 사용하지 않을 수 있습니다.


### Preferences

#####해당 어플리케이션은 다음 4가지 설정 요소를 갖추고 있습니다.
- 제목(title) : 화면 좌측의 두 줄 텍스트 중 윗줄의 텍스트를 정합니다.
- 부제목(subtitle) : 화면 좌측의 두 줄 텍스트 중 아랫줄의 텍스트를 정합니다.
- 배경이미지 이름(background) : 배경이미지의 파일명을 정합니다. 확장명을 포함해야합니다.
- 밝은 배경이미지 이름(background_bright) : 밝은 배경이미지의 파일명을 정합니다. 이 또한 확장명을 포함해야 하며, 없는 파일이름으로 정하면 사용안함 모드가 됩니다.

#####위의 설정들을 json문법에 맞게 (실행파일위치)/Preferences/prefs.json 파일에 저장하면 설정이 반영됩니다.
#####예시는 아래와 같습니다.

```
{
	"title":"public class HeroKun extends Person { ... }",
	"subtitle":"마음만은 언제까지나 멍멍이 소년",
	"background":"background.png",
	"background_bright":"background_bright.png"
}
```

##### * 밝은 배경이미지에 대해
#####밝기 애니메이션을 위한 옵션이며, 정상적으로 설정시 배경 이미지 위에 밝은 배경 이미지가 페이드 인/아웃 효과를 통해 천천히 깜빡입니다.
#####포토샵 등의 프로그램으로 명도 등의 조정을 주고 저장하여 설정했을 때 최적의 효과를 발휘합니다.