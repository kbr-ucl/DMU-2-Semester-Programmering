# **SOLID, Clean Architecture og DDD: “Det hele begynder med \*private set\*”**

Objektorienteret programmering har i årtier leveret de begreber og mekanismer, der gør software systematisk, robust og forståelig. Men i moderne softwareudvikling — særligt i enterprise‑applikationer — ser man ofte, at OOP enten bruges overfladisk eller direkte misbruges. Entiteter reduceres til datasække, der kan manipuleres frit fra alle hjørner af systemet. Resultatet er *anemiske domænemodeller*, tæt koblede lag og forretningslogik uden et sikkert hjem.

Derfor begynder denne artikel et overraskende sted:

👉 **med én enkelt linje kode:**

```c#
public string Name { get; private set; }
```

Det er netop *private set*, der markerer overgangen fra “objekter som data” til “objekter som adfærd og regler”. Og det er dér, både **SOLID**, **Clean Architecture** og **Domain‑Driven Design (DDD)** starter.

------

## 1. Objektorienteret kerneprincip: *Adfærd før data*

En af de oprindelige idéer i objektorienteret programmering er, at objekter **beskytter deres tilstand** og kun ændrer sig gennem veldefineret adfærd.

Dette respekterer nogle af de klassiske OOD‑principper som:

- **Encapsulation** – indkapsling af variation og ansvar.
- **Open/Closed Principle** – åbent for udvidelse, lukket for ændring.
- **Single Responsibility Principle** – ét objekt, ét ansvar.

Hvis en property har en **public setter**, har objektet i praksis opgivet retten til at styre sin egen tilstand. Enhver anden del af systemet kan bryde dens invariants.

Med en **private setter** tvinges al ændring gennem domænets metoder:



```c#
public void Rename(string newName)

{

  if (string.IsNullOrWhiteSpace(newName))
    throw new DomainException("Name cannot be empty");

  Name = newName;

}
```





------

## 2. SOLID begynder med private set

Flere SOLID‑principper starter med indkapsling:

### **S: Single Responsibility**

Hvis du kan ændre et objekt udefra med public setters, ender du med at have mange steder i koden, der i praksis “opholder ansvar”.

Med private setters flyttes ansvar ét sted hen: ind i objektet selv.

### **O: Open/Closed**

Når ændringer sker gennem metoder, kan du udvide adfærd uden at åbne selve objektets struktur.

Dette matcher teorien om, at objekter bør være **lukket for ændring** men **åbne for udvidelse**. 

### **L, I, D**

De øvrige SOLID‑principper styrkes af præcis de samme grunde: stærke abstraherede objekter, klare kontrakter, og minimal afhængighed.

------

## 3. Clean Architecture: at beskytte domænet

Clean Architecture arbejder ud fra et centralt princip, som kildetekster og artikler understreger tydeligt:

Domænelaget skal være **uafhængigt af alt andet** — UI, databaser, frameworks.

Med public setters kompromitterer man den beskyttelse. Domænemodellen kan ændres hvor som helst, og forretningsregler bliver spredt ud i services, controllers, repositories og hjælperklasser.

Ved at starte med private setters opnår vi:

- Domæneobjekter, der **er svære at misbruge**
- Use cases, der **udtrykker intention frem for mutation**
- En arkitektur, hvor **tilstand udelukkende ændres fra centrum**

Dette er i fuld overensstemmelse med idéerne om lagdeling og separation of concerns.

------

## 4. Domain‑Driven Design: Fra anemiske til rige modeller

DDD bygger på én grundidé:

**Software skal afspejle domænets sprog, regler og struktur.**

Det indebærer, at domæneobjekter skal rumme forretningslogik – ikke bare data.

Altså:

- En *Customer* må ikke kunne ændres uden at domænet godkender det.
- En *Order* må ikke kunne ændre status uden at følge en domæneproces.
- En *Value Object* skal være uforanderlig.

Hvis du giver entiteter public setters, ender du med **anemic domain model**, noget der eksplicit kritiseres i DDD‑litteraturen.

Sammenlign fx hvordan DDD fremhæver entiteters identitet, invariants og samarbejde med domain services.

Med **private set** fremtvinger du i stedet:

- domæneoperationer
- eksplicit regelhåndtering
- konsistente invariants
- domain events, der kun udløses når objektet har godkendt tilstandsændringen

------

## 5. Hvorfor *private set* er den lille ændring med den store effekt

Når studerende (eller udviklere i praksis) begynder at bruge private setters konsekvent, sker der tre ting:

1. **De tænker i adfærd frem for data.** Objekter bliver modelleret ud fra verb, ikke kun substantiver.
2. **Domænet flytter tilbage i centrum.** Regler kommer ind i domænelaget — ikke spredt i controllers og repositories.
3. **Clean Architecture opstår næsten af sig selv.** Når domænet er stærkt, skubber det automatiske “skallen” rundt om sig selv udad, hvor den hører hjemme.

Moderne blogs og artikler om DDD og Clean Architecture fremhæver præcis dette:
at skellet mellem *lagdeling* (Clean Architecture) og *domænets udtryk* (DDD) bliver lettere at håndhæve, når man begynder med stærke objekter, der styrer deres egen tilstand. [[khalilstemmler.com\]](https://khalilstemmler.com/articles/software-design-architecture/domain-driven-design-vs-clean-architecture/)

------

## 6. Eksempel: to versioner af det samme objekt

### ❌ **Anemisk model**



```c#
public class Student

{

  public string Name { get; set; }

  public bool IsActive { get; set; }

}
```



Et hvilket som helst service‑lag kan nu gøre:



```c#
student.IsActive = false;

student.Name = "";
```



### ✅ **Rich domain model**



```c#
public class Student

{

  public string Name { get; private set; }

  public bool IsActive { get; private set; }

  public void Rename(string name)

  {

    if (string.IsNullOrWhiteSpace(name))
      throw new DomainException("Invalid name");

    Name = name;

  }

  public void Deactivate()

  {

    if (!IsActive) return;

    IsActive = false;

    // AddDomainEvent(new StudentDeactivated(Id));

  }

}
```



Her er invarianten *en del af objektets natur*.

------

## 7. Konklusion: Det hele begynder med *private set*

Når man underviser i objektorienteret programmering, kan det være svært at forklare forskellen på:

- OOP som *datastrukturer og metoder*, og
- OOP som *abstraktion, ansvar og adfærd*.

Men der findes en lille arkitektonisk lakmustest:

👉 **Kan din property ændres uden om domænets regler?**

Hvis ja — så er dit domæne ikke beskyttet.\ Hvis nej — så er du allerede i gang med SOLID, Clean Architecture og DDD.

*Private set er ikke bare en teknisk detalje.
Det er en arkitekturmæssig markering:*
*At domænet bestemmer.*