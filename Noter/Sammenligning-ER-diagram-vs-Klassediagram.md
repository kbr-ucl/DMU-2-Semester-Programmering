### Sammenligning: ER-diagram vs Klassediagram

| **Kriterium**      |                                         **ER-diagram (ERD)** |                                      **Klassediagram (UML)** |
| ------------------ | -----------------------------------------------------------: | -----------------------------------------------------------: |
| **Formål**         | Modellere data og databaser; fokus på **entiteter** og **attributter** | Modellere objektorienteret design; fokus på **klasser**, **metoder** og adfærd |
| **Elementer**      |                      **Entitet**, **attribut**, **relation** | **Klasse**, **attribut**, **operation**, **association**, **arv** |
| **Relationstype**  |             Relationer mellem entiteter; kardinalitet vigtig | Associationer, afhængigheder, aggregation, komposition; multiplicitet og navigerbarhed |
| **Semantik**       |                              Datamodel: hvad der skal gemmes | Designmodel: hvordan systemet er struktureret og opfører sig |
| **Brugstidspunkt** |                                     Tidligt i krav/DB-design |                    Under design og implementering i OO-sprog |

[Visual Paradigm Guides](https://guides.visual-paradigm.com/class-diagram-vs-entity-relationship-diagram-erd-a-comparative-guide/)  [Creately](https://creately.com/guides/uml-vs-erd/)

------

### Hvad er et ER-diagram

**ER-diagram** beskriver dataobjekter (entiteter), deres attributter og relationer mellem dem. Fokus er på **dataintegritet**, normalisering og kardinalitet (1:1, 1:N, N:M). ERD bruges primært til at designe relationelle databaser og viser ikke metoder eller adfærd.   [Visual Paradigm Guides](https://guides.visual-paradigm.com/class-diagram-vs-entity-relationship-diagram-erd-a-comparative-guide/)

### Hvad er et klassediagram

**Klassediagram** er en UML‑model, der viser **klasser** med attributter og operationer samt relationer mellem klasser. Klassediagrammer indfanger både struktur og designbeslutninger i objektorienterede systemer, herunder arv, interfaces, synlighed og forskellige relationstyper som association, aggregation og komposition.   [Wikipedia](https://en.wikipedia.org/wiki/Class_diagram)  [Mundobytes](https://mundobytes.com/da/UML/)

------

### Relationer og associationer i klassediagrammer

#### Grundlæggende association

- **Association** er en strukturel forbindelse mellem to klasser. Den siger, at objekter af én klasse kender til eller bruger objekter af en anden klasse.   [Guru99](https://www.guru99.com/da/association-aggregation-composition-difference.html)

#### Envejs vs tovejs (navigerbarhed)

- **Tovejs association**: Begge klasser kan referere til hinanden; navigerbarhed i begge retninger. Noteres ofte uden pile eller med pile i begge retninger.
- **Envejs association**: Kun én klasse har en reference til den anden; noteres med en pil fra den navigerende klasse mod den målte klasse.
- **Praktisk betydning**: Envejs betyder mindre coupling og enklere ansvar; tovejs bruges når begge objekter aktivt skal finde hinanden. Angiv altid **multipliciteter** (fx `1`, `0..*`, `1..*`) ved associationen.   [Guru99](https://www.guru99.com/da/association-aggregation-composition-difference.html)

------

### Aggregation og komposition (specialiseringer af association)

#### Aggregation (svag helhed)

- **Definition**: En **helhed-del** relation hvor delene kan eksistere uafhængigt af helheden.
- **Notation**: Åben diamant ved helhedsenden.
- **Semantik**: “Har‑en” relation uden stærk livscyklusbinding. Eksempel: Et **Hold** har **Spillere**; en spiller kan eksistere uden holdet.   [Guru99](https://www.guru99.com/da/association-aggregation-composition-difference.html)

#### Komposition (stærk helhed)

- **Definition**: En stærkere helhed‑del relation hvor delens livscyklus er bundet til helheden.
- **Notation**: Fyldt (sort) diamant ved helhedsenden.
- **Semantik**: Når helheden slettes, slettes delene typisk også. Eksempel: Et **Hus** består af **Rum**; rum eksisterer normalt ikke uden huset.   [Guru99](https://www.guru99.com/da/association-aggregation-composition-difference.html)  [Wikipedia](https://en.wikipedia.org/wiki/Class_diagram)

#### Hvordan vælge mellem dem

- Spørg: *Kan delen eksistere meningsfuldt uden helheden?*
  - **Ja** → Aggregation.
  - **Nej** → Komposition.
- Overvej ejerskab og livscyklus: Komposition implicerer ejerskab og ansvar for oprettelse/sletning.

------

### Undervisningsstrategier til at afklare forveksling

1. **Vis parallelle diagrammer**
   - Tegn samme domæne som både ERD og klassediagram side om side. Marker hvad der er **entitet/attribut** vs **klasse/metode**.   [Visual Paradigm Guides](https://guides.visual-paradigm.com/class-diagram-vs-entity-relationship-diagram-erd-a-comparative-guide/)
2. **Fokus på spørgsmål, ikke notation**
   - Stil spørgsmål: *Er vi modellerer data til lagring?* → ERD. *Er vi modellerer objekter og adfærd til implementering?* → Klassediagram.
3. **Øvelser med navigerbarhed**
   - Giv korte opgaver: “Tegn en envejs association hvor kun A kender B” og “Tegn en tovejs association hvor begge kan finde hinanden”. Bed dem forklare konsekvenser for kode (referencer, getters).
4. **Livscyklus-cases for aggregation/komposition**
   - Brug konkrete scenarier: bil og hjul (komposition? nej, hjul kan skiftes men ofte eksisterer hjul uden bil) kontra bog og kapitel (kapitler giver sjældent mening uden bog → komposition). Diskuter hvorfor valget kan være kontekstafhængigt.
5. **Kodelink**
   - Vis hvordan associationer, aggregation og komposition typisk implementeres i et OO-sprog (referencer, ejerskab, destruktor/garbage collection) for at forbinde notation med praksis.

------

### Eksempler (tekstlig notation)

- **Envejs association**: `Kunde -> Ordre` multiplicitet `1 -> 0..*` (Kunde kender sine ordrer; ordre behøver ikke reference tilbage).
- **Tovejs association**: `Student <-> Kursus` multiplicitet `* <-> *` (Begge kan finde hinanden).
- **Aggregation**: `Team <>— Player` (åben diamant ved Team).
- **Komposition**: `House ◆— Room` (fyldt diamant ved House).

------

### Konklusion

Skelnen ligger i **formål** og **semantik**: ER‑diagrammer beskriver **data** og struktur til lagring; klassediagrammer beskriver **objekter**, ansvar og adfærd i et OO-design. Associationer handler om **forbindelser og navigerbarhed**; aggregation og komposition er to grader af helhed‑del‑relationer, hvor komposition indebærer stærkere ejerskab og livscyklusbinding. Brug konkrete eksempler, parallelle tegninger og kodeøvelser for at gøre forskellene håndgribelige for studerende.   [Visual Paradigm Guides](https://guides.visual-paradigm.com/class-diagram-vs-entity-relationship-diagram-erd-a-comparative-guide/)  [Guru99](https://www.guru99.com/da/association-aggregation-composition-difference.html)  [Wikipedia](https://en.wikipedia.org/wiki/Class_diagram)