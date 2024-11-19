# Guide til Opsætning af Udviklingsmiljø i WSL på Windows

> **VIGTIGT**: Denne guide forudsætter at du har Windows 10/11 og administratorrettigheder på din computer.

## Forudsætninger
* Windows 10/11
* Administratorrettigheder på din computer

## Trin-for-trin Guide

### 1. Installation af Visual Studio
1. Download den nyeste version af Visual Studio fra Microsofts hjemmeside
2. Kør installationsprogrammet
3. Vælg som minimum følgende workloads:
    * ASP.NET and web development
    * .NET desktop development
    * Linux development with C++

### 2. Installation af WSL og Ubuntu
1. Åbn PowerShell som administrator
2. Kør følgende kommando:
   ```powershell
   wsl --install
   ```
3. Genstart din computer
4. Efter genstart vil Ubuntu automatisk begynde at installere
5. Opret et brugernavn og kodeord når du bliver bedt om det

### 3. Kloning af Kildekode
1. Åbn Terminal i WSL
2. Naviger til den ønskede mappe
3. Klon repository'et:
   ```bash
   git clone [repository URL]
   ```

### 4. Åbning og Konfiguration i Visual Studio
1. Åbn .sln-filen i Visual Studio
2. Find dropdown-menuen til målplatform i værktøjslinjen
3. Vælg "WSL" fra dropdown-menuen
4. Vent på WSL-miljø konfiguration

## Fejlfinding

### Problem: Internetforbindelse/SDK Installation
Hvis du oplever problemer med at installere værktøjer eller SDK'er:

1. Åbn Terminal i WSL
2. Rediger resolv.conf filen:
   ```bash
   sudo nano /etc/resolv.conf
   ```
3. Ændr nameserver til:
   ```
   nameserver 8.8.8.8
   ```
4. Gem filen (Ctrl+O, Enter, Ctrl+X)

### Problem: Certifikatfejl
Følg disse trin ved certifikatfejl:

1. Gå til Tools > Options > Cross Platform > WSL i Visual Studio
2. Find certifikatsektionen
3. Klik "Export Certificate"
4. Ved eksportfejl, kør i WSL terminal:
   ```bash
   sudo mkdir -p /usr/local/share/ca-certificates
   sudo chmod 777 /usr/local/share/ca-certificates
   ```
5. Prøv certifikateksport igen

## Kørsel af Projekt

> **BEMÆRK**: Første kørsel kan tage ekstra tid pga. installation af værktøjer og afhængigheder.

1. Vælg WSL som målplatform i Visual Studio
2. Tryk F5 eller klik "Start Debugging"

## Nyttige Kommandoer

```bash
# Kontroller WSL version
wsl --version

# Liste installerede Linux-distributioner
wsl --list --verbose

# Genstart WSL
wsl --shutdown
```

## Support og Ressourcer
* [Microsoft WSL Dokumentation](https://docs.microsoft.com/windows/wsl/)
* [Visual Studio Dokumentation](https://docs.microsoft.com/visualstudio/)
* [Ubuntu WSL Guide](https://ubuntu.com/wsl)

> **TIP**: Ved fortsatte problemer, prøv at genstarte både Visual Studio og WSL.