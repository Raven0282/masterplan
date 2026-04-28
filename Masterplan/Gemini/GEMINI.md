# Gemini AI Collaboration Standards: Masterplan & MasterplanXP

## 1. Project Overview
*   **Original Masterplan (OMP):** D&D 4E design studio. WinForms, .NET 8 (Targeting .NET 10 migration). Uses legacy `BinaryFormatter`.
*   **MasterplanXP (MPXP):** Cross-platform port. AvaloniaUI, .NET 10, Clean Architecture, MVVM.
*   **The Bridge:** MessagePack-based serialization using DTOs to replace `BinaryFormatter`.

## 2. Architectural Mandates
*   **Clean Architecture (MPXP):**
    *   **Domain:** Logic-heavy classes (Creature, Encounter) must remain framework-agnostic.
    *   **Application:** Use CommunityToolkit.Mvvm Messenger for command/query separation. MediatR was explored but introduced other complexity that was not essential.
    *   **Infrastructure:** Data persistence (MessagePack/JSON) belongs here. Export to XML to match the existing files from the Offline Character Builder and Monster Builder is a desired export format.
*   **MVVM (MPXP):** Strict separation of View (XAML) and ViewModel. Use CommunityToolkit.Mvvm.
*   **Dependency Injection:** All services (Serialization, Logging, Discovery) must be injected via constructor.

## 3. Serialization Standards (Parallel Track Workflow)
*   **Format:** MessagePack (.mpxpl for libraries, .mpxpm for projects).
*   **Workflow Logic:**
    1.  **Prioritization:** OMP prioritized modern MessagePack formats first. It only fallbacks to legacy BinaryFormatter if the modern file is missing.
    2.  **Auto-Staging:** Upon loading a legacy file, OMP immediately stages a conversion copy in a `Converted/` sub-folder using the DTO Bridge.
    3.  **Persistence:** Until full validation is complete, all "Save" operations must remain in the legacy `BinaryFormatter` format to ensure no production data loss.
*   **DTO Bridge:** Bidirectional mapping is handled via `LibraryConversionService`. Never serialize Domain objects directly.
*   **Consistency:** All sub-collections (Powers, Auras, Traps, PlotPoints) must be deeply mapped.
*   **Images:** Store as `byte[]` in DTOs to ensure cross-platform compatibility (avoiding GDI+ dependencies in MPXP).

## 4. Coding Standards & Style
*   **C# 12+ Features:** Embrace primary constructors, collection expressions, and file-scoped namespaces.
*   **Nullability:** `#nullable enable` is mandatory for all new MPXP code.
*   **Naming:** PascalCase for properties/methods, camelCase with `f` prefix for private fields (legacy OMP) or `_` (new MPXP).
*   **Async/Await:** All I/O operations (file load/save) must be asynchronous.
*   **Code Integrity:** Before writing to files, a property-by-property review against the original domain object and schema discovery files (`MasterSchema.txt`, `Project.txt`) is mandatory to ensure no data fields are omitted. Additionally, verify that properties are writable (have a 'set' accessor) before attempting to assign to them during DTO-to-Domain mapping. Calculated or read-only properties (e.g., 'Note.Name', 'EncounterCard.XP') must be excluded from object initializers.

## 5. Testing & Validation
*   **Unit Testing:** xUnit for logic validation. Every DTO mapper must have a "Round-trip" test (Legacy -> DTO -> MessagePack -> DTO -> Legacy).
*   **UI Testing:** Avalonia.Headless for ViewModel/View interaction testing.
*   **CI/CD:** GitHub Actions recommended for build validation and automated testing on every PR.

## 6. Project Maturity & Status
*   **Serialization Progress:** *Active Implementation (High Confidence)* - Bidirectional DTO mapping for Library and Project is implemented. Mapping parity is based on `MasterSchema.txt`.
*   **Database Future:** *Suggested* - SQLite via Entity Framework Core is highly recommended for MPXP to handle large collections efficiently.
*   **Add-in System:** *Inferred (Medium Confidence)* - The `Extensibility` folder suggests a plugin architecture to be modernized in MPXP.

## 7. AI Interaction Guidelines
*   **Context:** Always reference `MasterSchema.txt` and `Project.txt` when generating mapping code.
*   **Surgical Edits:** When modifying OMP for .NET 10 compatibility, focus on removing `[Serializable]` and `BinaryFormatter` calls in favor of the new `Serialisation<T>` wrapper.
