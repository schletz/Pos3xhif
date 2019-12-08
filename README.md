# POS im III. Jahrgang der HTL Spengergasse

![](dotnet-bot.png)

## Wichtiges zum Start

1. [Anleitung zum Verbinden mit dem VPN und den Laufwerken](VpnSpengergasse.md)
1. [Installation von Visual Studio 2019](VisualStudioInstallation.md)
1. [Markdown Editing mit VS Code](markdown.md)
1. Optional: [Installation von SQL Server 2017](SqlServerInstallation.md)

## Lehrinhalte auf Basis von Microsoft .NET und C#
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


## Weiterführende Unterlagen:
- Moissenböck: C# (JKU Linz) http://dotnet.jku.at/courses/csharp/download/Agree.aspx?ReturnUrl=%2fcourses%2fcsharp%2fdownload%2fSlides.aspx
- C# 7 in a Nutshell: http://2.238.16.249:8880/get/pdf/106

## Synchronisieren des Repositories in einen Ordner
1. Lade von https://git-scm.com/downloads die Git Tools (Button *Download 2.20.1 for WIindows*)
    herunter. Es können alle Standardeinstellungen belassen werden, bei *Adjusting your PATH environment*
    muss aber der mittlere Punkt (*Git from the command line [...]*) ausgewählt sein.
2. Lege einen Ordner auf der Festplatte an, wo du die Daten speichern möchtest 
    (z. B. *C:\Schule\POS\Examples*). Das
    Repository ist nur die lokale Version des Repositories auf https://github.com/schletz/Pos3xhif.git.
    Hier werden keine Commits gemacht und alle lokalen Änderungen dort werden bei der 
    nächsten Synchronisation überschrieben.
3. Initialisiere den Ordner mit folgenden Befehlen, die du in der Konsole in diesem Verzeichnis
    (z. B. *C:\Schule\POS\Examples*) ausführst:
```bash {.line-numbers}
git init
git remote add origin https://github.com/schletz/Pos3xhif.git
```

4. Lege dir in diesem Ordner eine Datei *syncGit.cmd* mit folgenden Befehlen an. 
    Durch Doppelklick auf diese Datei wird immer der neueste Stand geladen. Neu erstellte Dateien
    in diesem Ordner bleiben auf der Festplatte, geänderte Dateien werden allerdings durch 
    *git reset* auf den Originalstand zurückgesetzt.
```bash {.line-numbers}
git fetch --all
git reset --hard origin/master
```

## Optional: anlegen eines eigenen Repositories
1. Lege dir auf [GitHub] einen Zugang an. Über *Repositories* kannst du dir ein neues Repository mit
    dem Namen *POS* (oder anders) anlegen. Nach dem Anlegen des Repositories erscheint eine URL,
    die du dann beim Initialisieren noch brauchen wirst.

2. Lege einen Ordner auf der Festplatte an, wo du dein lokales Work Repository speichern möchtest 
    (z. B. *C:\Schule\POS\Work*). Der Example Ordner darf kein Unterverzeichnis in diesem Ordner sein.

3. Setze in der Konsole deinen Namen und deine Mailadresse in den globalen Einstellungen deiner
   git Installation:
```bash {.line-numbers}
git config --global user.name "FIRST_NAME LAST_NAME"
git config --global user.email "MY_NAME@example.com"
```

4. Initialisiere dein Work Repository mit folgenden Befehlen. Statt *(URL)* schreibe die URL deines
    auf Github angelegten Repositories hinein (z. B. *https://github.com/username/POS.git*)
```bash {.line-numbers}
git init
git remote add origin (URL)
```
5. Lege dir in diesem Ordner eine Datei *syncGit.cmd* mit folgenden Befehlen an. Durch Doppelklick
    auf diese Datei im Explorer werden alle Änderungen bestätigt ("Commit") und der Inhalt mit dem
    Online Repository auf Github synchronisiert.
```bash {.line-numbers}
git add -A
git commit -a -m "Commit"
git pull origin master --allow-unrelated-histories
git push origin master
```

6. Erstellen von .gitignore: Damit nicht Builds und temporäre Dateien von Visual Studio hochgeladen werden, gibt es im Example
Repository eine Datei *.gitignore*. Kopiere diese Datei im Explorer aus deinem *Examples* Verzeichnis
in das *Work* Verzeichnis. Bei der Synchronisation wird nun die Datei übertragen. Ab jetzt werden die
temporären Dateien von Visual Studio zwar lokal gespeichert, aber nicht übertragen.

7. Zur Dokumentation wird im Programmierbereich die sogenannte Markdown Syntax verwendet. Sie definiert
    Formatierungsanweisungen in Textdateien. Eine Übersicht ist unter
    https://help.github.com/articles/basic-writing-and-formatting-syntax/ abrufbar. 

    Mit der Extension *Markdown Editor* kannst du in Visual Studio unter *Tools* - *Extensions and Updates* solche Dateien
    mit einer Voransicht entwerfen. In Chrome gibt es die Extension *Markdown Reader* für die Anzeige
    von lokalen md Dateien, wenn der Extension der Zugriff auf das *file://* Protokoll gestattet wurde.
    

[GitHub]: https://github.com

