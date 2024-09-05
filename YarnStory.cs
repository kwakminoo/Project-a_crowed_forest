title:A_crowed_forest
---
검은 도화지에 흰 점들이 아스라히 박힌 하늘 아래  

-> 검은 숲으로 들어간다
<<jump 검은숲의입구 >>
===

title:검은숲의입구
---

숲으로 들어서자마자 나는 끔찍한 광경을 목격했다. 불타버린 마차와 그 주변에 널브러진 시체들. 상단의 상인과 호위들로 보이는 시체들이 잔인하게 살해되어 있었다. 나는 조심스럽게 시체들 사이를 걸어가며 조사를 시작했다.

-> 갑옷을 입은 시체를 조사한다.
  <<jump 갑옷을입은시체 >>
-> 로브를 둘러쓴 시체를 조사한다.
  <<jump 로브를둘러쓴시체 >>
-> 부서진 마차를 조사한다.
  <<jump 부서진마차 >>

===

title:갑옷을입은시체
---

첫 번째로 눈에 띈 것은 강철 갑옷을 입은 기사의 시체였다. 나는 그의 몸에서 튼튼한 갑옷과 날카로운 검을 발견했다.
<<set $hasBroadsword to true>>
<<jump 광신도와의대결>>

===

title:로브를둘러쓴시체
---

다음으로 발견한 것은 긴 로브를 둘러쓴 마법사의 시체였다. 그의 손에는 오래된 마법서가 놓여 있었다.
<<set $hasSpellbook to true>>
<<jump 광신도와의대결>>

===
title:부서진마차
---

마지막으로 부서진 마차를 조사하다가 상인의 시체를 발견했다. 상인의 몸 주변에는 다양한 물품이 담긴 가방이 흩어져 있었다.

<<set $hasPotion to true>>
<<set $hasGoldMedallion to true>>
<<jump 광신도와의대결>>

===

title:광신도와의대결
---

조사를 마친 뒤, 나는 갑작스럽게 광신도와 마주쳤다. 그의 광기 어린 눈빛은 나를 향해 번뜩였다. 나는 즉시 검을 뽑아들고 전투 태세를 갖추었다.
<<jumpToScene BattleScene>>

===

title:양들의안식처마을
---
광신도와의 전투에서 승리한 나는 더 깊은 숲 속으로 걸어갔다. 얼마 지나지 않아, 나는 '양들의 안식처'라는 작은 마을에 도착했다. 마을은 작지만 필요한 모든 것이 갖춰져 있었다. 나는 마을을 둘러보며 무기점, 잡화점, 여관 중 하나를 선택해 들어가기로 했다.

-> 무기점에 들어간다.
  <<jump 무기점>>
-> 잡화점에 들어간다.
  <<jump 잡화점>>
-> 여관에 들어간다.
  <<jump 여관>>

===

title:무기점
---

무기점에 들어서자, 점원이 나를 반갑게 맞이하며 무기와 방어구를 보여주었다.

-> 무기를 구매한다.
-> 연습장을 이용한다.
===

title:잡화점
---

잡화점에 들어가 다양한 소모성 아이템을 살펴보았다. 점원은 나에게 다양한 포션과 아이템을 추천했다.

-> 물건을 구매한다.
-> 물건을 판매한다.
-> 의뢰를 수락한다.

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
