# Bankkonto - Objekt Orienteret Analyse & Design

Her er en beskrivelse af den **OOA/D (Objekt Orienteret Analyse & Design)** proces, der logisk leder frem til `BankAccount` klassen.

------

### 1. Domæneanalyse og Abstraktion

Første skridt var at kigge på virkelighedens "Bankkonto" og udføre en **abstraktion**. Vi kan ikke modellere *alt* ved en bankkonto (renter, valuta, SWIFT-koder, medejere), så vi skal udvælge det, der er relevant for netop denne case.

Vi identificerede de centrale **substantiver (navneord)**, som blev til vores properties:

- **Hvem ejer den?** $\rightarrow$ `Owner` (Vi har brug for at knytte kontoen til en person).
- **Hvor mange penge er der?** $\rightarrow$ `Balance` (Kontoens kernefunktion).
- **Hvad identificerer den unikt?** $\rightarrow$ `AccountNumber` (Navne kan være ens, men kontonumre er unikke).
- **Hvad er dens tilstand?** $\rightarrow$ `IsFrozen` (Vi har brug for en sikkerhedsmekanisme).

**Designvalg vedr. Datatyper:**

- **`decimal` til Balance:** Vi valgte bevidst `decimal` frem for `double` eller `float`. Flydende kommatal (`double`) kan have afrundingsfejl (f.eks. kan 0.1 + 0.2 blive 0.30000000004), hvilket er uacceptabelt i finansielle systemer. `decimal` er præcis.
- **`string` til AccountNumber:** Selvom det hedder et "nummer", kan det indeholde foranstillede nuller (00123) eller bindestreger, som ville gå tabt i en `int`.

------

### 2. Adfærdsanalyse (Methods)

Næste skridt var at identificere **verberne (udsagnsord)**. En bankkonto er ikke bare en beholder til data; det er en aktiv ting, man interagerer med.

Vi stillede spørgsmålet: *"Hvad må man gøre ved en konto?"*

- Man skal kunne sætte penge ind -> `Deposit()`
- Man skal kunne tage penge ud -> `Withdraw()`
- Banken skal kunne spærre kontoen -> `FreezeAccount()`

Vi valgte **Rich Domain Model** frem for en *Anemic Domain Model*. Det betyder, at logikken ligger *inde* i klassen (i metoderne) i stedet for at ligge udenfor i en "Service"-klasse.

------

### 3. Analyse af Integritet og Inkapsling (Encapsulation)

Dette er den vigtigste del af analysen. Vi spurgte: *"Hvordan sikrer vi, at data altid er korrekt?"*

Hvis vi gjorde `Balance` public (`public decimal Balance { get; set; }`), ville enhver udvikler kunne skrive:

```
account.Balance = -1000000;
```

uden nogen kontrol. Det ville ødelægge dataens integritet.

**Løsningen blev Inkapsling:**

1. **`private set`:** Vi lukkede for direkte skrivning til `Balance` og `IsFrozen`.
2. **Gatekeepers:** Vi tvinger omverdenen til at bruge `Deposit` og `Withdraw`. Disse metoder fungerer som "dørmænd", der tjekker ID (forretningsregler), før de ændrer data.

------

### 4. Definition af Forretningsregler (Invarianter)

Til sidst definerede vi de regler (invarianter), der *altid* skal være sande for at systemet er i en gyldig tilstand.

Disse regler blev oversat direkte til `if`-sætninger og `exceptions` i koden:

| **Forretningsregel (Krav)**                 | **Implementering i koden**                                   |
| ------------------------------------------- | ------------------------------------------------------------ |
| **Penge kan ikke opstå ud af det blå.**     | `Deposit` kaster fejl, hvis beløbet er $\le 0$.              |
| **Penge kan ikke forsvinde.**               | `Withdraw` kaster fejl, hvis beløbet er $\le 0$.             |
| **Man kan ikke bruge penge, man ikke har.** | `Withdraw` tjekker `if (amount > Balance)`.                  |
| **Sikkerhed står over alt andet.**          | Både `Deposit` og `Withdraw` tjekker først `IsFrozen`.       |
| **En konto skal være gyldig fra start.**    | `Constructor` sikrer, at vi ikke kan lave en konto uden ejer eller med negativ start-saldo. |

------

### Visuel opsummering (Diagram)

Du kan tegne dette på en tavle eller vise det. Det illustrerer, hvordan metoderne beskytter dataene.

Kodestykke

```
classDiagram
    class BankAccount {
        - decimal Balance
        - bool IsFrozen
        + string Owner
        + string AccountNumber
        + Deposit(amount)
        + Withdraw(amount)
        + FreezeAccount()
    }
    
   
```

> [!NOTE]
>
>  Note for BankAccount: "Metoderne (Deposit/Withdraw) fungerer som et skjold.
>
> De beskytter 'Balance' mod ugyldige ændringer."



### Opsamling

Analysen kan sammenfattes i sætningen:

> *"Vi har designet en klasse med **høj kohæsion** (den gør én ting: styrer en konto) og stærk **indkapsling** (den beskytter sine egne data mod ugyldig brug gennem validering i metoderne)."*

