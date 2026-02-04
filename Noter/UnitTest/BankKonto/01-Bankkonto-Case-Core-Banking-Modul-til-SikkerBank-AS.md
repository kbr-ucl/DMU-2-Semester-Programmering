# Case: Core Banking Modul til "SikkerBank A/S"

### Baggrund

SikkerBank A/S er i gang med at modernisere deres IT-systemer. Banken har oplevet fejl i deres nuværende system, hvor kassemedarbejdere ved en fejl er kommet til at oprette konti med negativ startsaldo, og kunder har kunnet hæve penge, de ikke havde (overtræk), hvilket har medført tab for banken.

Derudover har bankens sikkerhedsafdeling brug for en "nødbremse" til at stoppe transaktioner på en konto øjeblikkeligt, hvis der er mistanke om hvidvask eller identitetstyveri.

### Opgavebeskrivelse

Du er blevet hyret som Backend Udvikler til at designe og programmere kernen i det nye system: **`BankAccount`**.

Din opgave er at udvikle en C# klasse, der fungerer som en robust "Digital Bankbog". Klassen skal være selvkørende og sikre, at ingen ugyldige tilstande kan opstå, uanset hvordan resten af systemet (brugergrænsefladen) opfører sig.

### Funktionelle Krav

Systemet skal understøtte følgende funktionalitet:

1. **Oprettelse af konto:**
   - En konto skal have et unikt **kontonummer** og en **ejer** (navn).
   - Det skal være muligt at oprette en konto med en **startsaldo**.
   - *Regel:* Det må ikke være muligt at oprette en konto uden kontonummer eller med en negativ startsaldo.
2. **Indbetaling (Deposit):**
   - Der skal kunne sættes penge ind på kontoen.
   - *Regel:* Systemet skal afvise forsøg på at indsætte 0 kr. eller negative beløb.
3. **Udbetaling (Withdraw):**
   - Der skal kunne hæves penge fra kontoen.
   - *Regel:* Banken tillader **ikke** overtræk. Hvis man forsøger at hæve mere, end der står på kontoen, skal transaktionen afvises.
   - *Regel:* Negative hævninger (f.eks. -500 kr.) skal afvises.
4. **Sikkerhed (Freeze):**
   - Det skal være muligt at fryse en konto (`IsFrozen`).
   - *Regel:* Hvis en konto er frosset, skal **alle** forsøg på både indbetaling og udbetaling afvises øjeblikkeligt.

### Tekniske Krav & Kvalitetskrav

- **Sprog:** C# (.NET).
- **Arkitektur:** Løsningen skal følge objektorienterede principper, herunder **indkapsling (Encapsulation)**. Det betyder, at kontoens saldo (*Balance*) ikke må kunne ændres direkte udefra (f.eks. `konto.Balance = 1000` skal være forbudt). Ændringer må kun ske via metoder.
- **Fejlhåndtering:** Hvis en forretningsregel brydes (f.eks. overtræk), skal systemet kaste en passende `Exception` med en beskrivende fejlmeddelelse.
- **Kvalitetssikring:** Der skal udarbejdes **Unit Tests** (xUnit), der dokumenterer og verificerer, at alle forretningsregler overholdes – både "Happy Path" (det går godt) og "Unhappy Path" (fejlscenarier).

### Leverancer

1. En domæneklasse (`BankAccount`), der opfylder kravene.
2. En test-suite, der dækker scenarierne.
3. Et simpelt konsol-program, der demonstrerer brugen af klassen "live".

------

### Hvordan du bruger denne tekst

Når du præsenterer, kan du starte med at sige:

> *"Jeg har taget udgangspunkt i denne case fra SikkerBank A/S, hvor hovedproblemet var manglende dataintegritet – altså at man kunne lave overtræk og oprette ugyldige konti. Min løsning fokuserer derfor benhårdt på **Indkapsling** for at løse netop disse problemer..."*