# MovieMatch

Projekt ASP.NET Core MVC do wyszukiwania filmów i seriali (TMDB) z kontami użytkowników (ASP.NET Core Identity) i funkcją "Moja lista" (ulubione / do obejrzenia).

## Uruchomienie

1. Otwórz rozwiązanie `MovieMatch.sln` w Visual Studio 2022.
2. Przywróć pakiety NuGet (dzieje się automatycznie przy pierwszym buildzie).
3. Ustaw klucz TMDB:

   **Opcja A (polecana – User Secrets):**
   - PPM na projekt `MovieMatch` → **Manage User Secrets**
   - Wklej:
     ```json
     {
       "Tmdb": {
         "ApiKey": "TWÓJ_KLUCZ_TMDB"
       }
     }
     ```

   **Opcja B (szybka):**
   - W `appsettings.json` podmień `Tmdb:ApiKey`.

4. Baza danych (Identity):
   - `Tools` → `NuGet Package Manager` → `Package Manager Console`
   - Upewnij się, że Default project to `MovieMatch`
   - Wykonaj:
     ```powershell
     Update-Database
     ```

5. Uruchom (IIS Express / Kestrel).

## Struktura

- `Controllers/` – logika MVC
- `Services/` – integracja TMDB
- `Areas/Identity` – logowanie/rejestracja/profil
- `Views/` – widoki
- `wwwroot/css/site.css` – globalne style
- `wwwroot/css/pages/*` – style per widok (wyniesione z plików `.cshtml` dla czytelności)

## Notatki do oddania projektu

- W repozytorium celowo nie przechowujemy prawdziwego klucza API (w `appsettings.json` jest placeholder).
- Kod i style są ujednolicone (format, usunięte zbędne pliki, brak globalnych "psujących" selektorów CSS).