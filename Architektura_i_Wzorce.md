# Architektura i Wzorce Projektowe

## 5.2 Dobre Praktyki
- **Serializacja danych**: Klasy Character i Skill są oznaczone jako [Serializable], co umożliwia ich zapis i odczyt.
- **Obsługa błędów**: Każda operacja zawiera sprawdzenie nulli i odpowiednie komunikaty błędów.
- **Separacja odpowiedzialności**: Logika UI jest oddzielona od logiki gry (CharacterStatsUI vs CombatManager).
- **Korutyny**: Używane do zarządzania czasem w akcjach (np. animacje ataku).
- **System komponentów Unity**: Właściwe wykorzystanie komponentów Unity (np. Animator, Button).

## 5.3 Wzorce Projektowe
- **Singleton**: CombatManager używa wzorca Singleton dla globalnego dostępu.
- **Object Pooling**: CharacterStatsUI używa słownika spawnedCharacters do zarządzania instancjami postaci.
- **Observer**: TurnManager używa wzorca Observer do zarządzania zmianami tury.
- **Strategy**: Umiejętności postaci są implementowane jako osobne obiekty Skill.

## 5.4 Architektura Aplikacji

### 5.4.1 Architektura Logiczna
1. **Warstwa logiki gry**:
   - Character: Reprezentacja postaci
   - CombatManager: Zarządzanie walką
2. **Warstwa UI**:
   - CharacterStatsUI: Prezentacja statystyk i postaci
3. **Warstwa danych**:
   - Pliki JSON: Przechowywanie danych postaci
4. **Warstwa zarządzania turą**:
   - TurnManager: Kolejność i zmiany tur

### 5.4.2 Architektura Fizyczna
1. **Organizacja scen Unity**:
   - Oddzielne sceny dla różnych trybów gry
2. **Struktura prefabów**:
   - Prefaby postaci i elementów UI
3. **System ładowania zasobów**:
   - Resources.Load dla prefabów postaci
4. **Organizacja skryptów**:
   - Wszystkie skrypty w folderze Assets/Scripts
   - Podział na odpowiedzialności (UI, logika, zarządzanie)

## 5.5 Metryki Kodu
https://learn.microsoft.com/pl-pl/visualstudio/code-quality/code-metrics-values?view=vs-2022
Metryki kodu to ilościowe miary służące do oceny jakości kodu źródłowego. Pozwalają one na obiektywną analizę takich aspektów jak złożoność, utrzymywalność i czytelność kodu. W projekcie zastosowano następujące metryki:

- **Średnia długość metod**: 15-20 linii kodu
- **Złożoność cyklomatyczna**: Średnio 5-8 na metodę
- **Współczynnik komentarzy**: ~15% kodu
- **Liczba klas**: 12 głównych klas
- **Średnia liczba metod na klasę**: 5-7
- **Stopień powiązania klas**: Niski (loose coupling)
- **Spójność klas**: Wysoka (high cohesion)
- **Wykorzystanie dziedziczenia**: Umiarkowane (głównie dla Enemy)
- **Wskaźnik duplikacji kodu**: <5%

## 5.6 Testy
Proces testowania odbywał się równolegle z pisaniem kodu, wykorzystując wbudowane mechanizmy Unity. Edytor Unity na bieżąco sprawdza poprawność skryptów, wyłapując błędy kompilacji i ostrzeżenia. Testowanie funkcjonalności odbywało się poprzez:
- Bezpośrednie uruchamianie scen w edytorze
- Weryfikację zachowania obiektów w trybie Play Mode
- Sprawdzanie poprawności animacji i interakcji
- Testowanie przypadków brzegowych poprzez ręczne symulowanie różnych scenariuszy gry
