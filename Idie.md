유니티 한글 폰트 깨질 때 설정
----------------
![image](https://github.com/user-attachments/assets/7b8de12c-18a7-4a16-aebd-f9184af89e8f)<br>
* Source Font File: 변환할 폰트 파일<br>
* Sampling Point Size: 폰트의 기본 크기 설정. 이 크기는 Atlas에 렌더링 되는 글자의 실제 픽셀 크기에 영향을 미치므로, 적절한 스케일링을 통해 메모리 사용량을 보존<br>
* Padding: 각 글자 주변에 추가되는 여백을 결정. 이 여백은 서로 다른 글자들 사이의 공간을 둘 때 유용하며, 텍스트의 가독성을 향상함<br>
* Atlas Resolution: 생성되는 폰트 아틀라스의 해상도를 결정. 해상도가 높을수록 텍스트의 품질이 높아지지만, 메모리 사용량이 증가함<br>
* Character Set: ASCII, Unicode 등 생성할 글자 집합을 결정. 참고로 ASCII 코드는 기본적으로 한글을 지원하지 않지만, 해당 창에서 ASCII를 선택하더라도 실제로 유니코드 문자를 지원하기 때문에 한글을 사용할 수 있음<br>
* Render Mode: 텍스트가 SDF, Raster 등 렌더링 되는 방식을 결정<br>
* Get Kerning Pairs: 폰트의 글자 쌍 사이의 간격을 조정시키는 정보를 가져올 것인지 결정


![image](https://github.com/user-attachments/assets/cff62371-3886-4cf8-bcc5-a0842afd53ac)


Visual Studio나 IDE에서 클래스 정의로 이동하기
클래스 정의 이동
----------
DialogueRunner 위에 커서를 놓고 F12 키를 누릅니다.
또는 마우스 오른쪽 버튼을 클릭하고 'Go to Definition'을 선택합니다.
메서드 확인:

DialogueRunner 클래스 파일이 열리면 클래스 내부에 정의된 메서드를 찾습니다.
SelectOption 또는 유사한 이름의 메서드를 검색합니다.

라인 뷰의 텍스트를 아래로 출력 
---------------------------------------------------------------------------------------------
1. ScrollRect: 텍스트를 스크롤하는 데 사용할 수 있는 스크롤기능들을 제공


라인뷰의 텍스트 창 크기 조절이 안될 때
---------------------------------------------------------------------------------------------
1. RectTransform 조작 문제
LineView가 UI의 일부인 경우, RectTransform을 사용하여 크기를 조정해야 합니다. 텍스트 출력 창의 부모 객체나 RectTransform 설정에 문제가 있을 수 있습니다.

해결 방법:

LineView 객체를 선택하고 RectTransform 컴포넌트를 확인합니다.
RectTransform 컴포넌트의 크기 조정 핸들을 사용하여 창의 크기를 조정합니다.
부모 객체의 RectTransform 설정이 자식 객체의 크기 조정에 영향을 미칠 수 있으므로 부모 객체의 설정도 확인합니다.
2. Canvas 설정 문제
UI 요소는 Canvas 객체의 설정에 따라 다르게 동작할 수 있습니다. Canvas의 Render Mode 설정에 따라 UI 요소의 크기 조정이 제한될 수 있습니다.

해결 방법:

Canvas 객체의 Render Mode가 Screen Space - Overlay, Screen Space - Camera, 또는 World Space 중 어떤 것으로 설정되어 있는지 확인합니다.
Render Mode에 따라 UI 요소의 크기 조정 방법이 다를 수 있으므로 적절한 모드를 선택합니다.
3. Layout Component 충돌
LineView 객체나 부모 객체에 Layout 관련 컴포넌트(HorizontalLayoutGroup, VerticalLayoutGroup, ContentSizeFitter 등)가 추가되어 있는 경우, 크기 조정이 제한될 수 있습니다.

해결 방법:

LineView 객체와 부모 객체에 추가된 Layout 관련 컴포넌트를 확인합니다.
필요에 따라 Layout 관련 컴포넌트를 일시적으로 비활성화하거나 제거하고 크기를 조정한 후 다시 활성화합니다.
4. Anchors 설정 문제
RectTransform의 Anchor 설정이 잘못되어 있는 경우, 크기 조정이 의도한 대로 작동하지 않을 수 있습니다.

해결 방법:

RectTransform의 Anchor 설정을 확인합니다. 일반적으로 Anchor는 부모 객체 내에서 UI 요소의 위치와 크기를 제어합니다.
Anchor 설정을 적절히 조정하여 크기 조정이 가능하도록 합니다.
5. Yarn Spinner 스크립트 제한
Yarn Spinner 스크립트나 설정에서 LineView의 크기 조정을 제한하는 부분이 있을 수 있습니다.

해결 방법:

Yarn Spinner 설정 파일이나 스크립트를 확인하여 크기 조정과 관련된 제한이 있는지 확인합니다.
필요에 따라 Yarn Spinner 스크립트를 수정하거나 설정을 조정합니다.

스토리 설정
---------------------------------------------------------------------------------------------

다른 별들의 힘을 빼앗기 위해 황색 옷의 왕은 다른 별의 우주로 침투할 수 있는 제단 판테온을 건설했다

1. 레이븐은 황색 옷의 왕에게 이름과 힘을 빼앗긴 별과 함께 다니며 그 별의 힘으로 황색 옷의 왕을 쫒고 있다
2. 
