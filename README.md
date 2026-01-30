# 🎬 MovieMatch

**MovieMatch** to aplikacja webowa stworzona w technologii **ASP.NET Core MVC**, umożliwiająca wyszukiwanie filmów i seriali z wykorzystaniem zewnętrznego API (TMDB) oraz zarządzanie listą ulubionych i pozycji „do obejrzenia” przez zalogowanych użytkowników.

Projekt został zrealizowany jako **projekt edukacyjny**, z naciskiem na praktyczne wykorzystanie architektury MVC, integrację z API zewnętrznym oraz pracę z kontami użytkowników.

🔗 **Demo aplikacji:**  
https://moviematch-production-bc2c.up.railway.app/

---

## 🧩 Funkcjonalności

- Wyszukiwanie filmów i seriali z wykorzystaniem **TMDB API**
- Wyświetlanie szczegółów filmów i seriali
- Rejestracja i logowanie użytkowników (**ASP.NET Core Identity**)
- Lista użytkownika:
  - ulubione
  - do obejrzenia
- Podstawowa walidacja danych wejściowych
- Rozdzielenie logiki aplikacji od warstwy widoku (MVC)
- Wdrożenie aplikacji na platformie **Railway**

---

## 🛠️ Wykorzystane technologie

- C#
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQL
- HTML, CSS
- TMDB API
- Git / GitHub
- Railway (deployment)

---

## 📁 Struktura projektu

Projekt oparty jest na klasycznej architekturze **MVC**:
```text
MovieMatch/
│
├── Controllers/
│ ├── HomeController.cs
│ ├── MoviesController.cs
│ ├── SeriesController.cs
│ └── ...
│
├── Services/
│ ├── TmdbService.cs // komunikacja z TMDB API
│ └── ...
│
├── Models/
│ ├── Movie.cs
│ ├── Series.cs
│ ├── UserMovie.cs
│ └── ...
│
├── Data/
│ ├── ApplicationDbContext.cs
│ └── Migrations/
│
├── Areas/
│ └── Identity/ // logowanie, rejestracja, profil użytkownika
│
├── Views/
│ ├── Home/
│ ├── Movies/
│ ├── Series/
│ └── Shared/
│
├── wwwroot/
│ ├── css/
│ │ ├── site.css // style globalne
│ │ └── pages/ // style per widok
│ └── js/
│
├── appsettings.json
├── Program.cs
└── MovieMatch.sln
```
---

## 🚀 Uruchomienie projektu lokalnie

### 1️⃣ Wymagania

- Visual Studio 2022
- .NET SDK (zgodny z projektem)
- Dostęp do Internetu (TMDB API)

---

### 2️⃣ Klonowanie repozytorium

`bash`
git clone https://github.com/Krystianh14/MovieMatch.git

---

### 3️⃣ Konfiguracja klucza TMDB API

Aplikacja korzysta z zewnętrznego API serwisu The Movie Database (TMDB).
Klucz API można uzyskać po założeniu darmowego konta na stronie:

👉 https://www.themoviedb.org/

Po wygenerowaniu klucza należy dodać go do projektu w jeden z poniższych sposobów.

🔹 **Opcja A** (REKOMENDOWANA) – User Secrets

1. Kliknij PPM na projekt MovieMatch

2. Wybierz Manage User Secrets

3. Wklej:
   ```
   {
   "Tmdb": {
   "ApiKey": "TWÓJ_KLUCZ_TMDB"
   }}
**Opcja B** – appsettings.json (szybka)

W pliku appsettings.json:
```
"Tmdb": {
"ApiKey": "TWÓJ_KLUCZ_TMDB"
}
```
### 4️⃣ Baza danych (Identity)

Projekt wykorzystuje Entity Framework Core.

1. Otwórz:
```
Tools → NuGet Package Manager → Package Manager Console
```
2. Upewnij się, że Default project to MovieMatch

3. Wykonaj polecenie:
```
Update-Database
```
### 5️⃣ Uruchomienie aplikacji

Uruchom projekt w środowisku Visual Studio (lub innym kompatybilnym IDE)

Po uruchomieniu aplikacja będzie dostępna lokalnie w przeglądarce.

**🌐 Deployment**

Aplikacja została wdrożona na platformie Railway.
Proces obejmował:

- konfigurację zmiennych środowiskowych,

- połączenie repozytorium GitHub,

- testy działania po publikacji.

**ℹ️ Informacje dodatkowe**

- Klucz API nie jest przechowywany w repozytorium

- Style CSS zostały rozdzielone na:

  - globalne (site.css)

  - per widok (wwwroot/css/pages)


```

```
