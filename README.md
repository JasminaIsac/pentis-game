# Pentis

Joc Pentis pentru Windows, scris în C# cu .NET 10 și Windows Forms. Piese din 5 pătrate (12 pentomino), puț 10×20.

## Cerințe

- Windows 10 sau Windows 11
- [.NET Desktop Runtime 10](https://dotnet.microsoft.com/download/dotnet/10.0) pentru a rula aplicația
- .NET SDK 10 pentru a compila soluția

## Rulare

```bash
dotnet run --project PentisGame.UI
```

Aplicația pornește în meniul principal.

## Compilare și teste

```bash
dotnet build PentisGame.slnx
dotnet test PentisGame.slnx
```

## Structura soluției

- `PentisGame.Core` — logica jocului, fără Windows Forms
- `PentisGame.Data` — salvarea scorurilor și a setărilor
- `PentisGame.UI` — ferestrele și desenarea; proiectul de pornire
- `PentisGame.Tests` — teste xUnit

Valorile fixe ale jocului (dimensiunea puțului, viteza, punctajul) stau în `GameConstants`, în proiectul `Core`.
