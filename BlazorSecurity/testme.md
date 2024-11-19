# Manual Test Guide - To-Faktor Godkendelse

Denne guide beskriver hvordan du manuelt tester to-faktor godkendelse i udviklingmiljøet.

## Forudsætninger
* Google Authenticator app på din telefon
    * Download fra [App Store](https://apps.apple.com/us/app/google-authenticator/id388497605) (iOS)
    * Download fra [Google Play](https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2) (Android)
* Projektet klonet og sat op lokalt (se WSL setup guide)
* En webbrowser (Chrome, Firefox, etc.)

## Test Procedure

### 1. Start Projektet
1. Åbn projektet i Visual Studio
2. Vælg "https:test" profilen i dropdown menuen
3. Start projektet (F5 eller klik på "Start Debugging")

### 2. Opret Bruger
1. Klik på "Register" i navigationsbaren
2. Udfyld registreringsformularen:
    * Email
    * Password
    * Bekræft password
3. Klik på "Register" knappen

### 3. Bekræft Email
1. I browseren vil du se en bekræftelsesside
2. Klik på "Confirm" linket
   > **BEMÆRK**: I testmiljøet simuleres email-bekræftelse direkte i browseren

### 4. Database Migration
Vælg **én** af følgende metoder:

#### Metode A: Via Browser
1. Find "Apply Migrations" knappen i browseren
2. Klik på knappen for at udføre database migrationen

#### Metode B: Via Package Manager Console
1. Åbn Package Manager Console i Visual Studio
    * Tools > NuGet Package Manager > Package Manager Console
2. Kør følgende kommando:
   ```powershell
   Update-Database
   ```

### 5. To-Faktor Login Test
1. Gå til login siden
2. Indtast dine legitimationsoplysninger:
    * Email
    * Password
3. Når du bliver bedt om det, åbn Google Authenticator på din telefon
4. Scan QR-koden eller indtast den viste nøgle
5. Indtast den 6-cifrede kode fra Google Authenticator
6. Klik på "Login" knappen

### Forventet Resultat
* Du skulle nu være logget ind i systemet med to-faktor godkendelse aktiveret

## Fejlfinding

### Almindelige Problemer

#### QR-kode Scanner ikke
1. Prøv at bruge den manuelle nøgle i stedet
2. Sørg for at have god belysning
3. Hold telefonen stille

#### Database Migrations Fejl
1. Sørg for at alle tidligere migrationer er kørt
2. Prøv at slette databasen og køre migrations igen:
   ```powershell
   Drop-Database
   Update-Database
   ```
   Eller brug "Apply Migrations" knappen i browseren igen

## Support
Ved tekniske problemer, kontakt udviklingsteamet eller opret et issue i Git repositoriet.