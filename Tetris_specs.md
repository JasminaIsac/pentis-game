# Specificații de implementare — Tetris

| | |
|---|---|
| **Document** | `Tetris_specs.md` |
| **Versiune** | 1.0 |
| **Data** | 11.11.2026 |
| **Bază** | `SRS_Tetris.md` (cerințe) și `Taskuri_Tetris.md` (ordine de lucru) |
| **Platformă** | C# 14, .NET 10 (LTS), Windows Forms |
| **Scop** | Descrie concret ce trebuie să conțină proiectul: soluția, clasele, regulile, interfața, datele și criteriile prin care fiecare parte este acceptată |

Unde SRS-ul lasă intervalul „.NET 6/7/8 sau mai nou”, acest document fixează ținta pe **.NET 10**. Prioritățile rămân cele din SRS: **[M]** obligatoriu, **[S]** recomandat, **[C]** opțional. O parte marcată **[S]** sau **[C]** lipsește din livrarea minimă; dacă este implementată, respectă specificația de mai jos.

Un element este acceptat numai când toate criteriile lui de acceptare sunt adevărate.

---

## 1. Stivă și mediu

| Element | Valoare obligatorie |
|---|---|
| SDK | .NET SDK 10.0.x (`global.json`, `rollForward: latestFeature`) |
| Limbaj | C# 14 (implicit pentru `net10.0`; nu se setează o versiune mai mică) |
| IDE | Visual Studio 2026 18.0 sau mai nou, cu workload-ul **.NET desktop development** și componenta **.NET 10**. Visual Studio 2022 nu poate ținti `net10.0`. Alternativ: SDK-ul .NET 10 din linia de comandă. |
| Aplicație | Windows Forms, desenare GDI+ (`System.Drawing.Graphics`) |
| Biblioteci de joc | Niciun motor extern (fără WPF, fără Unity, fără MonoGame) |
| Teste | xUnit, șablonul `dotnet new xunit` |
| Serializare | `System.Text.Json`, inclus în .NET 10 |
| Runtime la rulare | .NET Desktop Runtime 10, Windows 10/11 pe 64 de biți |
| Rezoluție minimă | 1024×768 |

`TetrisGame.Core` și `TetrisGame.Data` țintesc `net10.0`. `TetrisGame.UI` țintește `net10.0-windows` și are `<UseWindowsForms>true</UseWindowsForms>`. `TetrisGame.Tests` țintește `net10.0`.

Proiectele sunt SDK-style. În `Directory.Build.props`, la nivelul soluției:

- `<Nullable>enable</Nullable>`
- `<ImplicitUsings>enable</ImplicitUsings>`
- `<LangVersion>14.0</LangVersion>`

`Core` nu referențiază `System.Windows.Forms` și nu referențiază `System.Drawing`. Culoarea din logică este un identificator numeric; culoarea GDI+ există doar în `UI`.

**Criterii de acceptare**

- `dotnet --list-sdks` arată un SDK 10.0, iar `global.json` nu lasă soluția să fie compilată cu un SDK 8 sau 9.
- `dotnet build TetrisGame.slnx` reușește fără erori și fără avertismente de nullable nejustificate.
- Referințele sunt doar: `UI` → `Core`, `Data`; `Data` → `Core`; `Tests` → `Core`, `Data`. `Core` nu depinde de niciun alt proiect.
- Căutarea în `TetrisGame.Core` nu găsește `System.Windows.Forms` sau `System.Drawing`.

---

## 2. Conținutul repository-ului

```
TetrisGame.slnx
global.json
Directory.Build.props
.gitignore
.editorconfig
README.md
SRS_Tetris.md
TetrisGame.Core/
TetrisGame.Data/
TetrisGame.UI/
TetrisGame.Tests/
```

Soluția se creează cu `dotnet new sln -n TetrisGame`. Pe SDK-ul .NET 10 formatul implicit este `.slnx`. Nu se folosește șablonul vechi **Windows Forms App (.NET Framework)**.

`.gitignore` provine din `dotnet new gitignore` și exclude `bin/`, `obj/`, `.vs/`. `.editorconfig` provine din `dotnet new editorconfig` (PascalCase pentru tipuri și membri publici, camelCase pentru locale și parametri).

`README.md` conține: descrierea jocului, cerința de .NET Desktop Runtime 10, pașii `dotnet build` și `dotnet run --project TetrisGame.UI`, tabelul de taste, structura soluției.

### 2.1 Fișierele din `TetrisGame.Core`

| Fișier | Tip |
|---|---|
| `GameConstants.cs` | clasă statică |
| `TetrominoType.cs` | enum |
| `TetrominoCatalog.cs` | formele și identificatorii de culoare |
| `Tetromino.cs` | piesa |
| `Board.cs` | puțul |
| `PieceFactory.cs` | generator 7-bag |
| `ScoreManager.cs` | scor, nivel, viteză |
| `GameState.cs` | enum |
| `GameEngine.cs` | stări, gravitație, comenzi |

Spațiul de nume este `TetrisGame.Core`, file-scoped. Fiecare tip public are `/// <summary>`.

### 2.2 Fișierele din `TetrisGame.Data`

| Fișier | Tip |
|---|---|
| `Models/HighScoreEntry.cs` | model |
| `Models/GameSettings.cs` | model |
| `AppJsonContext.cs` | `JsonSerializerContext` parțial, source generator |
| `HighScoreRepository.cs` | persistență scoruri **[M]** |
| `SettingsRepository.cs` | persistență setări **[S]** |

Spațiul de nume pentru modele este `TetrisGame.Data.Models`.

### 2.3 Fișierele din `TetrisGame.UI`

| Fișier | Rol |
|---|---|
| `Program.cs` | punct de intrare |
| `Forms/MainMenuForm.cs` | meniu **[M]** |
| `Forms/GameForm.cs` | fereastra de joc **[M]** |
| `Forms/HighScoresForm.cs` | clasament **[M]** |
| `Forms/SettingsForm.cs` | setări **[S]** |
| `Forms/AboutForm.cs` | despre și taste **[C]** |
| `Forms/NameEntryForm.cs` | dialogul de nume la high score **[M]** |
| `Rendering/BoardRenderer.cs` | desenarea puțului **[M]** |
| `Rendering/Theme.cs` | paleta de culori **[C]**, cu o temă întunecată prezentă oricum în **[M]** |
| `Audio/SoundManager.cs` | efecte **[C]** |

`Program.cs` apelează `ApplicationConfiguration.Initialize()`, apoi `Application.Run(new MainMenuForm())`. Proiectul de pornire este `TetrisGame.UI`.

### 2.4 Fișierele din `TetrisGame.Tests`

| Fișier | Acoperă |
|---|---|
| `TetrominoTests.cs` | rotație, celule, clonă |
| `BoardTests.cs` | limite, coliziuni, lock |
| `LineClearTests.cs` | 1–4 linii și coborârea rândurilor |
| `ScoreManagerTests.cs` | puncte, nivel, interval, drop |
| `PieceFactoryTests.cs` | 7-bag cu seed fix |
| `GameEngineTests.cs` | gravitație, game over, hold |
| `RepositoryTests.cs` | JSON lipsă, valid și corupt |

**Criterii de acceptare**

- Arborele de mai sus există, iar numele proiectelor sunt exact cele din tabele.
- `dotnet run --project TetrisGame.UI` deschide meniul principal.
- O persoană din afara echipei poate compila și porni jocul urmând doar `README.md`.

---

## 3. Constante

`GameConstants` este singurul loc în care apar valorile numerice de joc. Niciun alt fișier nu reintroduce aceste numere ca literali.

| Constantă | Valoare | Rol |
|---|---|---|
| `BoardWidth` | 10 | coloane |
| `BoardHeight` | 20 | rânduri vizibile |
| `HiddenRows` | 2 | rânduri deasupra zonei vizibile, pentru apariția piesei |
| `CellSize` | 30 | latura celulei, în pixeli |
| `InitialDropIntervalMs` | 800 | intervalul de cădere la nivel 1 |
| `MinDropIntervalMs` | 100 | plafonul de viteză |
| `LinesPerLevel` | 10 | linii eliminate pentru un nivel nou |
| `SoftDropPointsPerCell` | 1 | |
| `HardDropPointsPerCell` | 2 | |
| `MaxNameLength` | 12 | |
| `MaxHighScores` | 10 | |
| `LineClearScores` | `{ 0, 100, 300, 500, 800 }` | indexul este numărul de linii eliminate |
| `StartLevelMin` / `StartLevelMax` | 1 / 10 | |
| `DasDelayMs` | 170 | întârzierea până la repetarea săgeților |
| `DasRepeatMs` | 50 | perioada repetării |
| `LineClearFlashMs` | 200 | animația opțională |

**Criterii de acceptare**

- Toate constantele de mai sus există, sunt `const` sau `static readonly`, și au documentație XML.
- O căutare după `800`, `100`, `300`, `500` în afara `GameConstants` și a testelor nu găsește praguri de joc scrise direct.

---

## 4. Piesele

### 4.1 Tipuri și culori

```csharp
enum TetrominoType { I, O, T, S, Z, J, L }
```

În matricea puțului, `0` înseamnă celulă liberă. O celulă ocupată păstrează `(int)tip + 1`:

| Tip | Id în grid | Culoare în UI |
|---|---|---|
| I | 1 | cyan |
| O | 2 | galben |
| T | 3 | mov |
| S | 4 | verde |
| Z | 5 | roșu |
| J | 6 | albastru |
| L | 7 | portocaliu |

`TetrominoCatalog` expune, pentru orice tip: matricea de rotație 0, dimensiunea ei și id-ul de culoare. UI-ul traduce id-ul în `Color` prin `Theme`. Catalogul nu depinde de `System.Drawing`.

### 4.2 Formele la rotația 0

`#` este celulă ocupată. Originea matricei este colțul din stânga-sus.

```
I (4×4)          O (2×2)     T (3×3)      S (3×3)
. . . .          # #         . # .        . # #
# # # #          # #         # # #        # # .
. . . .                      . . .        . . .
. . . .

Z (3×3)          J (3×3)     L (3×3)
# # .            # . .       . . #
. # #            # # #       # # #
. . .            . . .       . . .
```

### 4.3 `Tetromino`

| Membru | Contract |
|---|---|
| `Type` | tipul piesei, neschimbat de rotație |
| `Shape` | matricea curentă; o rotație înlocuiește matricea, nu o modifică pe cea a catalogului |
| `X`, `Y` | colțul stânga-sus al matricei, în coordonate de puț |
| `Rotation` | 0, 1, 2 sau 3 |
| `RotateClockwise()` | transpunere, apoi inversarea fiecărui rând; `Rotation` crește cu 1 modulo 4 |
| `RotateCounterClockwise()` | inversul operației de mai sus; `Rotation` scade cu 1 modulo 4 |
| `Move(dx, dy)` | adună deplasarea la `X` și `Y` |
| `Clone()` | copie cu matrice proprie |
| `GetCells()` | exact cele 4 celule ocupate, ca perechi `(X + coloană, Y + rând)` |

Piesa nu cunoaște puțul, scorul sau tastatura.

Poziția de apariție, calculată de `PieceFactory`: `Y = 0` (începe în rândurile ascunse), `X = (BoardWidth - lățimeaMatricei) / 2` (împărțire întreagă). Rezultă `X = 3` pentru I și pentru piesele 3×3, `X = 4` pentru O. Rotația inițială este 0.

**Criterii de acceptare**

- Pentru fiecare dintre cele 7 tipuri, patru rotații orare readuc matricea la forma din secțiunea 4.2. Același lucru pentru patru rotații antiorare.
- `GetCells()` are lungimea 4 pentru orice tip și orice rotație.
- După `Clone()`, o mutare sau o rotație pe copie lasă originalul neschimbat, inclusiv conținutul matricei.
- Forma I orizontală ocupă un singur rând al matricei; forma O rămâne neschimbată după rotație.

---

## 5. Puțul (`Board`)

Grila este `int[BoardHeight + HiddenRows, BoardWidth]`, adică 22×10. Rândul `0` este marginea de sus. Rândurile `0` și `1` sunt ascunse. Rândurile `2`…`21` sunt vizibile. Coloana `0` este marginea din stânga.

| Metodă | Comportament |
|---|---|
| `IsInside(x, y)` | adevărat doar pentru `x` în `[0, 10)` și `y` în `[0, 22)` |
| `IsCellFree(x, y)` | celula este în interior și are valoarea 0 |
| `IsValidPosition(piece)` | toate cele 4 celule din `GetCells()` sunt în interior și libere |
| `LockPiece(piece)` | scrie id-ul piesei în cele 4 celule; se apelează doar pentru o poziție validă |
| `GetCell(x, y)` | valoarea din grilă, pentru desenare |
| `Clear()` | toate celulele devin 0 |
| `FindFullRows()` | indicii rândurilor în care toate cele 10 celule sunt nenule |
| `ClearRows(rows)` | șterge rândurile primite și coboară blocul de deasupra fiecărei goluri; sus se inserează rânduri goale |
| `ClearFullLines()` | `FindFullRows` urmat de `ClearRows`; întoarce numărul de linii, între 0 și 4 |

O celulă din rândurile ascunse participă la coliziune și la lock. Nu este desenată. O linie completă aflată în rândurile ascunse se elimină la fel ca una vizibilă.

`ClearRows` păstrează ordinea rândurilor rămase. Două linii pline separate de o linie incompletă nu fac ca linia incompletă să se amestece: ea coboară cu numărul de linii pline de sub ea.

**Criterii de acceptare**

- O piesă cu o celulă la `x = -1` sau `x = 10` este invalidă. O celulă la `y = 22` este invalidă.
- O piesă ale cărei celule ocupate sunt toate în rândurile 0–1, pe celule libere, este validă.
- După `LockPiece`, `GetCell` pe acele coordonate întoarce id-ul piesei, iar restul grilei rămâne neschimbat.
- Eliminarea a 1, 2, 3 și 4 linii lasă grila fără rânduri pline și fără rânduri goale la mijloc, acolo unde deasupra exista conținut.
- Linii pline neadiacente, o linie la baza puțului (`y = 21`) și o linie la mijloc coboară corect conținutul de deasupra.
- `Clear()` după piese blocate produce 22×10 zerouri.

---

## 6. Generatorul 7-bag (`PieceFactory`) **[M]** distribuția, algoritmul **[S]** în SRS

SRS marchează 7-bag ca **[S]**. În acest proiect generatorul este 7-bag, nu alegere independentă, pentru că taskul de logică îl tratează ca parte din motor.

- Sacul pornește cu cele 7 tipuri, amestecate Fisher–Yates folosind `Random` primit în constructor.
- `GetNext()` scoate următorul tip. Sacul gol se reumple și se amestecă din nou înainte de extragere.
- `Peek(count)` arată următoarele `count` tipuri fără să le scoată. Dacă sacul curent nu ajunge, privirea trece în ordinea unui sac următor deja generat, păstrat separat, astfel încât un `GetNext()` ulterior să coincidă cu ce s-a văzut.
- `Create(type)` construiește un `Tetromino` la poziția și rotația inițială din secțiunea 4.3.
- Testele pot pasa `new Random(seed)`.

**Criterii de acceptare**

- Cu orice seed, fiecare grup de 7 piese luate de la începutul unui sac conține fiecare tip exact o dată.
- Două fabrici cu același seed produc aceeași secvență.
- `Peek(3)` urmat de trei `GetNext()` întoarce aceleași trei tipuri, în aceeași ordine.
- Piesa creată are `Y = 0`, rotația 0 și `X` centrat conform secțiunii 4.3.

---

## 7. Scorul (`ScoreManager`)

| Proprietate | La `Reset(startLevel)` |
|---|---|
| `Score` | 0 |
| `Level` | `startLevel` (1–10) |
| `TotalLines` | 0 |
| `DropIntervalMs` | formula de mai jos, pentru nivelul curent |

`AddLineClear(lines)` pentru `lines` între 1 și 4:

1. adaugă `LineClearScores[lines] * Level` la `Score` (nivelul de dinaintea creșterii);
2. adaugă `lines` la `TotalLines`;
3. setează `Level = startLevel + TotalLines / LinesPerLevel`;
4. recalculează intervalul.

`AddSoftDrop(cells)` adaugă `cells * SoftDropPointsPerCell`. `AddHardDrop(cells)` adaugă `cells * HardDropPointsPerCell`. Niciuna nu schimbă liniile sau nivelul.

Intervalul, cu trunchiere către zero:

```text
DropIntervalMs = Max(MinDropIntervalMs,
                     (int)(InitialDropIntervalMs * 0.9 ^ (Level - 1)))
```

Valori de control: nivel 1 → 800 ms, nivel 2 → 720 ms, nivel 3 → 648 ms, nivel 4 → 583 ms. Sub 100 ms nu se coboară.

`ScoreChanged` se ridică doar dacă `Score` s-a schimbat. `LevelChanged` se ridică doar dacă `Level` s-a schimbat. `Reset` setează valorile fără să pretindă un joc anterior.

**Criterii de acceptare**

- La nivel 1, eliminările dau 100, 300, 500 și 800 de puncte.
- La nivel 3, o eliminare de 4 linii dă 2400 de puncte.
- 9 linii urmate de o eliminare de 1 linie lasă nivelul neschimbat în timpul calculului punctelor; nivelul crește după aceea, la a 10-a linie.
- `startLevel = 5` pornește la nivel 5 și trece la 6 după exact 10 linii.
- Soft drop pe 4 celule adaugă 4 puncte. Hard drop pe 4 celule adaugă 8 puncte.
- Evenimentele nu se ridică la o apelare care nu schimbă valoarea.

---

## 8. Motorul (`GameEngine`)

### 8.1 Stări

```csharp
enum GameState { Menu, Playing, Paused, GameOver }
```

Tranziții acceptate:

```text
Menu --StartNewGame--> Playing
Playing --Pause--> Paused --Resume--> Playing
Playing --piesa nouă invalidă--> GameOver
GameOver --StartNewGame--> Playing
Paused --StartNewGame--> Playing
```

`Update` nu modifică nimic când starea nu este `Playing`. Comenzile de mutare, rotație, drop și hold nu au efect în `Paused` sau `GameOver`.

### 8.2 Conținut

`GameEngine` deține `Board`, piesa curentă, `PieceFactory`, `ScoreManager`, starea, acumulatorul de gravitație, `HeldPiece` (`TetrominoType?`) și `CanHold`. Expune tipurile următoare pentru panoul Next, citite prin `Peek`, fără să le scoată din fabrică. Minimum un tip este disponibil; trei sunt ținta pentru panou **[S]** extinderea la trei piese, una este **[M]**.

### 8.3 Gravitație și fixare

`Update(elapsedMs)` adună timpul. Cât acumulatorul este cel puțin `DropIntervalMs`, scade un interval și încearcă o mutare cu `(0, +1)`.

- Mutarea reușește: piesa coboară o celulă, fără puncte.
- Mutarea eșuează: se execută fixarea, iar restul de timp acumulat nu mai mișcă piesa veche.

`LockCurrentPiece`:

1. `Board.LockPiece`;
2. `lines = Board.ClearFullLines()`;
3. dacă `lines > 0`, `ScoreManager.AddLineClear(lines)` și evenimentul `LinesCleared(lines)`;
4. `CanHold = true`;
5. piesa curentă devine următoarea piesă din fabrică, la poziția inițială;
6. dacă poziția ei nu este validă, `State = GameOver` și se ridică `GameOver`.

`PieceLocked` se ridică după scrierea piesei în grilă.

### 8.4 Comenzi

| Metodă | Efect în `Playing` |
|---|---|
| `StartNewGame(startLevel)` | golește puțul, resetează scorul la nivelul dat, sac nou, `CanHold = true`, `HeldPiece = null`, stare `Playing`, prima piesă așezată dacă poziția este validă |
| `Pause()` / `Resume()` | `Playing` ↔ `Paused`; la pauză acumulatorul nu mai crește |
| `MoveLeft()` / `MoveRight()` | `TryMove(-1, 0)` / `TryMove(1, 0)`; eșecul nu fixează piesa |
| `RotateClockwise()` / `RotateCounterClockwise()` | încearcă rotația pe o clonă |
| `SoftDrop()` | o celulă în jos; la succes adaugă 1 punct și pune acumulatorul pe 0; la eșec fixează piesa |
| `HardDrop()` | coboară până la ultima poziție validă, adaugă 2 puncte pe celulă parcursă, apoi fixează imediat; 0 celule înseamnă 0 puncte și fixare |
| `Hold()` | vezi 8.6 **[S]** |

`TryMove` și încercarea de rotație lucrează pe `Clone()`. Poziția reală se schimbă doar dacă `Board.IsValidPosition` este adevărat pentru clonă.

### 8.5 Wall kick **[S]**

Dacă rotația pe loc este invalidă, se încearcă, în ordine, deplasările `(−1, 0)`, `(+1, 0)`, `(−2, 0)`, `(+2, 0)`, `(0, −1)`, aplicate piesei deja rotite. Prima poziție validă este păstrată. Dacă niciuna nu este validă, piesa rămâne nerotită. Aceeași listă se folosește pentru toate tipurile, inclusiv I. O încercare nu este acceptată dacă trece printr-o celulă ocupată: se testează doar poziția finală, iar o poziție finală ocupată este respinsă.

### 8.6 Hold **[S]**

`Hold()` iese imediat dacă starea nu este `Playing` sau dacă `CanHold` este fals.

- Nu există piesă reținută: se reține `Type`-ul piesei curente și se aduce următoarea piesă din fabrică, la poziția inițială.
- Există piesă reținută: tipul curent și tipul reținut se schimbă între ele. Piesa adusă în joc este creată din nou, la rotația 0 și la poziția inițială.

Dacă piesa rezultată nu are poziție validă, `Hold()` nu schimbă nimic. La succes, `CanHold` devine fals până la următoarea fixare.

### 8.7 Poziția de aterizare

`GetDropPosition(piece)` întoarce un clone mutat în jos până când încă o celulă ar fi invalidă. Este folosit de hard drop și de ghost piece. Nu modifică piesa primită și nu scrie în puț.

### 8.8 Evenimente

`PieceLocked`, `LinesCleared(int)`, `LevelChanged`, `ScoreChanged`, `GameOver`, `StateChanged`. UI-ul se abonează la ele. Motorul nu apelează metode din formulare.

**Criterii de acceptare**

- Un test fără UI poate rula `Update` până când o piesă atinge baza, se blochează, iar o linie plină dispare.
- Gravitația nu mută piesa în `Paused` și nu o mută după `GameOver`.
- Nicio secvență de mutări și rotații nu lasă o celulă de piesă în afara grilei sau peste o celulă deja blocată.
- Dacă puțul este umplut astfel încât piesa nouă nu are 4 celule libere, starea devine `GameOver` și piesa nu este scrisă peste celule ocupate.
- `HardDrop` lasă piesa blocată pe aceeași poziție pe care o raportează `GetDropPosition`, iar scorul crește cu `2 * numărulDeCelule`.
- Hold aplicat de două ori fără o fixare între ele lasă a doua comandă fără efect. După o fixare, Hold funcționează din nou.
- Hold nu produce o piesă într-o poziție invalidă.

---

## 9. Interfața

Textele vizibile sunt în limba română. Fontul este de cel puțin 12 pt. Fundalul de joc este închis, cu contrast suficient pentru text și piese. Ferestrele secundare (clasament, setări, despre, introducerea numelui) se deschid modal, cu `ShowDialog`.

### 9.1 Meniul principal **[M]**

`MainMenuForm` este fereastra de start. Butoane, în această ordine: **Joc nou**, **Clasament**, **Setări**, **Despre**, **Ieșire**. Au aceeași lățime, același font și aceleași culori.

- **Joc nou** deschide `GameForm`. La închiderea jocului, meniul este din nou vizibil.
- **Clasament** deschide `HighScoresForm`.
- **Setări** deschide `SettingsForm` când partea **[S]** există; altfel butonul lipsește, nu deschide o fereastră goală.
- **Despre** deschide `AboutForm` când partea **[C]** există.
- **Ieșire** închide aplicația. Dacă un joc este în desfășurare, cere confirmare **[S]**.

Navigarea cu săgeți și Enter este **[S]**. Mouse-ul este obligatoriu.

### 9.2 Fereastra de joc **[M]**

`GameForm`:

- `FormBorderStyle = FixedSingle`, `MaximizeBox = false`, `StartPosition = CenterScreen`;
- `Size` aproximativ 600×720 px;
- `AutoScaleMode = None`, astfel încât celula să rămână 30×30 px;
- `DoubleBuffered = true`, setat în constructorul formularului (proprietatea este `protected`);
- `KeyPreview = true`.

Zona client, în pixeli:

```text
margine stânga 20
puț:      (20, 60)  300×600
spațiu    20
panou:    (340, 60) lățime 240
```

Titlul ferestrei este în română. La închidere, timerul se oprește și motorul nu mai primește `Update`.

### 9.3 Desenarea **[M]**

`BoardRenderer.Draw(Graphics g, GameEngine engine, Rectangle area)` desenează doar în `area`:

1. fundalul puțului;
2. grila, cu linii discrete;
3. celulele blocate, doar pentru rândurile vizibile (`y >= HiddenRows`);
4. ghost piece, dacă este activ **[S]**;
5. piesa curentă.

Fiecare celulă vizibilă este un pătrat plin de `CellSize` pixeli, cu un contur mai închis. Culoarea vine din id-ul grilei, prin temă. Rândurile 0 și 1 nu se desenează.

Pensulele și creioanele sunt create o dată și eliberate în `Dispose`. `Paint` nu construiește `Brush` sau `Pen` noi. După fiecare schimbare de stare, formularul apelează `Invalidate()`. Desenarea are loc doar în `OnPaint`.

Ghost piece **[S]**: contur sau culoare semitransparentă pe celulele întoarse de `GetDropPosition`. Celulele care coincid cu piesa curentă nu se desenează a doua oară. Ghost-ul lipsește când `GhostEnabled` este fals.

Animația de linie **[C]**: la `LinesCleared`, rândurile eliminate clipesc alb circa `LineClearFlashMs` înainte ca desenul să arate rândurile coborâte. În acest interval, formularul nu trimite comenzi de mișcare către motor. Scorul folosit este cel calculat de `ScoreManager`; animația nu mai adaugă puncte și nu modifică grila.

### 9.4 Panoul lateral **[M]**

În panoul din dreapta, de sus în jos:

- titlul jocului;
- caseta **NEXT** cu următoarea piesă, centrată; încă două piese sunt **[S]**;
- caseta **HOLD** **[S]**, cu piesa reținută; desen estompat cât timp `CanHold` este fals; casetă goală dacă nu există piesă reținută;
- **SCOR** pe 6 cifre, cu zerouri în față (`000000`);
- **NIVEL**;
- **LINII**.

Scorul, nivelul și liniile se redesenează ca reacție la `ScoreChanged` și `LevelChanged`, nu prin citirea motorului la fiecare tick doar pentru aceste valori. `Invalidate` după eveniment este reacția acceptată.

### 9.5 Bucla și tastatura **[M]**

Un `System.Windows.Forms.Timer` cu `Interval = 16` pornește odată cu jocul. La fiecare tick se măsoară timpul real cu `Stopwatch.GetTimestamp()` și `Stopwatch.GetElapsedTime`, se apelează `engine.Update(elapsedMs)`, apoi `Invalidate()`.

Taste implicite:

| Acțiune | Tastă | Repetare |
|---|---|---|
| Stânga / dreapta | ← / → | prima repetare după 170 ms, apoi la fiecare 50 ms, cât tasta este jos |
| Rotire orară | ↑ sau X | o singură dată pe apăsare |
| Rotire antiorară | Z | o singură dată pe apăsare |
| Soft drop | ↓ | repetă cât tasta este jos |
| Hard drop | Spațiu | o singură dată pe apăsare |
| Hold | C sau Shift | o singură dată pe apăsare **[S]** |
| Pauză | P sau Esc | comută pauză / reluare |
| Restart | R | cere confirmare, apoi `StartNewGame` **[S]** |

`KeyUp` oprește repetarea săgeții eliberate. O tastă ținută nu generează rotații sau hard drop-uri repetate.

### 9.6 Pauză, restart, Game Over **[M]**

- Pauză: strat semitransparent peste puț și textul **PAUZĂ**. Reluarea scoate stratul și continuă de la aceeași poziție.
- Pierderea focusului pune jocul pe pauză **[C]**.
- Restart: `MessageBox` de confirmare, apoi `StartNewGame` cu nivelul de start din setări.
- Game Over: panou cu textul **GAME OVER**, scorul final, nivelul, butonul **Joacă din nou** și butonul **Meniu**.
- În pauză și după Game Over, săgețile, rotația, drop-ul și hold nu mișcă piesa.

Dacă scorul final este high score, dialogul de nume apare înainte de a lăsa jucătorul doar cu panoul de final. Vezi secțiunea 10.

### 9.7 Despre **[C]**

`AboutForm` arată numele jocului, versiunea, numele echipei și tabelul de taste din secțiunea 9.5. Se închide cu Esc. Un jucător care nu a citit SRS-ul poate învăța comenzile din acest ecran.

### 9.8 Temă **[C]**, aspectul minim **[M]**

`Theme` conține culorile de fundal, grilă, text și cele 7 culori de piesă. Tema întunecată există în livrarea **[M]** și este folosită de `BoardRenderer` și de meniu. Tema luminoasă și comutarea din setări sunt **[C]**. Ambele teme păstrează un contrast lizibil între text, fundal și piese.

**Criterii de acceptare**

- Meniul oferă căile **Joc nou**, **Clasament** și **Ieșire**, iar închiderea fiecărei ferestre înapoiază controlul fără ferestre rămase deschise.
- Puțul desenat are 10×20 celule de 30 px. Rândurile ascunse nu apar.
- Nu se vede clipire la căderea piesei. Pensulele nu sunt alocate în `Paint`.
- O săgeată ținută mută piesa repetat; Spațiul ținut nu face decât un hard drop.
- Timpul dintre `KeyDown` pe ←, când mutarea este posibilă, și poziția nouă desenată este sub 50 ms.
- În pauză, piesa stă pe loc un interval mai lung decât `DropIntervalMs`.
- La Game Over se văd scorul și cele două acțiuni: rejucare și întoarcere la meniu.
- Ghost-ul, când este implementat, urmărește mutarea și rotația și dispare când setarea este oprită.

---

## 10. Scoruri și setări

### 10.1 Modele

`HighScoreEntry`: `Name` (string), `Score` (int), `Level` (int), `Date` (`DateTime`).

`GameSettings`:

| Proprietate | Implicit | Observație |
|---|---|---|
| `SoundEnabled` | `true` | **[C]** efectul sonor; câmpul există dacă există setările |
| `GhostEnabled` | `true` | **[S]** |
| `StartLevel` | `1` | 1–10 **[S]** |
| `Theme` | `"dark"` | **[C]** |
| `Keys` | dicționarul de mai jos | **[C]** reconfigurarea; valorile implicite sunt folosite oricum |

Chei implicite, cu numele din JSON-ul SRS:

```json
{
  "left": "Left",
  "right": "Right",
  "rotate": "Up",
  "rotateCounterClockwise": "Z",
  "softDrop": "Down",
  "hardDrop": "Space",
  "hold": "C",
  "pause": "P",
  "restart": "R"
}
```

Ambele tipuri au constructor fără parametri și proprietăți publice, ca să poată fi citite de source generator.

`AppJsonContext` este un `JsonSerializerContext` parțial, cu `PropertyNamingPolicy = CamelCase` și `WriteIndented = true`, și cu `[JsonSerializable]` pentru `List<HighScoreEntry>` și `GameSettings`. Fișierul scris arată ca exemplele din SRS 7.2 și 7.3: `name`, `score`, `level`, `date`, `soundEnabled`, `ghostEnabled`, `startLevel`, `theme`, `keys`. Data este ISO 8601.

### 10.2 `HighScoreRepository` **[M]**

Director: `%AppData%\TetrisGame\`, creat dacă lipsește. Fișier: `highscores.json`.

| Metodă | Contract |
|---|---|
| `Load()` | lista sortată descrescător după `Score`; la egalitate, data mai recentă este prima |
| `Save(list)` | scrie JSON-ul; păstrează cel mult `MaxHighScores` intrări |
| `IsHighScore(score)` | adevărat dacă lista are sub 10 intrări sau dacă `score` este strict mai mare decât ultimul loc |
| `Add(entry)` | inserează, sortează, taie la 10, salvează |

Erori:

- fișier lipsă → listă goală, fără excepție către UI;
- JSON invalid → listă goală, fișierul existent este redenumit `highscores.bak`, aplicația continuă;
- eroare de acces → aplicația rămâne deschisă și afișează un mesaj; jocul curent nu se pierde.

Nu se cere rularea ca administrator. Nu se scrie în afara acestui director.

### 10.3 Introducerea numelui **[M]**

La Game Over, dacă `IsHighScore(score)`:

- `NameEntryForm` cere numele;
- lungimea după `Trim` este între 1 și `MaxNameLength`;
- spațiile de la capete se elimină;
- numele gol sau format doar din spații nu poate fi salvat;
- se creează `HighScoreEntry` cu scorul, nivelul atins și `DateTime.Now`, apoi `Add`.

Reținerea ultimului nume ca valoare implicită în dialog este **[C]**.

### 10.4 Clasamentul **[M]**

`HighScoresForm` încarcă lista la deschidere. Coloane: loc, nume, scor, nivel, dată. Lista goală afișează **Niciun scor încă**. Butonul **Închide** există. **Șterge clasamentul**, cu confirmare, este opțional.

### 10.5 Setări **[S]**

`SettingsRepository` folosește `%AppData%\TetrisGame\settings.json`.

- `Load()` întoarce valorile implicite dacă fișierul lipsește sau este corupt.
- Valorile din afara domeniului se corectează la încărcare: `StartLevel` ajunge în 1–10; un nume de temă necunoscut devine `dark`.
- `Save(settings)` scrie fișierul.
- `ResetToDefaults()` revine la tabelul din 10.1 **[C]**.

`SettingsForm` are: bifă pentru sunet, bifă pentru ghost piece, selector de nivel 1–10, listă de teme când temele există. **Salvează** scrie fișierul. **Anulează** închide fără să scrie. Ghost și sunet se aplică imediat în jocul deja deschis; nivelul de start se aplică la următorul `StartNewGame`.

Reconfigurarea tastelor **[C]**: fiecare acțiune are o tastă, următoarea tastă apăsată o înlocuiește, iar o tastă deja folosită de altă acțiune este refuzată. `GameForm` citește legăturile din `GameSettings.Keys`.

**Criterii de acceptare**

- Un scor din top 10 salvat, apoi reîncărcat într-o instanță nouă, păstrează numele, scorul, nivelul și data.
- Locul 11 nu rămâne în fișier.
- Ștergerea `highscores.json` sau înlocuirea lui cu text invalid lasă aplicația pornită, cu clasament gol.
- Un nume de 13 caractere sau un nume gol nu este scris în clasament.
- Clasamentul este sortat descrescător și arată mesajul de listă goală când nu există intrări.
- Pentru setări **[S]**: după repornire, ghost, sunet și nivelul de start sunt cele salvate. `StartLevel` 0 sau 11 din fișier devine o valoare din 1–10, iar jocul pornește.

---

## 11. Sunet **[C]**

Fișiere `.wav` în proiect, cu copiere la output dacă sunt mai noi: mutare, rotire, fixare, eliminare de linie, patru linii, Game Over. `SoundManager` le redă la evenimentele motorului prin `System.Media.SoundPlayer`. Dacă tipul nu este vizibil din referințele implicite, se adaugă pachetul `System.Windows.Extensions` versiunea 10.0.x. NAudio este alternativă, nu cerință.

Lipsa unui fișier nu aruncă excepția până în formular. `SoundEnabled = false` oprește efectele. Muzica de fundal, dacă există, este în buclă și se oprește în pauză și la Game Over.

**Criterii de acceptare**

- Cu sunetul pornit, fixarea, eliminarea unei linii și Game Over produc efectul corespunzător.
- Cu sunetul oprit, aceleași acțiuni nu produc sunet.
- Un fișier `.wav` șters de pe disc nu închide aplicația.

---

## 12. Teste și calitate

### 12.1 Teste automate **[M]**

`dotnet test` trece pe toate fișierele din secțiunea 2.4. Acoperirea pentru `Board`, `ScoreManager` și `PieceFactory` include fiecare metodă publică și ramurile descrise în secțiunile 5–7 (coliziune, lock, linii adiacente și neadiacente, nivel, interval, sac). Testele de repository folosesc un director temporar, nu `%AppData%` real.

### 12.2 Scenarii manuale **[M]**

| ID | Pași | Rezultat |
|---|---|---|
| T-01 | Pornire | Meniul principal |
| T-02 | Joc nou, ← la peretele stâng | Piesa nu iese din puț |
| T-03 | O linie completă la nivel 1 | Linia dispare, scorul crește cu 100 |
| T-04 | Patru linii cu piesa I | Scorul crește cu 800 × nivelul curent |
| T-05 | P în timpul jocului, apoi P din nou | Pauză, apoi reluare din aceeași poziție |
| T-06 | Umplerea puțului | Game Over cu scorul final |
| T-07 | Scor care intră în top 10 | Se cere numele și apare în clasament |
| T-08 | Ștergerea `highscores.json`, pornire din nou | Fără eroare, clasament gol |
| T-09 | Hard drop | Aterizare imediată, +2 puncte pe celulă parcursă |
| T-10 | Hold de două ori consecutiv | A doua comandă este ignorată |

Separat de tabel: o partidă de 15–20 de minute, pauză și reluare repetată, restart în timpul jocului, mutarea ferestrei în timpul jocului. Defectele găsite se notează cu pașii de reproducere și se corectează înainte de predare.

### 12.3 Performanță și predare **[M]**

- Cel puțin 60 de cadre pe secundă pe un calculator cu 4 GB RAM și procesor dual-core.
- Pornirea aplicației, până la meniul desenat, durează sub 3 secunde.
- Memoria procesului rămâne sub 150 MB în timpul unei partide.
- Aplicația nu se închide la erorile de fișier din secțiunea 10.

Publicarea:

```text
dotnet publish TetrisGame.UI -c Release -f net10.0-windows -o publish
```

Varianta implicită este framework-dependent și cere .NET Desktop Runtime 10 pe calculatorul țintă. Arhiva de predare conține codul sursă, folderul `publish`, SRS-ul, README-ul și capturi cu meniul, jocul și clasamentul. Tag-ul Git `v1.0` este pe `main`.

**Criterii de acceptare**

- `dotnet test` se termină fără teste eșuate.
- T-01 … T-10 sunt bifate, cu rezultatul observat notat.
- `publish` pornește pe un calculator care are Desktop Runtime 10 și nu are SDK-ul instalat.
- Revizuirea confirmă separarea straturilor, documentația XML pe tipurile publice și lipsa literalilor numerici de joc în afara `GameConstants`.

---

## 13. În afara proiectului

Următoarele nu se implementează și nu blochează acceptarea:

- multiplayer în rețea, conturi, server;
- mod Sprint, Ultra sau Versus;
- linii de gunoi, replay, două persoane pe aceeași tastatură;
- portare pe Linux sau macOS;
- sunet sintetizat dinamic.

---

## 14. Urmărirea cerințelor

| SRS | Conținut în proiect |
|---|---|
| FR-01, FR-02, FR-03 | `MainMenuForm` |
| FR-04 | `AboutForm` **[C]** |
| FR-10 … FR-16 | `Board`, `Tetromino`, `PieceFactory`, `GameEngine` |
| FR-20 … FR-27 | `GameForm` și comenzile din `GameEngine` |
| FR-30 … FR-36 | `Board.ClearFullLines`, `ScoreManager`, panoul lateral, animația **[C]** |
| FR-40 … FR-42 | panoul Next, Hold, ghost |
| FR-50 … FR-53 | pauză, Game Over, restart, pauză la pierderea focusului **[C]** |
| FR-60 … FR-63 | `HighScoreRepository`, `NameEntryForm`, `HighScoresForm` |
| FR-70 … FR-72 | `GameSettings`, `SettingsRepository`, `SettingsForm` |
| FR-80, FR-81 | `SoundManager` **[C]** |
| NFR-01 … NFR-04 | secțiunea 12.3 |
| NFR-05, NFR-06 | erorile de JSON din secțiunea 10 |
| NFR-10 … NFR-13 | `Directory.Build.props`, XML docs, `GameConstants`, `Core` fără Windows Forms |
| NFR-14, NFR-15 | Windows 10/11, scriere doar în `%AppData%\TetrisGame` |
