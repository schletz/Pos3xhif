# Lehrinhalte auf Basis von Microsoft .NET und C#

![](dotnet-bot.png)

Gem. [Lehrplan BGBl. II Nr. 262/2015](https://www.ris.bka.gv.at/GeltendeFassung.wxe?Abfrage=Bundesnormen&Gesetzesnummer=20009288)
für den 3. Jahrgang. Die *kursiv* gedruckten Teile in der Spalte Lehrplan kennzeichnen die wesentlichen
Punkte im Sinne der LBVO.

Beachte bei der Umsetzung des Lehrplanes: Die Inhalte kommen noch aus der pre .NET Zeit (ausgehendes 20. Jhd.).
Daher werden auch Konzepte, die eigentlich im IV. Jahrgang vorkommen (OR Mapper, Parallele Programmierung, Reflection)
verwendet, da sie durch das .NET Framework wesentlich einfacher zugänglich sind.

<table>
    <tr><th>Inhalt</th><th>Lehrplan</th></tr>
    <tr><td colspan="2"><strong>Wintersemester</strong></td></tr>
    <tr>
        <td>
            - OOP Grundlagen in C# (Properties, Vererbung)<br>
            - Basisklassen und Interfaces im .NET Framework (Collections, IEquatable, ICompareable)<br>
            - LINQ (das IEnumerable Interface, Navigation Properties, Method Syntax und Lambda Expressions, Query Syntax, Gruppierung)<br>
            - LINQ to XML, Newtonsoft JSON (JArray und JObject mit LINQ, JsonConvert )
        </td>
        <td>
            <em>Polymorphie, generische Datentypen und Programmiertechniken, Serialisierung von Objekten</em>, 
            Versionsverwaltung, Teststrategien, Unit-Tests.
        </td>
    <tr>
        <td>
            - WPF und XAML (Syntax, Controls, Code-Behind, Eventhandler, Binding an Controlwerte)<br>
            - Umsetzung des MVVM Patterns in WPF Teil 1 (ViewModel, Binding, Commands)
        </td>
        <td>
             <em>Elemente von graphischen Benutzeroberflächen, Design, Layout</em>, Usability,  <em>Eventhandling.</em>
        </td>
    </tr>
    <tr><td colspan="2"><strong>Sommersemester</strong></td></tr>
    <tr>
        <td>
            Umsetzung des MVVM Patterns in WPF Teil 2 (Aktualisierung, Listviews, Menüs, User Controls, States)
        </td>
        <td>
            <em>Graphische Benutzerschnittstellen mit Validierung der Benutzereingaben und Fehlerbehandlung.</em>
        </td>
    <tr>
        <td>
            Das Entity Framework, Erstellen von Modelklassen, Verwendung in WPF.
        </td>
        <td>
            <em>Zugriffe auf Datenbanken.</em>
        </td>
    </tr>
    <tr>
        <td>
            TPL (Task Parallel Library) in C# (Task Scheduler, Parallel, await/async, lock, SemaphoreSlim, TPL Dataflow)
        </td>
        <td>
             <em>Threads</em>, Lebenszyklus, Race Conditions
        </td>
    </tr>    
</table>