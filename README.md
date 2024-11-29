# VäderAppProd

Ett C#-konsolprogram för att analysera väderdata, med fokus på temperatur, luftfuktighet och mögelrisk.

## Funktioner
- Visa medeltemperatur för ett valt datum (utomhus/inomhus).
- Sortera varmaste och torraste dagar (utomhus/inomhus).
- Visa meteorologisk höst och vinter baserat på temperaturdata.
- Identifiera dagar med störst mögelrisk (utomhus/inomhus).

## Krav
- **.NET 6.0 eller högre** installerat på datorn.
- **CSV-filen** (`TempFuktData.csv`) placerad i `bin\Debug\net8.0`. I mitt fall blir det `C:\Users\Malek Mustafa\source\repos\VäderAppProd\bin\Debug\net8.0\TempFuktData.csv`.

## Installation och körning
1. Klona eller ladda ner projektet.
2. Öppna projektet i din IDE (t.ex. Visual Studio).
3. Bygg och kör projektet.
4. Följ menyerna i konsolen för att använda funktionerna.

## Installation av NuGet-paket
För att säkerställa att alla beroenden är installerade:

1. När du öppnar projektet i Visual Studio, kör:
   - **`Build > Build Solution` (Ctrl + Shift + B)**.
2. Visual Studio laddar automatiskt ner och installerar alla nödvändiga NuGet-paket.
3. Om det inte fungerar, öppna **Package Manager Console** (`Tools > NuGet Package Manager > Package Manager Console`) och kör:
   ```bash
   dotnet restore
   ```
4. Om du inte använder Visual Studio, kör kommandot `dotnet restore` i terminalen där projektet finns.

## Filstruktur
```
VäderAppProd/
├── Program.cs
├── VäderData.cs
├── VäderDataMap.cs
├── VäderDBContext.cs
├── README.md
├── Dependencies/
```

## CSV-format
CSV-filen (`TempFuktData.csv`) måste ha följande kolumner:
- **Datum**: Datum och tid i formatet `yyyy-MM-dd H:mm`.
- **Plats**: Anger om mätningen är "Ute" eller "Inne".
- **Temp**: Temperaturen i grader Celsius.
- **Luftfuktighet**: Luftfuktigheten i procent.

## Exempeldata
```csv
Datum,Plats,Temp,Luftfuktighet
2016-11-15,Ute,2.1,85
2016-11-14,Inne,22.5,40
2016-11-13,Ute,1.3,88
```

## Författare
- Malek Mustafa
- Sabri Said