# Hvad er god objektorienteret design



God objektorienteret design handler i bund og grund om at skabe software, der er **robust**, **fleksibel**, **genbrugelig** og **nemt at vedligeholde**. Det lyder abstrakt, men der findes nogle velafprøvede principper og mønstre, som hjælper dig derhen.

Her er en klar og praktisk gennemgang.

------

## 🧱 Grundprincipper i god objektorienteret design

### 🔒 1. **Indkapsling (Encapsulation)**

- Hold data og funktionalitet samlet i klasser.
- Skjul implementationen og eksponer kun det nødvendige via metoder.
- Gør systemet mere robust og lettere at ændre.

Dette er et af de helt centrale principper i objektorienteret programmering  [logb.dk](https://logb.dk/kurser/grundlaeggende-objektorientering/).

------

### 🧬 2. **Nedarvning (Inheritance)**

- Gør det muligt at genbruge kode ved at lade klasser arve fra hinanden.
- Brug det med omtanke – for meget nedarvning kan gøre systemet stift.

Nedarvning nævnes som en grundlæggende del af OO-design  [logb.dk](https://logb.dk/kurser/grundlaeggende-objektorientering/).

------

### 🔄 3. **Polymorfi**

- Gør det muligt at behandle objekter af forskellige typer ens, hvis de deler en fælles grænseflade.
- Gør systemet fleksibelt og udvidbart.

------

### 🧩 4. **Abstraktion**

- Fokusér på hvad et objekt *gør*, ikke hvordan det gør det.
- Brug interfaces og abstrakte klasser til at definere kontrakter.

------

## 🧭 Designprincipper (SOLID)

Selvom SOLID ikke nævnes direkte i kilderne, er det bredt anerkendt som fundamentet for god OO-design. Her er en kort version:

| Princip                   | Kort forklaring                                              |
| ------------------------- | ------------------------------------------------------------ |
| **S**ingle Responsibility | En klasse bør kun have ét ansvar.                            |
| **O**pen/Closed           | Klasser bør være åbne for udvidelse, men lukkede for ændring. |
| **L**iskov Substitution   | Subklasser skal kunne bruges som deres superklasser.         |
| **I**nterface Segregation | Mange små interfaces er bedre end få store.                  |
| **D**ependency Inversion  | Afhæng af abstraktioner, ikke konkrete klasser.              |

------

## 🧠 Objektorienteret analyse og design (OOAD)

OOAD handler om at forstå problemområdet og omsætte det til et godt design. Det involverer bl.a.:

- UML-diagrammer
- Tilstandsdiagrammer
- Analyse af krav og brugsmønstre

Dette beskrives i objektorienteret analyse og design-metoder  [da.wikipedia.org](https://da.wikipedia.org/wiki/Objektorienteret_analyse_og_design).

------

## 🧱 Designmønstre (Design Patterns)

Designmønstre er gennemprøvede løsninger på tilbagevendende problemer. Eksempler:

- **Singleton** – én instans af en klasse
- **Observer** – objekter reagerer på ændringer i andre objekter
- **Strategy** – udskiftelig adfærd

Designmønstre beskrives som en måde at genbruge arkitektur og løsninger på  [webhotel4.ruc.dk](http://webhotel4.ruc.dk/~keld/teaching/OOP_e09/Slides/pdf/04_Design.pdf).

------

## 🎯 Hvad kendetegner *godt* OO-design?

Et godt objektorienteret design er:

- **Modulært** – hver del har et klart ansvar
- **Løst koblet** – ændringer ét sted kræver ikke ændringer alle andre steder
- **Høj samhørighed** – klasser gør kun det, de skal
- **Genbrugeligt** – komponenter kan bruges i andre sammenhænge
- **Let at teste** – små, veldefinerede enheder er nemme at teste

