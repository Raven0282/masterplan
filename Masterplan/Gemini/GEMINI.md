GEMINI.md

    1 # Gemini AI Collaboration Standards: Masterplan & MasterplanXP
    2
    3 ## 1. Project Overview
    4 *   **Original Masterplan (OMP):** D&D 4E design studio. WinForms, .NET 8 (Targeting .NET 10 migration). Uses
      legacy `BinaryFormatter`.
    5 *   **MasterplanXP (MPXP):** Cross-platform port. AvaloniaUI, .NET 10, Clean Architecture, MVVM.
    6 *   **The Bridge:** MessagePack-based serialization using DTOs to replace `BinaryFormatter`.
    7
    8 ## 2. Architectural Mandates
    9 *   **Clean Architecture (MPXP):**
   10     *   **Domain:** Logic-heavy classes (Creature, Encounter) must remain framework-agnostic.
   11     *   **Application:** Use MediatR or similar patterns for command/query separation.
   12     *   **Infrastructure:** Data persistence (MessagePack/JSON) belongs here.
   13 *   **MVVM (MPXP):** Strict separation of View (XAML) and ViewModel. Use ReactiveUI or CommunityToolkit.Mvvm.
   14 *   **Dependency Injection:** All services (Serialization, Logging, Discovery) must be injected via constructor.
   15
   16 ## 3. Serialization Standards (Core Task)
   17 *   **Format:** MessagePack (.mpxl / .xLibrary).
   18 *   **DTO Pattern:** Never serialize Domain objects directly. Map `Data/` objects to `Dto/` models.
   19 *   **Consistency:** Use `TypelessContractlessStandardResolver` for polymorphic interfaces (IRole, IElement).
   20 *   **Images:** Store as `byte[]` in DTOs to ensure cross-platform compatibility (avoiding GDI+ dependencies in
      MPXP).
   21
   22 ## 4. Coding Standards & Style
   23 *   **C# 12+ Features:** Embrace primary constructors, collection expressions, and file-scoped namespaces.
   24 *   **Nullability:** `#nullable enable` is mandatory for all new MPXP code.
   25 *   **Naming:** PascalCase for properties/methods, camelCase with `f` prefix for private fields (legacy OMP) or
      `_` (new MPXP).
   26 *   **Async/Await:** All I/O operations (file load/save) must be asynchronous.
   27
   28 ## 5. Testing & Validation
   29 *   **Unit Testing:** xUnit for logic validation. Every DTO mapper must have a "Round-trip" test (Legacy -> DTO ->
      MessagePack -> DTO -> Legacy).
   30 *   **UI Testing:** Avalonia.Headless for ViewModel/View interaction testing.
   31 *   **CI/CD:** GitHub Actions recommended for build validation and automated testing on every PR.
   32
   33 ## 6. Inferred Information & Confidence
   34 *   **Serialization Progress:** *Inferred (High Confidence)* - The current conversion service is a skeleton and
      needs a full-depth implementation of the DTO mapping logic.
   35 *   **Database Future:** *Suggested* - SQLite via Entity Framework Core is highly recommended for MPXP to handle
      the large collections of creatures and items more efficiently than flat files.
   36 *   **Add-in System:** *Inferred (Medium Confidence)* - The `Extensibility` folder suggests a plugin architecture
      that must be ported to a more modern `IPlugin` or `IServiceCollection` based system in MPXP.
   37
   38 ## 7. AI Interaction Guidelines
   39 *   **Context:** Always reference `MasterSchema.txt` when generating mapping code.
   40 *   **Surgical Edits:** When modifying OMP for .NET 10 compatibility, focus on removing `[Serializable]` and
      `BinaryFormatter` calls in favor of the new `Serialisation<T>` wrapper.
