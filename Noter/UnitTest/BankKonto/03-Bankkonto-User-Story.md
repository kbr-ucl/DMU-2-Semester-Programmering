# Bankkonto - User Stories

Nedenstående er vist fire ***User Stories for domænebegrebet "Bankkonto"***

------

### User Story 1: Oprettelse af konto

**Som** bankkunde,

**Ønsker jeg** at kunne åbne en ny konto med et startbeløb,

**Således at** jeg har et sted at opbevare mine penge sikkert.

**Acceptkriterier (AC):**

- **AC1.1:** En konto skal oprettes med et kontonummer og en ejer.
- **AC1.2:** Startsaldoen skal sættes korrekt ved oprettelse.
- **AC1.3:** Det skal **ikke** være muligt at oprette en konto med en negativ startsaldo (Systemet skal afvise det).
  - *(Mapper til test: `Constructor_WithNegativeBalance_ShouldThrowException`)*

------

### User Story 2: Indbetaling (Deposit)

**Som** bankkunde,

**Ønsker jeg** at kunne indsætte penge på min konto,

**Således at** min saldo stiger.

**Acceptkriterier (AC):**

- **AC2.1:** Når jeg indsætter et gyldigt beløb, skal saldoen stige tilsvarende.
- **AC2.2:** Det skal **ikke** være muligt at indsætte 0 kr. eller negative beløb.
  - *(Mapper til test: `Deposit_WithZeroOrNegativeAmount_ShouldThrowException`)*
- **AC2.3:** Det skal ikke være muligt at indsætte penge, hvis kontoen er frosset.

------

### User Story 3: Udbetaling (Withdraw)

**Som** bankkunde,

**Ønsker jeg** at kunne hæve penge fra min konto,

**Således at** jeg kan bruge dem til forbrug.

**Acceptkriterier (AC):**

- **AC3.1:** Når jeg hæver et beløb, skal saldoen falde tilsvarende.
- **AC3.2:** Systemet skal forhindre overtræk (Jeg må ikke hæve mere, end der står på kontoen).
  - *(Mapper til test: `Withdraw_MoreThanBalance_ShouldThrowException`)*
- **AC3.3:** Det skal ikke være muligt at hæve negative beløb.

------

### User Story 4: Sikkerhed og Frysning

**Som** bankens sikkerhedsafdeling,

**Ønsker jeg** at kunne fryse en konto ved mistanke om svindel,

**Således at** ingen penge kan flyttes ind eller ud af kontoen.

**Acceptkriterier (AC):**

- **AC4.1:** Det skal være muligt at ændre kontoens status til "Frosset".
- **AC4.2:** Hvis en konto er frosset, skal forsøg på **indbetaling** afvises med en fejlmeddelelse.
- **AC4.3:** Hvis en konto er frosset, skal forsøg på **udbetaling** afvises med en fejlmeddelelse.
  - *(Mapper til tests: `..._WhenAccountIsFrozen_ShouldThrowException`)*

