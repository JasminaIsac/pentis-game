# Specificația Cerințelor Software (SRS)
## Joc Tetris — C# / .NET / Windows Forms

| | |
|---|---|
| **Versiune document** | 1.0 |
| **Data** | 11.11.2026 |
| **Echipa** | 3 membri (vezi secțiunea 9) |
| **Tehnologii** | C#, .NET (6/7/8), Windows Forms (GDI+) |
| **Stare** | Ciornă pentru aprobare internă |

---

## 1. Introducere

### 1.1 Scopul documentului
Acest document descrie complet cerințele funcționale și nefuncționale ale aplicației **Tetris**, un joc desktop pentru Windows. Documentul servește drept referință comună pentru cei trei membri ai echipei în proiectare, implementare, testare și predare.

### 1.2 Domeniul de aplicare
Aplicația este un joc single-player de tip Tetris: piese geometrice (tetromino) cad într-un puț de joc, iar jucătorul le deplasează și le rotește pentru a completa linii orizontale, care dispar și aduc puncte. Jocul se termină când piesele ating partea de sus a puțului.

Aplicația **nu** include: multiplayer în rețea, conturi online, sunet generat dinamic complex sau portare pe alte platforme.

### 1.3 Definiții, acronime și abrevieri

| Termen | Semnificație |
|---|---|
| SRS | Software Requirements Specification |
| Tetromino | Piesă formată din 4 pătrate unite (I, O, T, S, Z, J, L) |
| Puț (board) | Grila de joc, 10 coloane × 20 rânduri vizibile |
| Linie completă | Rând în care toate cele 10 celule sunt ocupate |
| Hard drop | Căderea instantanee a piesei până la bază |
| Soft drop | Accelerarea căderii cât timp tasta este apăsată |
| Ghost piece | Umbra piesei curente, arătând unde va ateriza |
| Tick | Un pas de cădere al piesei în bucla jocului |
| GDI+ | Biblioteca grafică 2D folosită de Windows Forms |

### 1.4 Referințe
- Ghidul oficial Tetris (reguli generale: Super Rotation System, scoring clasic)
- Documentația Microsoft pentru .NET și Windows Forms
- IEEE Std 830-1998 (structura documentului SRS)

### 1.5 Structura documentului
Secțiunea 2 oferă o privire de ansamblu; secțiunea 3 listează cerințele funcționale; secțiunea 4 interfețele; secțiunea 5 cerințele nefuncționale; secțiunile 6–8 arhitectura, datele și testarea; secțiunea 9 organizarea echipei și planul.

---

## 2. Descriere generală

### 2.1 Perspectiva produsului
Aplicație desktop autonomă, rulată local, fără server. Starea persistentă (scorurile maxime și setările) se salvează în fișiere locale.

### 2.2 Funcțiile principale ale produsului
1. Joc Tetris clasic cu 7 tipuri de piese.
2. Sistem de scor, niveluri și viteză progresivă.
3. Previzualizarea piesei următoare și funcția Hold.
4. Pauză, repornire și ieșire.
5. Clasament local (High Scores).
6. Setări (taste, sunet, ghost piece, temă).

### 2.3 Clase de utilizatori

| Utilizator | Descriere | Cerințe speciale |
|---|---|---|
| Jucător ocazional | Persoană care joacă pentru distracție | Reguli simple, comenzi intuitive |
| Jucător avansat | Urmărește scoruri mari | Control precis, răspuns rapid la taste |
| Evaluator / profesor | Verifică proiectul | Cod clar, documentație, funcționalitate completă |

### 2.4 Mediul de operare
- Windows 10 / 11 (64-bit)
- .NET Runtime / Desktop Runtime 6.0 sau mai nou
- Rezoluție minimă: 1024×768

### 2.5 Constrângeri de proiectare și implementare
- Limbaj: C#; framework: .NET; interfață: Windows Forms (fără WPF, fără motoare de joc externe).
- Randarea se face prin GDI+ (`Graphics`, `Paint`) cu **double buffering** activat.
- Logica jocului trebuie separată de interfață (principiul separării responsabilităților).
- Controlul versiunilor: Git.

### 2.6 Presupuneri și dependențe
- Utilizatorul are tastatură funcțională.
- Nu sunt necesare biblioteci externe în afara celor incluse în .NET (opțional: NUnit/xUnit pentru teste).

---

## 3. Cerințe funcționale

Prioritate: **M** = obligatoriu (Must), **S** = recomandat (Should), **C** = opțional (Could).

### 3.1 Meniul principal

| ID | Cerință | Prior. |
|---|---|---|
| FR-01 | La pornire aplicația afișează un meniu cu: *Joc nou*, *Clasament*, *Setări*, *Despre*, *Ieșire*. | M |
| FR-02 | Meniul poate fi navigat cu mouse-ul și cu tastatura (săgeți + Enter). | S |
| FR-03 | Opțiunea *Ieșire* cere confirmare dacă există un joc în desfășurare. | S |
| FR-04 | Ecranul *Despre* afișează numele echipei, versiunea și instrucțiunile de joc. | C |

### 3.2 Puțul de joc și piesele

| ID | Cerință | Prior. |
|---|---|---|
| FR-10 | Puțul are 10 coloane și 20 de rânduri vizibile (opțional 2 rânduri ascunse deasupra pentru apariția pieselor). | M |
| FR-11 | Există 7 tipuri de piese: I, O, T, S, Z, J, L, fiecare cu o culoare distinctă. | M |
| FR-12 | Piesele noi apar centrat în partea de sus a puțului. | M |
| FR-13 | Piesele sunt generate prin sistemul **7-bag** (se amestecă cele 7 piese, se extrag pe rând, apoi se reamestecă), pentru distribuție echitabilă. | S |
| FR-14 | Piesa curentă cade automat cu un pas la fiecare tick, în funcție de nivel. | M |
| FR-15 | Când piesa nu mai poate coborî, se fixează (lock) în puț, iar o nouă piesă apare. | M |
| FR-16 | Dacă piesa nouă nu poate fi plasată (spațiu ocupat), jocul se încheie (Game Over). | M |

### 3.3 Controlul piesei

| ID | Cerință | Prior. |
|---|---|---|
| FR-20 | Mutare stânga/dreapta cu săgețile ← → (cu repetare automată la apăsare lungă). | M |
| FR-21 | Rotire în sens orar cu ↑ sau X; opțional rotire antiorară cu Z. | M |
| FR-22 | Soft drop cu ↓ (cădere accelerată; +1 punct pe celulă). | M |
| FR-23 | Hard drop cu Spațiu (cădere instantanee; +2 puncte pe celulă). | S |
| FR-24 | Mutările și rotațiile care ar produce coliziune cu pereții sau cu piesele fixate sunt refuzate. | M |
| FR-25 | Rotirea implementează *wall kick* simplu (încercarea de deplasare laterală cu 1–2 celule când rotirea directă e blocată). | S |
| FR-26 | Funcția **Hold** (tasta C sau Shift): piesa curentă se stochează și se înlocuiește cu piesa reținută; permisă o singură dată pe piesă. | S |
| FR-27 | Tastele pot fi reconfigurate din Setări. | C |

### 3.4 Eliminarea liniilor, scor și niveluri

| ID | Cerință | Prior. |
|---|---|---|
| FR-30 | O linie completă este eliminată, iar rândurile de deasupra coboară. | M |
| FR-31 | Se pot elimina simultan 1, 2, 3 sau 4 linii (Tetris). | M |
| FR-32 | Scorul pentru eliminare se calculează după formula: 1 linie = 100 × nivel, 2 = 300 × nivel, 3 = 500 × nivel, 4 = 800 × nivel. | M |
| FR-33 | Nivelul crește la fiecare 10 linii eliminate. | M |
| FR-34 | Viteza de cădere crește odată cu nivelul (interval inițial 800 ms, scăzând cu ~10% pe nivel, minim 100 ms). | M |
| FR-35 | Pe ecran se afișează permanent: scor, nivel, număr de linii. | M |
| FR-36 | Eliminarea liniilor este însoțită de o scurtă animație (clipire de ~200 ms). | C |

### 3.5 Previzualizare, ghost piece

| ID | Cerință | Prior. |
|---|---|---|
| FR-40 | Un panou lateral arată următoarea piesă (opțional următoarele 3). | M |
| FR-41 | Un panou lateral arată piesa din Hold. | S |
| FR-42 | **Ghost piece**: conturul/umbra piesei curente în poziția de aterizare; activabil/dezactivabil din Setări. | S |

### 3.6 Stările jocului

| ID | Cerință | Prior. |
|---|---|---|
| FR-50 | Tasta P / Esc pune jocul pe pauză și afișează un mesaj „Pauză”; reapăsarea reia jocul. | M |
| FR-51 | La Game Over se afișează scorul final, nivelul și opțiunile *Joacă din nou* / *Meniu*. | M |
| FR-52 | Tasta R (sau butonul *Restart*) repornește jocul în orice moment, cu confirmare. | S |
| FR-53 | Jocul se pune automat pe pauză când fereastra pierde focusul. | C |

### 3.7 Clasament (High Scores)

| ID | Cerință | Prior. |
|---|---|---|
| FR-60 | Se păstrează top 10 scoruri locale (nume jucător, scor, nivel, dată). | M |
| FR-61 | Dacă scorul final intră în top 10, jucătorul este invitat să-și introducă numele (max. 12 caractere). | M |
| FR-62 | Clasamentul se salvează într-un fișier JSON și se încarcă la pornire. | M |
| FR-63 | Ecranul *Clasament* afișează lista sortată descrescător după scor. | M |

### 3.8 Setări

| ID | Cerință | Prior. |
|---|---|---|
| FR-70 | Setări disponibile: volum/sunet on-off, ghost piece on/off, nivel de start (1–10), temă de culori. | S |
| FR-71 | Setările se salvează automat și se aplică la următoarea pornire. | S |
| FR-72 | Este disponibilă opțiunea *Resetare la valorile implicite*. | C |

### 3.9 Sunet

| ID | Cerință | Prior. |
|---|---|---|
| FR-80 | Efecte sonore pentru: mutare, rotire, fixare, eliminare linie, Game Over. | C |
| FR-81 | Muzică de fundal opțională, cu posibilitate de dezactivare. | C |

---

## 4. Cerințe de interfață externă

### 4.1 Interfața cu utilizatorul

**Fereastra principală** (dimensiune fixă ~ 600×720 px, nu redimensionabilă):

```
+-------------------------------------------------+
|  TETRIS                                         |
|  +-----------------+   +---------------------+  |
|  |                 |   | NEXT                |  |
|  |                 |   |  [ piesa urm. ]     |  |
|  |     PUȚUL       |   | HOLD                |  |
|  |    10 x 20      |   |  [ piesa reținută ] |  |
|  |                 |   | SCOR:   000000      |  |
|  |                 |   | NIVEL:  1           |  |
|  |                 |   | LINII:  0           |  |
|  +-----------------+   +---------------------+  |
+-------------------------------------------------+
```

- Celula are 30×30 px; puțul are 300×600 px.
- Culori standard pentru piese: I – cyan, O – galben, T – mov, S – verde, Z – roșu, J – albastru, L – portocaliu.
- Fundalul puțului este închis, cu grilă discretă.
- Formularele secundare (Clasament, Setări, Despre) sunt ferestre modale (`ShowDialog`).

### 4.2 Interfețe hardware
Tastatură (obligatoriu). Mouse pentru meniuri. Ieșire audio (opțional).

### 4.3 Interfețe software
- .NET Desktop Runtime.
- Sistemul de fișiere local pentru `highscores.json` și `settings.json` (în `%AppData%\TetrisGame\`).
- `System.Text.Json` pentru serializare.
- `System.Media.SoundPlayer` sau `NAudio` (opțional) pentru sunet.

### 4.4 Interfețe de comunicații
Nu există comunicare în rețea.

### 4.5 Comenzi de la tastatură (implicite)

| Acțiune | Tastă |
|---|---|
| Stânga / Dreapta | ← / → |
| Rotire orară | ↑ sau X |
| Rotire antiorară | Z |
| Soft drop | ↓ |
| Hard drop | Spațiu |
| Hold | C sau Shift |
| Pauză | P sau Esc |
| Restart | R |

---

## 5. Cerințe nefuncționale

### 5.1 Performanță
- **NFR-01**: Jocul rulează la minimum 60 FPS pe un calculator cu 4 GB RAM și procesor dual-core.
- **NFR-02**: Latența între apăsarea unei taste și reacția vizibilă a piesei: sub 50 ms.
- **NFR-03**: Timpul de pornire al aplicației: sub 3 secunde.
- **NFR-04**: Consumul de memorie: sub 150 MB.

### 5.2 Fiabilitate
- **NFR-05**: Aplicația nu se închide neașteptat; erorile de citire/scriere a fișierelor sunt tratate (se folosesc valori implicite și se afișează un mesaj).
- **NFR-06**: La fișiere JSON corupte, aplicația le regenerează fără a se bloca.

### 5.3 Utilizabilitate
- **NFR-07**: Un utilizator nou înțelege comenzile în maximum 1 minut (instrucțiuni vizibile în Despre/Ajutor).
- **NFR-08**: Interfața este lizibilă (contrast suficient, font ≥ 12 pt).
- **NFR-09**: Textele interfeței sunt în limba română (opțional: engleză).

### 5.4 Mentenabilitate
- **NFR-10**: Codul respectă convențiile C# (PascalCase pentru tipuri și metode, camelCase pentru variabile locale).
- **NFR-11**: Logica jocului nu depinde de clase Windows Forms (testabilă izolat).
- **NFR-12**: Metodele publice sunt comentate cu XML documentation.
- **NFR-13**: Valorile constante (dimensiuni, viteze, scoruri) sunt definite într-un singur loc.

### 5.5 Portabilitate
- **NFR-14**: Rulează pe Windows 10/11. Nu se garantează funcționarea pe Linux/macOS.

### 5.6 Securitate
- **NFR-15**: Aplicația nu cere privilegii de administrator și scrie doar în directorul propriu din `%AppData%`.

---

## 6. Arhitectura sistemului

### 6.1 Structura pe straturi

```
+----------------------------------------------+
| Prezentare (Windows Forms)                    |
|  MainMenuForm, GameForm, HighScoresForm,      |
|  SettingsForm, AboutForm, BoardRenderer       |
+----------------------------------------------+
| Logica jocului (Core)                         |
|  GameEngine, Board, Tetromino, PieceFactory,  |
|  ScoreManager, InputHandler                   |
+----------------------------------------------+
| Date / Persistență                            |
|  HighScoreRepository, SettingsRepository      |
+----------------------------------------------+
```

### 6.2 Structura soluției (Visual Studio)

```
TetrisGame.sln
├── TetrisGame.Core/        (class library: logica)
│   ├── Board.cs
│   ├── Tetromino.cs
│   ├── TetrominoType.cs
│   ├── PieceFactory.cs
│   ├── GameEngine.cs
│   ├── ScoreManager.cs
│   └── GameState.cs
├── TetrisGame.Data/        (persistență)
│   ├── HighScoreRepository.cs
│   ├── SettingsRepository.cs
│   └── Models/ (HighScoreEntry.cs, GameSettings.cs)
├── TetrisGame.UI/          (Windows Forms)
│   ├── Program.cs
│   ├── Forms/ (MainMenuForm, GameForm, HighScoresForm, SettingsForm, AboutForm)
│   └── Rendering/BoardRenderer.cs
└── TetrisGame.Tests/       (teste unitare)
```

### 6.3 Clasele principale

| Clasă | Responsabilitate |
|---|---|
| `Board` | Matrice `int[20,10]`; verifică coliziuni, fixează piese, detectează și elimină linii |
| `Tetromino` | Tipul, forma (matrice), rotația, poziția (x, y); metode `Move`, `Rotate`, `Clone` |
| `PieceFactory` | Generează piese prin 7-bag |
| `GameEngine` | Bucla jocului: tick, gravitație, gestionarea stărilor, evenimente |
| `ScoreManager` | Scor, nivel, linii, viteza curentă |
| `BoardRenderer` | Desenează puțul, piesele, ghost, panourile laterale folosind `Graphics` |
| `GameForm` | Preia tastele, pornește `Timer`-ul, apelează `GameEngine` și `Invalidate()` |
| `HighScoreRepository` | Încarcă/salvează top 10 în JSON |
| `SettingsRepository` | Încarcă/salvează setările |

### 6.4 Evenimente (decuplare Core ↔ UI)
`GameEngine` expune evenimentele: `PieceLocked`, `LinesCleared(int count)`, `LevelChanged`, `ScoreChanged`, `GameOver`. UI-ul se abonează și reacționează (redesenare, sunet, animații).

### 6.5 Bucla jocului
- Un `System.Windows.Forms.Timer` (~16 ms) pentru redesenare și intrare.
- Un contor de timp acumulat declanșează gravitația când se depășește intervalul nivelului curent.
- Desenarea se face în `OnPaint` / evenimentul `Paint` cu `DoubleBuffered = true`.

### 6.6 Diagramă de stări a jocului

```
[Meniu] --Joc nou--> [Joc activ] --P--> [Pauză] --P--> [Joc activ]
                         |
                    piesă imposibil de plasat
                         v
                    [Game Over] --Joacă din nou--> [Joc activ]
                         |--Meniu--> [Meniu]
```

---

## 7. Modelul datelor

### 7.1 Piesele (forme de bază, rotația 0)

```
I:  ....    O:  ##    T:  .#.    S:  .##    Z:  ##.    J:  #..    L:  ..#
    ####        ##        ###        ##.        .##        ###        ###
```

Fiecare piesă este reprezentată ca matrice pătratică (4×4 pentru I, 2×2 pentru O, 3×3 pentru restul); rotația se obține prin transpunere + inversare de rânduri.

### 7.2 Fișier `highscores.json`

```json
[
  { "name": "Ana", "score": 12500, "level": 5, "date": "2026-11-11T18:30:00" }
]
```

### 7.3 Fișier `settings.json`

```json
{
  "soundEnabled": true,
  "ghostEnabled": true,
  "startLevel": 1,
  "theme": "dark",
  "keys": { "left": "Left", "right": "Right", "rotate": "Up",
            "softDrop": "Down", "hardDrop": "Space", "hold": "C", "pause": "P" }
}
```

---

## 8. Verificare și validare

### 8.1 Teste unitare (pentru `Core`)
- Coliziunea piesei cu pereții și cu piesele fixate.
- Rotația fiecărui tip de piesă (4 rotații = poziția inițială).
- Eliminarea corectă a 1, 2, 3, 4 linii și coborârea rândurilor de deasupra.
- Calculul scorului și al nivelului.
- Generatorul 7-bag: în fiecare grup de 7 piese apar toate cele 7 tipuri.
- Serializarea/deserializarea scorurilor și setărilor.

### 8.2 Teste manuale (exemple)

| ID | Scenariu | Rezultat așteptat |
|---|---|---|
| T-01 | Pornire aplicație | Se afișează meniul principal |
| T-02 | Joc nou, apăsare ← la marginea puțului | Piesa nu iese din puț |
| T-03 | Completare o linie | Linia dispare, scorul crește cu 100 |
| T-04 | Completare 4 linii cu piesa I | Scor +800 × nivel |
| T-05 | Apăsare P în timpul jocului | Jocul se oprește; reapăsarea îl reia |
| T-06 | Umplerea puțului | Apare Game Over cu scorul final |
| T-07 | Scor în top 10 | Se cere numele și se salvează în clasament |
| T-08 | Ștergerea `highscores.json` | Aplicația pornește fără eroare, clasament gol |
| T-09 | Hard drop | Piesa aterizează instant, scor +2 per celulă |
| T-10 | Hold de două ori consecutiv | A doua oară este ignorat |

### 8.3 Criterii de acceptare
Aplicația este acceptată dacă toate cerințele de prioritate **M** sunt implementate și testate, iar testele T-01…T-10 trec fără erori.

---

## 9. Organizarea echipei și planificare

### 9.1 Roluri (propunere pentru 3 persoane)

| Membru | Rol | Responsabilități principale |
|---|---|---|
| **Membru 1** | Logica jocului (Core) | `Board`, `Tetromino`, `PieceFactory`, `GameEngine`, `ScoreManager`, teste unitare |
| **Membru 2** | Interfața grafică (UI) | `MainMenuForm`, `GameForm`, `BoardRenderer`, desenare, animații, tastatură |
| **Membru 3** | Date, setări, integrare | `HighScoreRepository`, `SettingsRepository`, `SettingsForm`, `HighScoresForm`, sunet, documentație, QA |

Toți membrii participă la revizuirea codului (code review) prin pull request-uri și la testarea finală.

### 9.2 Planul etapelor

| Etapa | Conținut | Durată estimată |
|---|---|---|
| 1 | Configurarea soluției, repository Git, structura proiectelor; mock UI | 2–3 zile |
| 2 | `Board`, `Tetromino`, coliziuni, mutare, rotație; desenarea puțului | 1 săptămână |
| 3 | Gravitație, fixare, eliminare linii, scor, niveluri; panouri laterale | 1 săptămână |
| 4 | Hold, ghost, hard drop, pauză, Game Over; meniul principal | 1 săptămână |
| 5 | Clasament, setări, persistență JSON, sunet | 1 săptămână |
| 6 | Teste, remedierea erorilor, optimizare, documentație, predare | 3–5 zile |

### 9.3 Fluxul de lucru Git
- Ramuri: `main` (stabil), `develop`, câte o ramură per funcționalitate (`feature/board`, `feature/ui-menu` etc.).
- Commit-uri mici, cu mesaje clare; merge în `develop` doar după revizuirea unui coleg.

### 9.4 Riscuri

| Risc | Impact | Măsură |
|---|---|---|
| Palpâire/flicker la desenare | Mediu | Activare `DoubleBuffered`, desen într-un singur `Paint` |
| Bucla jocului prea lentă sau neregulată | Mediu | Timp acumulat (delta time), nu intervale fixe de gravitație |
| Conflicte la merge | Mic | Împărțirea pe proiecte/clase, merge frecvent |
| Rotații incorecte lângă pereți | Mediu | Wall kick + teste unitare |
| Depășirea termenului | Mare | Prioritizare MoSCoW: întâi cerințele M |

---

## 10. Extensii posibile (în afara domeniului curent)
- Mod două persoane pe același calculator.
- Mod Sprint (40 de linii) și Ultra (2 minute).
- Linii de gunoi (garbage lines) și mod Versus contra calculatorului.
- Salvarea replay-urilor.
- Teme grafice suplimentare.

---

## 11. Anexe

### 11.1 Valori implicite recomandate

| Parametru | Valoare |
|---|---|
| Dimensiune celulă | 30 px |
| Puț | 10 × 20 |
| Interval gravitație nivel 1 | 800 ms |
| Linii pe nivel | 10 |
| Soft drop | +1 punct / celulă |
| Hard drop | +2 puncte / celulă |
| Lungime nume în clasament | max. 12 caractere |

### 11.2 Istoricul versiunilor

| Versiune | Data | Autor | Modificări |
|---|---|---|---|
| 1.0 | 11.11.2026 | Echipa | Prima versiune completă |
