# Backlog — Tetris

Fiecare task este un issue pentru GitHub. Descrie rezultatul funcțional și condițiile prin care issue-ul se închide. Contractul detaliat este în `Tetris_specs.md`. Modul de implementare rămâne la persoana care preia taskul.

**Platformă:** C# 14, .NET 10, Windows Forms.  
**Priorități:** **[M]** Must, **[S]** Should, **[C]** Could.  
**Copiere în GitHub:** **Titlu** devine titlul issue-ului, **Descriere** devine corpul, **Acceptance criteria** se copiază ca listă de bifă. **Etichete** și **Epic** se pun pe issue.

---

## Epic: Fundament

### TASK 01 [M] — Soluția TetrisGame pe .NET 10

**Titlu:** Soluție .NET 10 cu proiectele Core, Data, UI și Tests  
**Etichete:** `must` `setup`  
**Epic:** Fundament

**Descriere:**

Repository-ul conține soluția `TetrisGame` pe .NET 10, cu patru proiecte separate. Logica de joc, persistența, interfața și testele pot fi compilate și referite independent. Aplicația pornește din proiectul de interfață.

**Acceptance criteria:**

- [ ] Soluția se numește `TetrisGame` și este creată cu SDK-ul .NET 10.
- [ ] `TetrisGame.Core` și `TetrisGame.Data` țintesc `net10.0`. `TetrisGame.UI` țintește `net10.0-windows` și este aplicație Windows Forms. `TetrisGame.Tests` țintește `net10.0` și folosește xUnit.
- [ ] `UI` referențiază `Core` și `Data`. `Data` referențiază `Core`. `Tests` referențiază `Core` și `Data`. `Core` nu referențiază celelalte proiecte și nu folosește Windows Forms sau `System.Drawing`.
- [ ] `dotnet build` trece fără erori. Pornirea proiectului `UI` deschide o fereastră.

### TASK 02 [M] — Repository și reguli de lucru

**Titlu:** Repository Git cu ramuri, ignorare de build și README  
**Etichete:** `must` `setup`  
**Epic:** Fundament  
**Depinde de:** TASK 01

**Descriere:**

Codul stă într-un repository GitHub pe care toți membrii îl pot clona și compila. Ramura stabilă și ramura de integrare există. Lucrul nou intră în `develop` prin pull request revizuit. README-ul este suficient ca o persoană din afară să pornească jocul.

**Acceptance criteria:**

- [ ] Repository-ul are `.gitignore` pentru `bin/`, `obj/` și `.vs/`, plus un `README.md` cu descrierea jocului, cerința de .NET Desktop Runtime 10, comanda de rulare și structura soluției.
- [ ] Există ramurile `main` și `develop`.
- [ ] Fiecare funcționalitate are ramura ei, iar integrarea în `develop` se face prin pull request aprobat de un alt membru.
- [ ] Fiecare membru a clonat repository-ul, a compilat soluția și are cel puțin un commit.

### TASK 03 [M] — Valorile fixe ale jocului

**Titlu:** Un singur loc pentru dimensiuni, viteză, scor și limite  
**Etichete:** `must` `core`  
**Epic:** Fundament  
**Depinde de:** TASK 01

**Descriere:**

Toate valorile fixe ale jocului sunt definite o singură dată în `TetrisGame.Core` și sunt documentate. Restul proiectului le folosește de acolo.

Valorile: puț 10×20, 2 rânduri ascunse, celulă 30 px, cădere inițială 800 ms, cădere minimă 100 ms, 10 linii pe nivel, 1 punct pe celulă la soft drop, 2 puncte pe celulă la hard drop, nume de maxim 12 caractere, top 10 scoruri, punctaj pe linii `{0, 100, 300, 500, 800}`.

**Acceptance criteria:**

- [ ] Fiecare valoare din descriere există într-un singur tip public din `Core`, cu documentație XML.
- [ ] Dimensiunile, viteza, punctajele și limitele de nume sau clasament din celelalte proiecte folosesc aceste definiții.
- [ ] Schimbarea punctajului pentru patru linii într-un singur loc schimbă valoarea văzută de calculul scorului.

### TASK 04 [M] — Ferestrele aplicației

**Titlu:** Meniu, joc, clasament, setări și despre, cu navigare din meniu  
**Etichete:** `must` `ui`  
**Epic:** Fundament  
**Depinde de:** TASK 01

**Descriere:**

Aplicația are ferestrele `MainMenuForm`, `GameForm`, `HighScoresForm`, `SettingsForm` și `AboutForm`. Pornirea deschide meniul. Din meniu se poate deschide fereastra de joc. Fereastra de joc are dimensiune fixă, este centrată, are titlu în română și desenare fără clipire. Celelalte ferestre se deschid și se închid.

**Acceptance criteria:**

- [ ] Pornirea aplicației afișează `MainMenuForm`.
- [ ] Din meniu, acțiunea de joc nou deschide `GameForm`. Închiderea jocului readuce meniul.
- [ ] `GameForm` are aproximativ 600×720 px, nu poate fi maximizată, stă centrată pe ecran și are titlul în română.
- [ ] Clasamentul, setările și ecranul Despre se deschid din meniu și se închid fără să blocheze aplicația.

---

## Epic: Logica jocului

### TASK 05 [M] — Cele șapte piese și culorile lor

**Titlu:** Tipurile I, O, T, S, Z, J, L cu formă și culoare  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 03

**Descriere:**

Jocul recunoaște exact șapte tetromino. Fiecare are forma de bază din `Tetris_specs.md` (I pe 4×4, O pe 2×2, celelalte pe 3×3) și un identificator de culoare propriu. Logica păstrează identificatorul numeric. Culoarea desenată (cyan, galben, mov, verde, roșu, albastru, portocaliu) este aleasă în interfață.

**Acceptance criteria:**

- [ ] Există un tip pentru I, O, T, S, Z, J și L.
- [ ] Pentru fiecare tip se poate obține forma de rotație 0 și identificatorul de culoare dintr-un singur loc.
- [ ] Identificatorii sunt diferiți între ei și diferiți de celula goală.
- [ ] `Core` expune identificatorul, iar proiectul `Core` rămâne independent de culorile GDI+.

### TASK 06 [M] — Piesa mutabilă

**Titlu:** Tetromino cu poziție, rotație, celule ocupate și copie  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 05

**Descriere:**

O piesă își cunoaște tipul, forma curentă, rotația și poziția colțului din stânga-sus. Poate fi mutată, rotită în ambele sensuri și copiată. Lista celulelor ocupate reflectă forma și poziția curente. Piesa rămâne independentă de puț.

**Acceptance criteria:**

- [ ] Patru rotații în același sens readuc fiecare tip la forma inițială.
- [ ] Celulele ocupate sunt exact patru, pentru orice tip și orice rotație.
- [ ] O copie modificată lasă piesa originală neschimbată.
- [ ] Tipul piesei rămâne același după rotație.

### TASK 07 [M] — Puțul și coliziunile

**Titlu:** Puț 10×20 plus două rânduri ascunse, cu celule libere și piese fixate  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 06

**Descriere:**

Puțul are 10 coloane, 20 de rânduri vizibile și 2 rânduri ascunse deasupra. O poziție este validă când toate celulele ocupate ale piesei stau în puț și pe locuri libere. O piesă validă poate fi fixată în grilă. Conținutul grilei poate fi citit și golit. Rândurile ascunse participă la coliziune și la fixare.

**Acceptance criteria:**

- [ ] O celulă în afara celor 10 coloane sau sub baza puțului face poziția invalidă.
- [ ] O piesă ale cărei celule stau doar în rândurile ascunse, pe locuri libere, are poziție validă.
- [ ] După fixare, celulele piesei rămân ocupate cu identificatorul ei, iar celelalte celule rămân cum erau.
- [ ] O piesă care se suprapune peste o celulă deja ocupată are poziție invalidă.
- [ ] Golirea puțului lasă toate celulele libere.

### TASK 08 [M] — Eliminarea liniilor

**Titlu:** Liniile complete dispar, iar conținutul de deasupra coboară  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 07

**Descriere:**

Un rând în care toate cele 10 celule sunt ocupate este o linie completă. Eliminarea șterge toate liniile complete găsite odată și coboară rândurile rămase, păstrându-le ordinea. Rezultatul spune câte linii au fost eliminate: 0, 1, 2, 3 sau 4.

**Acceptance criteria:**

- [ ] O linie completă dispare, iar rândurile de deasupra ei coboară cu o poziție.
- [ ] Două, trei și patru linii complete eliminate deodată dispar toate, iar conținutul rămas coboară cu numărul corect de rânduri.
- [ ] Liniile complete care au un rând incomplet între ele dispar, iar rândul incomplet rămâne întreg și coboară.
- [ ] O linie completă de la baza puțului și una din mijloc sunt eliminate corect.
- [ ] După eliminare, puțul conține rânduri goale doar în zona rămasă liberă de deasupra conținutului.

### TASK 09 [M] — Mutare și rotire valide

**Titlu:** Piesa se mută și se rotește doar în poziții libere  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 07

**Descriere:**

Jucătorul poate muta piesa o celulă la stânga sau la dreapta și o poate roti în sens orar sau antiorar. Comanda se aplică doar dacă rezultatul stă în puț și pe celule libere. O comandă respinsă lasă piesa exact unde era. O coborâre respinsă înseamnă că piesa trebuie fixată.

**Acceptance criteria:**

- [ ] O mutare laterală validă schimbă poziția cu o celulă.
- [ ] La perete sau lângă o piesă fixată, mutarea laterală și rotația lasă piesa neschimbată.
- [ ] O rotație validă schimbă forma și păstrează piesa în puț.
- [ ] O coborâre care nu mai are loc liber dedesubt este semnalată ca fixare, fără să scoată piesa din puț.

### TASK 10 [S] — Wall kick

**Titlu:** Rotația lângă perete încearcă o deplasare de salvare  
**Etichete:** `should` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 09

**Descriere:**

Când rotația pe loc este blocată, jocul încearcă aceeași rotație cu o deplasare de una sau două celule pe orizontală, apoi cu o celulă în sus. Prima variantă liberă este păstrată. Dacă niciuna nu este liberă, piesa rămâne nerotită. Piesa rotită ocupă doar celule libere.

**Acceptance criteria:**

- [ ] O piesă lipită de peretele stâng sau drept, care nu poate fi rotită pe loc, se rotește dacă una dintre deplasările de salvare este liberă.
- [ ] Aceeași regulă se aplică piesei I, cu deplasare de până la două celule.
- [ ] Când toate variantele sunt blocate, forma și poziția rămân cele dinaintea comenzii.
- [ ] Rotația cu deplasare ocupă doar celule libere din interiorul puțului.

### TASK 11 [M] — Generator 7-bag și poziția de apariție

**Titlu:** Piesele apar în grupuri de șapte, centrate sus  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 06

**Descriere:**

Piesele noi sunt extrase dintr-un sac care conține fiecare tip exact o dată. Sacul gol este înlocuit cu un sac nou, amestecat. Următoarele piese pot fi văzute înainte să fie extrase, iar extragerea confirmă ce s-a văzut. Piesa nouă apare centrată, cu rotația 0, în partea de sus a puțului, inclusiv în rândurile ascunse. Secvența poate fi repetată în teste cu aceeași sursă aleatoare.

**Acceptance criteria:**

- [ ] În orice grup de șapte piese luat de la începutul unui sac, fiecare tip apare exact o dată.
- [ ] Două generatoare pornite la fel produc aceeași secvență.
- [ ] Previzualizarea următoarelor piese coincide cu piesele extrase apoi, în aceeași ordine.
- [ ] Piesa creată începe la rotația 0, pe rândul de sus, centrată pe lățimea de 10 coloane.

### TASK 12 [M] — Scor, nivel și viteză

**Titlu:** Puncte pe linii și pe cădere, nivel la 10 linii, viteză crescătoare  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 03

**Descriere:**

Scorul pornește de la 0. Eliminarea liniilor adaugă `100 / 300 / 500 / 800 × nivelul curent`, folosind nivelul de dinaintea creșterii. Nivelul crește cu unu la fiecare 10 linii eliminate și pornește de la nivelul ales la începutul partidei. Intervalul de cădere este 800 ms la nivel 1 și scade cu 10% la fiecare nivel, rămânând cel puțin 100 ms. Soft drop adaugă 1 punct pe celulă. Hard drop adaugă 2 puncte pe celulă. Schimbarea reală a scorului sau a nivelului este anunțată.

**Acceptance criteria:**

- [ ] La nivel 1, una, două, trei și patru linii adaugă 100, 300, 500 și 800 de puncte.
- [ ] La nivel 3, patru linii adaugă 2400 de puncte.
- [ ] A zecea linie eliminată crește nivelul după calculul punctelor pentru acea eliminare.
- [ ] O partidă pornită de la nivel 5 începe la nivel 5 și ajunge la nivel 6 după 10 linii.
- [ ] Nivel 1 cade la 800 ms, nivel 2 la 720 ms, nivel 3 la 648 ms, iar intervalul rămâne cel puțin 100 ms.
- [ ] Patru celule de soft drop adaugă 4 puncte. Patru celule de hard drop adaugă 8 puncte.
- [ ] Anunțul de scor sau de nivel apare doar când valoarea s-a schimbat.

### TASK 13 [M] — Partida: cădere, fixare și stări

**Titlu:** Motorul pornește, oprește, fixează piesele și încheie partida  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 08, TASK 09, TASK 11, TASK 12

**Descriere:**

O partidă are stările meniu, în joc, pauză și sfârșit. În timpul jocului, piesa coboară singură la intervalul nivelului curent. Când nu mai poate coborî, se fixează, liniile complete se elimină, scorul se actualizează și apare piesa următoare. Dacă piesa următoare nu are loc, partida se termină. În pauză și după sfârșit, timpul de cădere stă pe loc. Motorul poate fi condus din teste, fără fereastră. Mutarea, rotația, pauza, reluarea și pornirea unei partide noi sunt comenzi ale motorului.

**Acceptance criteria:**

- [ ] O partidă nouă golește puțul, pune scorul la 0, pornește de la nivelul cerut și aduce prima piesă.
- [ ] După trecerea intervalului de cădere, piesa coboară o celulă.
- [ ] O piesă care nu mai poate coborî rămâne fixată, liniile complete dispar, iar punctele corespund numărului de linii.
- [ ] Piesa următoare apare după fixare. Dacă poziția ei este ocupată, starea devine sfârșit de partidă și piesa nu acoperă celule deja ocupate.
- [ ] În pauză și după sfârșitul partidei, același interval de timp lasă piesa nemișcată.
- [ ] Scenariul de mai sus poate fi rulat din proiectul de teste, fără interfață.

### TASK 14 [M] — Soft drop și hard drop

**Titlu:** Cădere accelerată cu puncte și cădere instantanee până jos  
**Etichete:** `must` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 13

**Descriere:**

Soft drop coboară piesa cu o celulă, adaugă 1 punct și reia așteptarea căderii automate. Dacă dedesubt nu mai este loc, piesa se fixează. Hard drop **[S]** duce piesa în ultima poziție liberă de dedesubt, adaugă 2 puncte pentru fiecare celulă parcursă și o fixează imediat. Aceeași poziție de aterizare este disponibilă și pentru umbra piesei.

**Acceptance criteria:**

- [ ] Soft drop pe o celulă liberă coboară piesa și adaugă 1 punct.
- [ ] După soft drop, căderea automată așteaptă din nou intervalul complet al nivelului.
- [ ] Soft drop fără loc dedesubt fixează piesa.
- [ ] Hard drop lasă piesa fixată pe poziția de aterizare, iar scorul crește cu 2 puncte pe fiecare celulă parcursă.
- [ ] Hard drop pornit deja pe poziția de aterizare fixează piesa și adaugă 0 puncte.
- [ ] Poziția de aterizare anunțată pentru piesa curentă este aceeași cu locul în care ajunge hard drop.

### TASK 15 [S] — Hold

**Titlu:** Păstrarea piesei curente, o singură dată până la fixare  
**Etichete:** `should` `core`  
**Epic:** Logica jocului  
**Depinde de:** TASK 13

**Descriere:**

Jucătorul poate pune piesa curentă deoparte și poate primi în locul ei piesa păstrată. La primul Hold, locul ei este luat de piesa următoare. La următorul Hold permis, cele două piese se schimbă. Piesa adusă în joc apare centrată sus, la rotația 0. Hold este disponibil o singură dată până când piesa din joc se fixează. Comanda care ar aduce o piesă într-o poziție ocupată lasă partida neschimbată.

**Acceptance criteria:**

- [ ] Primul Hold reține tipul piesei curente și aduce piesa următoare în poziția de start.
- [ ] Hold-ul următor, după o fixare, schimbă piesa din joc cu cea reținută, ambele la rotația 0 și în poziția de start.
- [ ] Al doilea Hold, fără o fixare între ele, lasă piesa curentă și piesa reținută neschimbate.
- [ ] Un Hold care nu are loc liber în poziția de start lasă starea de dinaintea comenzii.
- [ ] După fixarea piesei, Hold devine din nou disponibil.

---

## Epic: Interfață

### TASK 16 [M] — Desenarea puțului și a pieselor

**Titlu:** Puțul, piesa curentă și piesele fixate sunt vizibile și stabile  
**Etichete:** `must` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 04, TASK 13

**Descriere:**

Zona de joc arată fundalul, grila, piesele fixate și piesa curentă. Fiecare celulă vizibilă are 30×30 px. Rândurile ascunse rămân în afara desenului. Culoarea celulei corespunde tipului piesei. Desenul se actualizează când starea jocului se schimbă și rămâne stabil în timpul căderii.

**Acceptance criteria:**

- [ ] Pe ecran se văd 10×20 celule, cu piesa curentă și cu piesele deja fixate.
- [ ] Rândurile de deasupra zonei vizibile, în care piesa își face apariția, rămân nedesenate.
- [ ] Culorile sunt cele ale celor șapte tipuri: cyan, galben, mov, verde, roșu, albastru, portocaliu.
- [ ] În timpul căderii, imaginea rămâne fără clipire.
- [ ] Actualizarea desenului urmărește schimbările din partidă.

### TASK 17 [M] — Bucle de joc și taste

**Titlu:** Partida rulează din fereastră și răspunde la taste  
**Etichete:** `must` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 13, TASK 16

**Descriere:**

Fereastra de joc ține partida în mișcare și trimite comenzile jucătorului către motor. Săgețile laterale mută piesa și se repetă dacă rămân apăsate. Săgeata sus și X rotesc în sens orar. Z rotește în sens antiorar. Săgeata jos accelerează căderea. Spațiul face hard drop. C sau Shift fac Hold, când Hold există. P sau Esc schimbă pauza. R cere repornirea. Rotația și hard drop se întâmplă o dată la fiecare apăsare. La închiderea ferestrei, partida se oprește.

**Acceptance criteria:**

- [ ] Piesa coboară singură, la intervalul nivelului, cât fereastra de joc este deschisă.
- [ ] O mutare laterală posibilă se vede în mai puțin de 50 ms de la apăsare.
- [ ] O săgeată laterală ținută apăsată repetă mutarea după o scurtă așteptare, apoi la interval scurt.
- [ ] X, Z, săgeata sus, săgeata jos, Spațiul, C, Shift, P, Esc și R declanșează acțiunea din descriere.
- [ ] Ținerea apăsată a tastei de rotație sau de hard drop produce o singură comandă.
- [ ] Închiderea ferestrei oprește actualizarea partidei.

### TASK 18 [M] — Panoul de scor și piesa următoare

**Titlu:** Scor, nivel, linii, piesa următoare și piesa din Hold  
**Etichete:** `must` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 12, TASK 16

**Descriere:**

În dreapta puțului se văd scorul pe șase cifre, nivelul, numărul de linii și piesa următoare, centrată în caseta ei. Caseta Hold **[S]** arată piesa reținută, iar când Hold nu este disponibil piesa apare estompată. Valorile de scor, nivel și linii se schimbă odată cu partida.

**Acceptance criteria:**

- [ ] Scorul, nivelul și liniile sunt vizibile pe toată durata partidei.
- [ ] Scorul este afișat pe șase cifre, cu zerouri în față.
- [ ] Caseta piesei următoare arată piesa care va intra în joc și se actualizează după fiecare fixare.
- [ ] După o eliminare de linie, scorul și numărul de linii de pe ecran corespund partidei.
- [ ] Când Hold este implementat, caseta arată piesa reținută și o afișează estompat cât timp comanda este blocată.

### TASK 19 [S] — Umbra piesei

**Titlu:** Ghost piece pe poziția de aterizare, cu posibilitate de oprire  
**Etichete:** `should` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 14, TASK 16

**Descriere:**

Jucătorul vede unde va ateriza piesa curentă. Umbra urmărește mutările și rotațiile și se deosebește de piesa reală prin contur sau transparență. Poate fi oprită din setări.

**Acceptance criteria:**

- [ ] Umbra stă pe poziția de aterizare a piesei curente.
- [ ] După mutare sau rotație, umbra ajunge în noua poziție de aterizare.
- [ ] Umbra rămâne vizibil distinctă de piesa curentă și de piesele fixate.
- [ ] Cu opțiunea oprită, umbra lipsește din desen.

### TASK 20 [M] — Pauză, repornire și sfârșit de partidă

**Titlu:** Pauză, confirmare la repornire și ecran de Game Over  
**Etichete:** `must` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 13, TASK 17

**Descriere:**

P sau Esc oprește partida și afișează **PAUZĂ** peste puț. Aceeași comandă reia partida din poziția lăsată. R, sau un buton de repornire, cere confirmare și începe o partidă nouă. La sfârșitul partidei se văd **GAME OVER**, scorul final, nivelul, **Joacă din nou** și **Meniu**. În pauză și după sfârșit, piesa ignoră tastele de joc. Punerea automată pe pauză când fereastra își pierde focusul este **[C]**.

**Acceptance criteria:**

- [ ] În pauză, textul **PAUZĂ** este vizibil, iar piesa rămâne pe loc până la reluare.
- [ ] Reluarea continuă din aceeași poziție, cu același scor.
- [ ] Repornirea întreabă jucătorul înainte să șteargă partida curentă.
- [ ] La Game Over se văd scorul final, nivelul și cele două ieșiri: partidă nouă și meniu.
- [ ] În pauză și la Game Over, săgețile, rotația și căderea accelerată lasă piesa nemișcată.

### TASK 21 [M] — Meniul principal

**Titlu:** Meniu în română pentru joc, clasament, setări, despre și ieșire  
**Etichete:** `must` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 04

**Descriere:**

Meniul oferă **Joc nou**, **Clasament**, **Setări**, **Despre** și **Ieșire**, cu același aspect. Jocul nou deschide partida și, la întoarcere, meniul este din nou pe ecran. Ieșirea închide aplicația. Confirmarea la ieșire, când o partidă este în curs, este **[S]**. Navigarea cu săgeți și Enter este **[S]**. Toate ferestrele folosesc fundal închis, text lizibil și font de cel puțin 12 pt.

**Acceptance criteria:**

- [ ] Toate cele cinci acțiuni sunt vizibile, în română, și au același stil de buton.
- [ ] **Joc nou** deschide partida. Închiderea partidei arată din nou meniul.
- [ ] **Clasament**, **Setări** și **Despre** deschid fereastra corespunzătoare, care se închide și lasă meniul utilizabil.
- [ ] **Ieșire** închide aplicația.
- [ ] Textul din meniu și din ferestrele deschise din el are cel puțin 12 pt.

### TASK 22 [C] — Animația liniilor eliminate

**Titlu:** Liniile complete clipesc scurt înainte să coboare restul  
**Etichete:** `could` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 08, TASK 16

**Descriere:**

La eliminarea liniilor, rândurile complete sunt evidențiate cam 200 ms, apoi desenul arată puțul cu rândurile coborâte. În acest răstimp, piesa ignoră gravitația și tastele de joc. Punctele și conținutul logic al puțului rămân cele produse de eliminare.

**Acceptance criteria:**

- [ ] Liniile eliminate se văd evidențiate înainte ca rândurile de deasupra să apară coborâte.
- [ ] Evidențierea durează aproximativ 200 ms.
- [ ] În acest interval, piesa curentă rămâne nemișcată.
- [ ] Scorul de după animație este scorul calculat pentru numărul de linii eliminate.

### TASK 23 [C] — Ecranul Despre

**Titlu:** Versiune, echipă și lista de taste  
**Etichete:** `could` `ui`  
**Epic:** Interfață  
**Depinde de:** TASK 21

**Descriere:**

Ecranul Despre arată numele jocului, versiunea, echipa și tastele implicite. Se deschide deasupra meniului și se închide cu Esc. Un jucător nou poate învăța comenzile din acest ecran.

**Acceptance criteria:**

- [ ] Sunt vizibile numele jocului, versiunea și numele echipei.
- [ ] Tabelul conține mutarea, rotația în ambele sensuri, soft drop, hard drop, Hold, pauza și repornirea, cu tastele implicite.
- [ ] Esc închide ecranul și readuce meniul.

---

## Epic: Date și setări

### TASK 24 [M] — Modelele de scor și de setări

**Titlu:** Scorul și setările pot fi scrise și citite fără pierdere  
**Etichete:** `must` `data`  
**Epic:** Date și setări  
**Depinde de:** TASK 01

**Descriere:**

O intrare de clasament conține numele, scorul, nivelul și data. Setările conțin sunetul, umbra piesei, nivelul de start, tema și tastele. O valoare scrisă este citită înapoi cu aceleași câmpuri. Numele câmpurilor din fișier sunt cele din `Tetris_specs.md`: `name`, `score`, `level`, `date`, `soundEnabled`, `ghostEnabled`, `startLevel`, `theme`, `keys`.

**Acceptance criteria:**

- [ ] O intrare cu nume, scor, nivel și dată este citită cu aceleași valori după scriere.
- [ ] Setările implicite au sunetul pornit, umbra pornită, nivelul de start 1, tema `dark` și tastele implicite ale jocului.
- [ ] Câmpurile din fișierul scris folosesc numele din descriere.

### TASK 25 [M] — Salvarea clasamentului

**Titlu:** Top 10 local în %AppData%\TetrisGame\highscores.json  
**Etichete:** `must` `data`  
**Epic:** Date și setări  
**Depinde de:** TASK 24

**Descriere:**

Clasamentul local păstrează cel mult 10 scoruri, sortate descrescător, în `%AppData%\TetrisGame\highscores.json`. Un scor intră în top dacă lista are mai puțin de 10 intrări sau dacă este mai mare decât ultimul loc. Lipsa fișierului sau un fișier invalid lasă aplicația deschisă, cu o listă goală. Fișierul invalid este păstrat ca `highscores.bak`. O eroare de acces este anunțată și lasă aplicația deschisă.

**Acceptance criteria:**

- [ ] Un scor salvat este prezent după o pornire nouă a aplicației, cu numele, scorul, nivelul și data.
- [ ] Lista încărcată este ordonată descrescător după scor și conține cel mult 10 intrări.
- [ ] Al unsprezecelea scor, mai mic decât toate celelalte, rămâne în afara clasamentului.
- [ ] Fără fișier, clasamentul este gol și aplicația pornește.
- [ ] Cu un fișier care conține text invalid, clasamentul este gol, iar fișierul anterior există ca `highscores.bak`.

### TASK 26 [M] — Numele jucătorului la scor mare

**Titlu:** La top 10 se cere un nume de cel mult 12 caractere  
**Etichete:** `must` `ui` `data`  
**Epic:** Date și setări  
**Depinde de:** TASK 20, TASK 25

**Descriere:**

La sfârșitul partidei, un scor care intră în top 10 deschide un dialog de nume. Numele are între 1 și 12 caractere după eliminarea spațiilor de la capete. Intrarea salvată conține numele, scorul, nivelul atins și data. Ultimul nume folosit poate apărea completat în dialog **[C]**.

**Acceptance criteria:**

- [ ] Un scor din top 10 deschide dialogul de nume înainte ca jucătorul să încheie ecranul de Game Over.
- [ ] Un nume gol sau format doar din spații rămâne nesalvat.
- [ ] Un nume mai lung de 12 caractere rămâne nesalvat.
- [ ] Numele acceptat apare imediat în clasament, cu scorul și nivelul partidei.
- [ ] Un scor care cade în afara top 10 închide partida fără dialog de nume.

### TASK 27 [M] — Ecranul de clasament

**Titlu:** Lista top 10 cu loc, nume, scor, nivel și dată  
**Etichete:** `must` `ui`  
**Epic:** Date și setări  
**Depinde de:** TASK 25

**Descriere:**

Ecranul de clasament arată scorurile salvate, în ordinea descrescătoare, cu locul, numele, scorul, nivelul și data. Lista goală arată **Niciun scor încă**. Ecranul se închide cu un buton. Ștergerea clasamentului, cu confirmare, este opțională.

**Acceptance criteria:**

- [ ] La deschidere sunt vizibile scorurile salvate, sortate descrescător.
- [ ] Fiecare rând arată locul, numele, scorul, nivelul și data.
- [ ] Fără scoruri salvate, se vede textul **Niciun scor încă**.
- [ ] Butonul de închidere readuce ecranul anterior.

### TASK 28 [S] — Salvarea setărilor

**Titlu:** Setările rămân în %AppData%\TetrisGame\settings.json  
**Etichete:** `should` `data`  
**Epic:** Date și setări  
**Depinde de:** TASK 24

**Descriere:**

Setările se salvează în `%AppData%\TetrisGame\settings.json` și sunt disponibile jocului și interfeței. Lipsa fișierului sau un fișier invalid înseamnă setările implicite. Un nivel de start în afara intervalului 1–10 este adus în acest interval. O temă necunoscută devine `dark`. Există și revenirea la valorile implicite **[C]**.

**Acceptance criteria:**

- [ ] Setările salvate sunt aceleași după o pornire nouă.
- [ ] Fără fișier, jocul primește valorile implicite din TASK 24.
- [ ] Un fișier invalid lasă aplicația pornită, tot cu valorile implicite.
- [ ] Un nivel de start salvat ca 0 sau 11 este folosit ca o valoare între 1 și 10.

### TASK 29 [S] — Ecranul de setări

**Titlu:** Sunet, umbră, nivel de start și temă, cu salvare  
**Etichete:** `should` `ui`  
**Epic:** Date și setări  
**Depinde de:** TASK 28

**Descriere:**

Ecranul de setări permite oprirea sunetului, oprirea umbrei, alegerea nivelului de start între 1 și 10 și alegerea temei, când temele există. **Salvează** scrie alegerile. **Anulează** închide ecranul și lasă setările anterioare. Umbra și sunetul se aplică și unei partide deja deschise. Nivelul de start se aplică la partida următoare. Revenirea la valorile implicite este **[C]**.

**Acceptance criteria:**

- [ ] Jucătorul poate schimba sunetul, umbra și nivelul de start din ecran.
- [ ] **Salvează** face ca valorile să fie prezente și după repornirea aplicației.
- [ ] **Anulează** lasă valorile dinaintea deschiderii ecranului.
- [ ] Umbra pornită sau oprită se vede în partida curentă.
- [ ] Nivelul de start salvat este nivelul cu care începe partida următoare.

### TASK 30 [C] — Taste configurabile

**Titlu:** Fiecare acțiune poate primi o altă tastă, fără duplicate  
**Etichete:** `could` `ui`  
**Epic:** Date și setări  
**Depinde de:** TASK 17, TASK 29

**Descriere:**

În setări, fiecare acțiune arată tasta ei curentă. Jucătorul alege acțiunea, iar următoarea tastă apăsată o înlocuiește. O tastă deja folosită de altă acțiune este refuzată. Partida folosește tastele salvate.

**Acceptance criteria:**

- [ ] Lista arată toate acțiunile de joc și tasta curentă a fiecăreia.
- [ ] O tastă nouă, liberă, înlocuiește tasta acțiunii alese și rămâne salvată după repornire.
- [ ] O tastă deja atribuită lasă ambele acțiuni cu tastele anterioare.
- [ ] În partidă, acțiunea răspunde la tasta configurată.

### TASK 31 [C] — Teme de culoare

**Titlu:** Temă întunecată și temă luminoasă în tot jocul  
**Etichete:** `could` `ui`  
**Epic:** Date și setări  
**Depinde de:** TASK 16, TASK 29

**Descriere:**

Jocul are două teme, întunecată și luminoasă. Tema stabilește fundalul, grila, textul și culorile celor șapte piese. Schimbarea se vede în meniu, în partida în curs și în ferestrele secundare. Textul și piesele rămân lizibile pe fundal.

**Acceptance criteria:**

- [ ] Ambele teme pot fi alese din setări.
- [ ] După schimbare, puțul, panoul lateral și meniul folosesc tema aleasă.
- [ ] Cele șapte piese rămân deosebite între ele în ambele teme.
- [ ] Textul rămâne lizibil pe fundalul temei alese.

### TASK 32 [C] — Sunet

**Titlu:** Efecte pentru mutare, rotație, fixare, linii și Game Over  
**Etichete:** `could` `ui`  
**Epic:** Date și setări  
**Depinde de:** TASK 13, TASK 29

**Descriere:**

Jocul are efecte sonore pentru mutare, rotație, fixarea piesei, eliminarea unei linii, eliminarea a patru linii și sfârșitul partidei. Efectele urmează evenimentele partidei. Sunetul se oprește din setări. Un fișier audio lipsă lasă jocul deschis și tăcut pentru acel efect. Muzica de fundal, dacă este inclusă, rulează în buclă și se oprește în pauză.

**Acceptance criteria:**

- [ ] Cu sunetul pornit, fixarea, eliminarea unei linii și Game Over se aud distinct.
- [ ] Cu sunetul oprit, aceleași momente rămân fără sunet.
- [ ] Lipsa unui fișier audio lasă aplicația deschisă.
- [ ] În pauză, muzica de fundal este oprită, dacă muzica există.

---

## Epic: Calitate și predare

### TASK 33 [M] — Teste automate pentru logică și salvare

**Titlu:** Teste pentru piese, puț, scor, generator, partidă și fișiere  
**Etichete:** `must` `tests`  
**Epic:** Calitate și predare  
**Depinde de:** TASK 06, TASK 08, TASK 12, TASK 13, TASK 15, TASK 25

**Descriere:**

Proiectul de teste acoperă comportamentul din taskurile de logică și de salvare. Testele de fișiere folosesc un director temporar. Suita trece în întregime.

**Acceptance criteria:**

- [ ] Rotația, cele patru celule și copia independentă a piesei sunt verificate pentru toate tipurile.
- [ ] Sunt verificate coliziunea cu pereții, cu baza și cu piesele fixate, plus fixarea pe celulele corecte.
- [ ] Sunt verificate eliminarea a 1, 2, 3 și 4 linii, liniile neadiacente și coborârea rândurilor de deasupra.
- [ ] Sunt verificate punctele pe linii și pe nivel, creșterea nivelului la 10 linii, intervalul de cădere, soft drop și hard drop.
- [ ] Pentru un generator cu aceeași sursă, fiecare grup de șapte piese conține toate tipurile.
- [ ] Este verificat sfârșitul partidei când piesa nouă nu are loc și blocarea celui de-al doilea Hold.
- [ ] Sunt verificate salvarea, încărcarea, fișierul lipsă și fișierul invalid pentru scoruri și setări.
- [ ] `dotnet test` se termină fără teste eșuate.

### TASK 34 [M] — Verificare manuală

**Titlu:** Scenariile T-01 … T-10 și o partidă lungă, cu rezultatul notat  
**Etichete:** `must` `qa`  
**Epic:** Calitate și predare  
**Depinde de:** TASK 17, TASK 20, TASK 26

**Descriere:**

Echipa parcurge scenariile de mai jos pe aplicația rulată și notează rezultatul fiecăruia. Separat, parcurge o partidă de 15–20 de minute, pauză și reluare repetată, repornire în timpul partidei, ștergerea fișierelor JSON între rulări și mutarea ferestrei în timpul jocului. Orice defect găsit devine issue nou, cu pașii prin care se reproduce.

| ID | Situație | Rezultat așteptat |
|---|---|---|
| T-01 | Pornirea aplicației | Apare meniul principal |
| T-02 | Joc nou și ← la marginea din stânga | Piesa rămâne în puț |
| T-03 | O linie completă la nivel 1 | Linia dispare, scorul crește cu 100 |
| T-04 | Patru linii cu piesa I | Scorul crește cu 800 × nivelul |
| T-05 | P în timpul partidei, apoi P din nou | Partida se oprește, apoi continuă |
| T-06 | Puțul se umple | Apare Game Over cu scorul final |
| T-07 | Scor care intră în top 10 | Se cere numele și apare în clasament |
| T-08 | `highscores.json` șters înainte de pornire | Aplicația pornește, clasamentul este gol |
| T-09 | Hard drop | Piesa aterizează imediat, +2 puncte pe celulă |
| T-10 | Hold de două ori la rând | A doua comandă rămâne fără efect |

**Acceptance criteria:**

- [ ] T-01 … T-10 au rezultat notat, iar fiecare scenariu obligatoriu are rezultatul așteptat.
- [ ] Partida lungă, pauza repetată, repornirea, fișierele șterse și mutarea ferestrei sunt notate.
- [ ] Defectele găsite există ca issue-uri, cu pașii de reproducere.

### TASK 35 [M] — Defecte rămase și limite de rulare

**Titlu:** Defectele cunoscute sunt închise, iar jocul ține 60 FPS  
**Etichete:** `must` `qa`  
**Epic:** Calitate și predare  
**Depinde de:** TASK 34

**Descriere:**

Toate defectele deschise de verificarea manuală sunt corectate. Pe un calculator cu 4 GB RAM și procesor dual-core, jocul rămâne la cel puțin 60 FPS, pornește în mai puțin de 3 secunde și rămâne sub 150 MB. Desenarea rămâne stabilă pe durata unei partide. O eroare la citirea sau scrierea fișierelor lasă aplicația deschisă.

**Acceptance criteria:**

- [ ] Issue-urile deschise din TASK 34 sunt închise, iar scenariul care le-a găsit are rezultatul așteptat.
- [ ] Partida se menține la cel puțin 60 FPS.
- [ ] Meniul apare în mai puțin de 3 secunde de la pornire.
- [ ] Procesul rămâne sub 150 MB în timpul unei partide.
- [ ] Un fișier de scor lipsă sau invalid lasă aplicația deschisă.

### TASK 36 [M] — Revizuirea codului

**Titlu:** Fiecare parte este revizuită de un alt membru  
**Etichete:** `must` `review`  
**Epic:** Calitate și predare  
**Depinde de:** TASK 33

**Descriere:**

Fiecare membru revizuiește munca altui membru. Codul respectă convențiile C# din soluție, tipurile publice sunt documentate, iar logica din `Core` rămâne independentă de Windows Forms și de `System.Drawing`.

**Acceptance criteria:**

- [ ] Fiecare pull request de funcționalitate are o aprobare de la un alt membru.
- [ ] Tipurile și membrii publici folosesc PascalCase, iar parametrii și variabilele locale folosesc camelCase.
- [ ] Tipurile publice au un rezumat XML.
- [ ] `TetrisGame.Core` se compilează fără referință la Windows Forms sau la `System.Drawing`.

### TASK 37 [M] — Documentație și predare

**Titlu:** README, build Release și arhivă care pornește pe alt calculator  
**Etichete:** `must` `release`  
**Epic:** Calitate și predare  
**Depinde de:** TASK 35, TASK 36

**Descriere:**

README-ul explică jocul, cerința de .NET Desktop Runtime 10, compilarea, pornirea, tastele și structura soluției. Varianta Release a interfeței pornește pe un calculator diferit de cel de dezvoltare, care are Desktop Runtime 10. Arhiva de predare conține sursele, aplicația publicată, SRS-ul, README-ul și capturi cu meniul, partida și clasamentul. `main` are tag-ul `v1.0`.

**Acceptance criteria:**

- [ ] O persoană din afara echipei pornește jocul urmând doar README-ul.
- [ ] Aplicația publicată în Release pornește pe un calculator cu .NET Desktop Runtime 10, fără SDK.
- [ ] Arhiva conține sursele, aplicația publicată, SRS-ul, README-ul și capturile meniului, partidei și clasamentului.
- [ ] Tag-ul `v1.0` există pe `main`.
