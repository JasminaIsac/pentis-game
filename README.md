# Tetris

Joc Tetris pentru Windows, scris în C# cu .NET 10 și Windows Forms.

## Cerințe

- Windows 10 sau Windows 11
- [.NET Desktop Runtime 10](https://dotnet.microsoft.com/download/dotnet/10.0) pentru a rula aplicația
- .NET SDK 10 pentru a compila soluția

## Rulare

```bash
dotnet run --project TetrisGame.UI
```

Aplicația pornește în meniul principal.

## Compilare și teste

```bash
dotnet build TetrisGame.slnx
dotnet test TetrisGame.slnx
```

## Structura soluției

- `TetrisGame.Core` — logica jocului, fără Windows Forms
- `TetrisGame.Data` — salvarea scorurilor și a setărilor
- `TetrisGame.UI` — ferestrele și desenarea; proiectul de pornire
- `TetrisGame.Tests` — teste xUnit

Valorile fixe ale jocului (dimensiunea puțului, viteza, punctajul) stau în `GameConstants`, în proiectul `Core`.
