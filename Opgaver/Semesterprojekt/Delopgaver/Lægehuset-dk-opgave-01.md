# Lægehuset DK - Opgave 01

## Referencer

Case beskrivelse der danner grundlag for opgaven:  [Lægehuset-dk.md](..\Lægehuset-dk.md) 



## Formål

Formålet med at løse opgaven er at kunne fremvise en minimalistisk kernen af systemet, hvor domæneobjektet `Konsultationsaftale` og `Konsultationstype` er realiseret i C#.

Men det skal være nemt at tilføje nye konsultationstyper, da der løbende opstår behov for nye konsultationstyper.



## Afgrænsning

- I denne minimalistiske udgave skal der ikke tages højde for andre Konsultationsaftale.
- I denne minimalistiske udgave gemmes Konsultationsaftaler ikke.
- I denne minimalistiske udgave er det alene egenskaber forretningslogik der er i fokus.
- Der skal IKKE laves brugerinterface - fremvisningen til kunden skal ske ved at præsenterer systemets tests.



## Funktionelle Krav

- Der skal kunne oprettes en Konsultationsaftale med tilhørende `Patient` ,  `Læge` ,  `Konsultationstype` og start tidspunkt.
- Konsultationsaftalen skal have en egenskab som indeholder et beregnet sluttidspunkt.
- Konsultationstypen skal kunne ændres.
- Start tidspunkt skal ligge i fremtiden.



## Non Funktionelle Krav

- Løsningen skal udarbejdes iht. "reglerne" for OO kode. Dvs. at der skal være fokus på:
  - Indkapsling
- Til løsningen skal der udarbejdes UnitTests



## Proceskrav

Opgaven løses efter TDD (Test Driven Development) tankerne - dvs. test kode skrives før forretningslogik kode, og der arbejdes efter "Red, Green, Refactor" metoden.



## Hint

Nogle "klasser" er tomme "dummy" klasser - andre har egenskaber og adfærd.









