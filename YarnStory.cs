  title:A_crowed_forest
---
검은 도화지에 흰 점들이 아스라히 박힌 하늘 아래 달빛이 깔아놓은 레드카펫을 밝고 나는 걸어갔다

-> 검은 숲으로 들어간다
<<jump 검은숲의입구 >>
===

title:검은숲의입구
---

숲으로 들어서자 부서진 마차와 시체들이 사방으로 넓으러져 있다. 쓸만한 것이 있을지도 모른다

-> 가슴이 찢겨나간 시체를 조사한다.
  <<jump 갑옷을입은시체 >>
-> 머리가 잘린 시체를 조사한다.
  <<jump 로브를둘러쓴시체 >>
-> 부서진 마차를 조사한다.
  <<jump 부서진마차 >>

===

title:갑옷을입은시체
---

<<set $hasBroadsword to true>>
<<jump 라베스와의대결>>

===

title:로브를둘러쓴시체
---

<<set $hasSpellbook to true>>
<<jump 라베스와의대결>>

===
title:부서진마차
---

<<set $hasPotion to true>>
<<set $hasGoldMedallion to true>>
<<jump 라베스와의대결>>

===

title:라베스와의대결
---

수풀 속에서 요란한 소리를 내며 라베스가 튀어나왔다. 손과 입에 질척하게 피를 묻힌 모습이 방금 식사를 마친 것 같았지만 아직 만족하지 못했는지 포식자의 눈으로 나에게 달려들었다.
<<jumpToScene BattleScene>>

===

title:마을입구
---
나는 더 깊은 숲 속으로 걸어갔다. 얼마 지나지 않아, 나는 '양들의 안식처'라는 작은 마을을 발견했다. 마을은 작지만 필요한 모든 것이 갖춰져 있는 것 같다.

-> 마을로 들어간다
<<jump 양들의안식처>>
-> 그냥 지나친다
  <<jump >>

===

title:양들의안식처
---
허름한 외관과 달리 여러 가게들이 눈에 보이고 거리에 사람들은 대대분 무기를 가지고있다. 지나가갈 때마다 적대적인 눈빛이 보낸다. 이방인을 경계한다기에는 확실한 적의와 두려움이 느껴진다. 무슨일이 있었던 걸까?

  ->대장간으로 간다
  <<jump 대장간>>
  ->잡화점으로 간다
  <<jump 잡화점>>
  ->여관으로 간다
  <<jump 여관>>
===
  
title:대장간
---

"처음 보는 놈이군" 다부진 체격과 술냄새, 호탕한 말투의 대장장이는 마을 분위기와는 상반된 모습이다. "지옥에 온걸 환영한다 애송이 외상은 안받아 언제 죽을지도 모르는 것들이 외상원 뭔~"

->구매한다.
->판매한다.
->말을건다.(물건을 샀을 때)
  "어이 애송이 좋은 걸 하나 알려주지 마을 서쪽에 안개가 보이거든 도망쳐. 그곳으로는 얼씬도 하지 말란 소리야. 거기 들어갔다 죽은 놈이 한둘이 아니거든. 으잉? 왜 이런걸 말해주냐고? 내가 네놈 걱정돼서 이런 말을 하겠냐? 다음에 또 와야 뭐라도 팔릴거 아니야! 난 '손님'에게는 친절하다고~"
->가게를 나온다
===

title:잡화점
---

잡화점에 들어가니 여러 물약과 다양한 도구들이 빼곡하게 나열되어있다 안쪽에서 피어오르고 있는 연기와 알 수 없는 약초냄새때문에 머리가 어지러울 지경이다
"못 보던 손님이네 천천히 둘러봐요 없는 것 빼곤 다있으니까." 

->구매한다.
->판매한다.
->말을건다.
  "...흐음 의뢰하나 받을래? 요즘 우리 가게에 푸른손가락 꽃이 부족해. 원래 정기적으로 거래를 하던 사람이 있었는데 언젠가부터 안보이더라고 어디서 죽었을 지도 모르지. 어때 관심있어?"
  ->수락한다
    "고마워 안그래도 골란하던 참이거든 그렇게 어려운건 아니야 가게 뒤편에서 이어지는 언덕을 따라 올라가면 푸른손가락 꽃이 가득한 군락지가 나와 거기서 꽃을 따와주면 되 어렵지 않지?"
    나는 꽃을 담을 바구니를 받고선 안내를 받아 언덕으로 올라가는 길 앞에 섰다
  ->거절한다
    "아쉽네 어쩘 수 없지"
    나는 가게를 나왔다
===

title: 언덕초입1
---
나는 언덕을 따라 올라갔다 정돈되진 않았지만 사람이 오고간 흔적으로 길이 나있었다
가는 동중 갑자기 나무뒤에서 야수한마리가 나타났다
나를 바라보며 발톱을 새우고 다가온다
<<jumpToScene BattleScene>>
===

title:언덕초입2
---
올라가던 도중 수풀을 해치며 한 소녀가 튀어나왔다.
"어! 안녕하세요" 밝고 명량하게 인사를 건내는 소녀를 보며 나는 일단 검을 다시 집어넣었다.
"아저씨 어디로 가요? 어! 나도 거기로 가요! 푸른화원! 같이갈래요?"

  ->함께 간다
    "우와! 고마워요 어자씨! 사실 혼자가기 좀 무서웠거든요 헤헤" 수줍게 말하는 소녀는 얼굴에 미소를 피우며 내 손을 잡았다 나는 그 손을 구지 놓지 않았다
    <<jump 언덕초입3>>
  ->따로간다
    "아...어...하..하하 네 그럼 조심히 가요!" 소녀는 나무 뒤로 사라졌다.
  <<jump 언덕중간3>>
  ->소녀를 공격한다
    소녀를 바라보았다 흰원피스에 검은 생머리라 허리까지 내려뜨렸고 똘망똘망한 눈은 순진무구한 어린 소녀였다
    나는 시선을 내렸다. 소녀는 맨발이었다. 아무리 길이 있다지만 정돈되지 않는 이 산길을 맨발로 저렇게 깨끝이 다닐 수 있을까?
    순간 검을 꺼내들어 소녀를 향해 휘둘렀다 소녀는 그림자가 없었다
    소녀를 공격하자 소녀의 몸이 아지랑이가 되어 사라졌다.
    마지막으로 본 소녀의 눈빛은 이상하게도 나를 걱정하는 듯 해보였다
    <<jump 언덕중간3>>
===

title: 언덕초입3
---
길을 따라가며 소녀는 쉴새없이 떠들었다. 호기심에 초롱초롱 빛나는 눈과 순수한 웃음을 볼 때면 잊었던 평온이 찾아온 기분이었다
"아저씨 이거 먹을래요?" 소녀가 붉은 조그마한 열매를 내밀었다.
  ->먹는다
    (체력회복)
    <<jump 언덕중간1>>
  ->먹지 않는다
    "...이거 맛있는데" 소녀는 볼을 크게 부풀리고는 나를 앞질러갔다. 토라진 그 모습이 퍽 귀여웠다.
    <<jump 언덕중간1>>
===

title: 언덕중간1
---
소녀가 갑자기 걸음을 멈췄다
나는 소녀를 살펴보았다. 소녀의 얼굴은 파랗게 질려 창백했고 어깨는 심하게 떨리고있었다 마치 저 앞에 무언가를 두려워하는 것 같았다
소녀의 어깨에 손을 올리려던 순간 소녀는 비명을 지르며 도망쳤다

  -> 소녀를 쫒아간다
    <<jump 언덕중간2>>
  ->앞으로 나아간다
    나는 검을 뽑아들고 수풀넘어로 나아갔다
    <<jump 언덕중간3>>
===

title: 언덕중간2
---
나는 소녀를 쫒아왔다. 소녀는 나무 밑에 쪼그려 울고있었다.
"아저씨 괴물들이 있어요...괴물들이 제 친구를 해칠려고 했는데 못 도와줬어요"
소녀는 끝없이 흐르는 눈물을 두 손으로 억지로 막으며 말했다
"너무 무서워서...도망쳤어요 그러면 안됐는데 제가 친구를 버렸어요"
소녀의 목소리는 후회와 죄책감에 젖어 아무리 흘려보내도 사라지지 않을 듯 들렸다

  ->소녀를 도와준다
    "정말요? 정말 도와줄거에요?...제 친구를 구해 줄 수 있어요?"
    나는 고개를 끄덕였다.
    "...이거 가져가요 필요할 거에요"
    소녀는 푸른색 꽃으로 만든 반지를 내 손에 끼워주며 말했다.
    "제 친구를 꼭 구해주세요 아저씨"
    소녀는 이제 울지 않았다. 나는 자리에서 일어나 다시 길을 올라갔다
    "...고마워요 아저씨"
    <<jump 언덕중간3>>
  ->길을 따라간다
    나는 소녀를 버려두고 길을 올라갔다
    어린소녀를 어르고 달래는 기술 같은건 나한테 없다
    이 지옥에서 모든 것은 스스로 감당해야하는 것이고 힘이 없음이 죄가 되는 세상이다
    <<jump 언덕중간3>>
===

title: 언덕중간3
---
나는 소녀를 뒤로하고 언덕을 올라갔다
수풀을 해치고 올라가자 검은 로브를 둘러쓴 무리들 발견했다
옷은 넝마가 되있었고 대부분 피를 흘린채 숨을 헐떡이며 몇몇은 팔다리가 잘린 인원도 있었다
그들의 옷에 그려진 문양이 내 눈을 사로잡았다. 그들은 황색이었다
<<jumpToScene BattleScene>>
===

title: 푸른화원1
---
황색들을 죽이고 언덕을 올라 드디어 정상에 올랐다.
초록색 잎사귀 위로 수만개의 푸른 꽃들이 바람이 불 때마다 일렁이는 파도가 되어 화원을 이루고 있었다
꽃을 꺽으려는데 저 멀리 무언가 푸른 꽃들 사이에 어울리지 않은 검은색 거대한 형체가 보였다
그것은 순간 날아오르더니 내 앞에 떨어졌다. 몸 이곳 저곳에 피를 흘리는 거대한 까마귀였다

  ->공격한다
    <<jumpToScene BattleScene>>
    (데미지 주고 시작)
  ->다가간다
    (반지가 있을 때) <<jump 푸른화원2>>
    (반지가 없을 때) 나는 까마귀에 머리로 천천히 손을 가져갔다. 그런 내손을 날카로운 발톱으로 쳐내며 날아올랐다 <<jumpToScene BattleScene>>(데미지 입음)
===

title:여관
---

여관에 들어가 따뜻한 식사와 숙박을 요청했다. 여관 주인은 나에게 검은 숲과 광신도들에 대한 다양한 정보를 제공했다.

-> 정보를 수집한다.
-> 휴식을 취한다.

===

title:마무리
---

[[선택한 장소에서 필요한 물품을 챙기고 정보를 얻은 나는 이제 검은 숲 속 더 깊은 곳으로 들어가 광신도들을 추적하며 복수의 여정을 계속 이어갈 준비가 되었다. 나의 복수는 이제 막 시작되었다.]]
===
