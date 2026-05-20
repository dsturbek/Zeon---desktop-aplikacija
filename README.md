# 💻 Zeon Desktop – WPF Stolna Aplikacija za Upravljanje Fitness Klijentima i Analitiku

Zeon Desktop je robusna stolna aplikacija namijenjena osobnim fitness trenerima za centralizirano upravljanje klijentima, izradu personaliziranih planova treninga/prehrane te detaljno praćenje napretka. Aplikacija rješava problem fragmentacije alata u fitness industriji integrirajući napredne sustave za analitiku podataka, generiranje izvještaja i komunikaciju s klijentima na jednom mjestu.

> 👥 **Napomena o projektu:** Ovaj projekt je razvijen u sklopu timskog rada na fakultetu (4 člana) kao dio šireg Zeon ekosustava. Moj primarni doprinos na desktop aplikaciji obuhvaćao je razvoj **modula za prehranu i recepte**, sustava za **vizualizaciju podataka i pregled feedbacka** te implementaciju **algoritama za generiranje PDF izvještaja o napretku klijenta**.

---

## 📐 Arhitektura sustava (N-Tier Architecture)

Projekt je strukturiran kao višeslojni programski sustav podijeljen u tri zasebna projekta unutar istog Visual Studio rješenja (*Solution*), čime je postignuta stroga modularnost i ponovna iskoristivost koda:

1. **Prezentacijski sloj (WPF):** Odgovoran za moderan i responzivan GUI, validaciju korisničkih unosa na klijentskoj strani te vizualni prikaz i prosljeđivanje zahtjeva.
2. **Sloj poslovne logike (BLL):** Sadrži domenske entitete (`Trainer`, `Client`, `WorkoutPlan`), servisne klase za poslovne operacije i napredne validacijske klase (sprječavanje dupliciranja). Komunicira isključivo s podatkovnim slojem.
3. **Sloj pristupa podacima (DAL):** Izoliran kroz repozitorijski uzorak (*Repository Pattern*). Sadrži kontekstne klase i repozitorije koji skrivaju centraliziranu SQL bazu podataka od viših slojeva, osiguravajući podatke normalizirane do 3. normalne forme (3NF).

---

## 🛠️ Tehnološki stog (Tech Stack)

- **Jezik i platforma:** C# / .NET Framework
- **Korisničko sučelje:** WPF (Windows Presentation Foundation) uz XAML
- **Pristup podacima:** Entity Framework (ORM) – Object-Relational Mapping
- **Upiti nad podacima:** LINQ (Language Integrated Query) za kompleksno filtriranje i transformaciju podataka
- **Izvoz dokumenata:** .NET biblioteke za dinamičko generiranje i oblikovanje PDF datoteka
- **Modeliranje:** Visual Paradigm (UML dijagrami i arhitektura)
- **Alati:** Git, GitHub Wiki (tehnička dokumentacija), GitHub Projects (Kanban)

---

## 🚀 Funkcionalnosti i Moj Doprinos

Sustav je podijeljen na module, a u tablici ispod istaknute su funkcionalnosti s naglaskom na module koje sam samostalno dizajnirao i programirao:

| Oznaka | Naziv | Kratki opis | Razvijatelj |
| :--- | :--- | :--- | :--- |
| **F07** | **Generiranje izvještaja o napretku** | *Razvoj sustava za analizu promjena u težini, postotku masnoće i performansama. Implementacija filtriranja po vremenskom periodu i klijentu uz vizualni prikaz kroz grafove, tablice te izvoz u PDF format.* | **Dorian Šturbek** |
| **F09** | **Pregled povratnih informacija** | *Sučelje za pregled tekstualnih komentara i feedbacka klijenata na treninge i obroke, sinkronizirano s imenom klijenta i datumom.* | **Dorian Šturbek** |
| **F11** | **Upravljanje planovima prehrane** | *CRUD operacije nad bazom recepata (sastojci, makronutrijenti, upute za pripremu) te logika za dodjeljivanje specifičnih planova pojedinačnim klijentima ili grupama.* | **Dorian Šturbek** |
| F01 | Login/Registracija | Autentifikacija i registracija trenera u sustav. | Timski rad |
| F02 | Uređivanje profila trenera | Upravljanje kvalifikacijama, opisom i osobnim podacima trenera. | Timski rad |
| F03 | Upravljanje klijentima | Dodavanje, ažuriranje i pregled relevantnih podataka o klijentima. | Timski rad |
| F04 | Upravljanje obavijestima | Prijem i obrada notifikacija o uspješno obavljenim treninzima klijenata. | Timski rad |
| F05 | Postavljanje ciljeva klijentu | Izračun BMR i TDEE (Mifflin–St Jeor formula) te definiranje kalorijskih potreba. | Timski rad |
| F06 | Plan treninga klijenta | Kreiranje i uređivanje specifičnih planova vježbanja po klijentu. | Timski rad |
| F08 | Predefinirani planovi vježbanja | Izrada i učitavanje višekratno iskoristivih predložaka treninga. | Timski rad |
| F10 | Upravljanje bazom hrane | Globalno proširivanje baze namirnica unosom nutritivnih vrijednosti. | Timski rad |
| F12 | Chat s klijentom | Komunikacija s klijentima i dohvat poruka putem eksternog web servisa. | Timski rad |

---

## 🔒 Ključne tehničke značajke mog koda

- **Napredna LINQ Analitika:** Za potrebe generiranja izvještaja (F07) implementirao sam kompleksne LINQ upite za agregaciju i transformaciju povijesnih podataka klijenta iz dislocirane SQL baze u formate pogodne za UI grafikone.
- **WPF Data Binding & DataTemplates:** Modul za prehranu (F11) koristi reaktivno povezivanje podataka (Data Binding) kako bi se makronutrijenti dinamički preračunavali na ekranu ovisno o promjeni količine sastojaka u receptu.
- **Skalabilna arhitektura (Separation of Concerns):** Kroz strogo poštivanje troslojne arhitekture osigurao sam da moje servisne klase u BLL-u obrađuju logiku izvještaja, dok DAL projekt rješava transakcije s bazom podataka, sprječavajući curenje SQL logike u prezentacijski sloj.

---

## 📁 Struktura Visual Studio rješenja (Solution)

```text
├── Zeon.Presentation/   # WPF Projekt: XAML i Code-behind (UI sloj)
├── Zeon.BusinessLogic/  # Class Library: Servisi, entiteti i validacija (Poslovna logika)
├── Zeon.DataAccess/     # Class Library: DbContext, Repozitoriji i Sučelja (Podatkovni sloj)
└── Slike/               # Dijagrami arhitekture i screenshotovi aplikacije
