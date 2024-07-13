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
