# Await und Async: Task-Based Asynchronous Pattern 

Manchmal müssen in einem Programm zeitintensive Vorgänge einmalig gestartet werden. Meist sind dies I/O
Vorgänge wie
- das Laden von einer URL
- das Lesen einer (großen) Datei
- das Abfragen einer Datenbank
- das Lesen von einer Schnittstelle.

Allen Vorgängen gemein ist, dass sie im Vergleich zum Zugriff auf den Hauptspeicher sehr lange Zeit benötigen.
Damit das Ergebnis der Operation weiterverarbeitet werden kann, muss gewartet werden. Solange der Thread
aber in einer Warteschleife hängt, kann er keine anderen Vorgänge ausführen. Bei Benutzeroberflächen macht sich dies durch das
"einfrieren" bemerkbar.

In anderen Sprachen wie JavaScript gibt es das Konzept der Callback Funktionen und der Promises. Hier wird
der Vorgang angestoßen und der Vorgang selbst signalisiert dann das Ende. Die Warteschleife entfällt und
der Thread kann weiter ausgeführt werden um Events zu verarbeiten, ...

Allerding macht die *Synchronisation*, also das Bestimmen des Zeitpunktes, wann der Vorgang beendet ist
und das Ergebnis verarbeitet werden kann, bei mehreren parallelen Vorgängen den Code schwer lesbar. 
Deswegen wurden in C# 5 zwei neue Schlüsselwörter eingefügt: ***await*** und ***async***. Ziel war es, 
asynchrone Vorgänge so einfach wie synchrone Vorgänge anstoßen und auswerten zu können.

Folgendes Beispiel zeigt das asynchrone Lesen von einer Webquelle. Das Ergebnis wird auf der Konsole
ausgegeben:
```c#
private async Task RequestDemo(string url)
{
    HttpClient client = new HttpClient();
    string result = await client.GetStringAsync("https://www.google.com/search?q=c+sharp+better+than+java");
    Console.WriteLine(result);
}
``` 

Im .NET Framework gibt es ab Version 4 zu fast jeder I/O Operation eine Methode, die auf *Async* endet.
Hier ist es die Methode *HttpClient.GetStringAsync()*. Diese Methoden haben eine Besonderheit: Sie liefern
nicht das Ergebnis direkt (also einen String) zurück, sondern einen *Task* (in diesem Fall einen *Task&lt;string&gt;*).
Ein Task ist ein "Promise", also ein Versprechen der Methode, irgendwann das Ergebnis zu liefern. In dieser
Zeit wird allerdings der aufrufende Thread nicht blockiert. Die MSDN liefert eine gute Definition von *await*:

>> An await expression does not block the thread on which it is executing. Instead, it causes the compiler 
>> to sign up the rest of the async method as a continuation on the awaited task. Control then returns 
>> to the caller of the async method. When the task completes, it invokes its continuation, and execution 
>> of the async method resumes where it left off.
>> -- <cite>[MSDN](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/await)</cite>

*await* kann jedoch nur in speziellen Methoden aufgerufen werden. Da sich der Thread im Laufe der Methode
ändern kann, muss dies im Header mit dem Schlüsselwort *async* bekannt gegeben werden. Async Methoden
haben 3 mögliche Rückgabewerte:
- *Task&lt;Datatype&gt;* für Methoden, die ein Ergebnis liefern, auf das gewartet werden soll.
- *Task* für Methoden, die nichts zurückgeben. Durch die Rückgabe eines Tasks kann in der aufrufenden
  Methode mit *await* oder *Methodenname.Wait()* - falls die aufrufende Methode nicht async ist - gewartet werden.
- Der Rückgabewert *void* ist zwar möglich, sollte aber Eventhandlern vorbehalten sein. Sonst kann in der
  aufrufenden Methode nicht gewartet werden, bis die asynchrone Methode beendet wurde!

## Verwenden von *Task.Run()*

Im Codebeispiel wird eine Methode *HeavyWorkAsync()* als Beispiel für CPU intensive Arbeit definiert:
```c#
private Task<double> HeavyWorkAsync()
{
    return Task.Run(() =>
    {
        double result = 0;
        // Do some heavy work.
        return result;
    });
}
```

Mit *Task.Run()* können einfach Methoden in einem eigenen Thread ausgeführt werden. Diese Methode ist 
nicht mit dem Schlüsselwort *async* definiert, da wir den Task selbst kreieren. Dadurch können wir ihn
aber explizit zurückgeben. In einer *async* Methode macht dies der Compiler für uns.

## Die Ausgabe des Musterprogrammes im Detail

| Ausgabe | Erklärung |
| ------- | --------- |
|	Starte&nbsp;Programm&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;1                                                                 	|	Die *Main()* Methode läuft in Thread 1.	|
|	Starte&nbsp;MainAsync&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;1                                                                	|	Obwohl *MainAsync()* eine *async* Methode ist, wird noch kein neuer Thread erstellt. Das geschieht erst bei *await* Anweisungen.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;MainAsync01&nbsp;Thread&nbsp;ID&nbsp;7                                                          	|	Nach *HttpClient.GetStringAsync()* kehrt die Methode in einen anderen Thread (7) zurück.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;MainAsync02&nbsp;Thread&nbsp;ID&nbsp;4                                                          	|	Nach *StreamWriter.WriteAsync()* kehrt die Methode in einen anderen Thread (4) zurück.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Starte&nbsp;HeavyWork&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;4                	|	Nach dem Aufruf von *HeavyWork()* ändert sich der Thread nicht.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Starte&nbsp;HeavyWork&nbsp;Thread&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;11   	|	Durch *Task.Run()* wird die übergebene Action in einem anderen Thread ausgeführt.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;MainAsync03&nbsp;Thread&nbsp;ID&nbsp;11                                                         	|	Nach *HeavyWork()* kehrt *MainAsync()* nicht wieder in den alten Thread zurück, sondern bleibt in 11.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Periodic&nbsp;Thread&nbsp;ID&nbsp;5                                     	|	Durch *await Task.Delay(sleep);* wird auch der Thread gewechselt.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BEEP!                                           	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Periodic&nbsp;Thread&nbsp;ID&nbsp;4                                     	|	Durch *await Task.Delay(sleep);* wird auch der Thread gewechselt.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BEEP!                                           	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;MainAsync04&nbsp;Thread&nbsp;ID&nbsp;4                                                          	|	*MainAsync()* bleibt im selben Thread wie nach dem letzten Aufruf von *Task.Delay()* in *PeriodicAsync()*.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Starte&nbsp;HeavyWork&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;4                	|	*await Task.WhenAll()* startet 2 Methoden: *HeavyWork()* und *PeriodicAsync()*, die zuerst im Thread von *MainAsync()* starten.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Starte&nbsp;HeavyWork&nbsp;Thread&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;5    	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Periodic&nbsp;Thread&nbsp;ID&nbsp;11                                    	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BEEP!                                           	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Periodic&nbsp;Thread&nbsp;ID&nbsp;4                                     	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BEEP!                                           	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;MainAsync05&nbsp;Thread&nbsp;ID&nbsp;5                                                          	|	*MainAsync()* ist nun im Thread, der länger gedauert hat. Das ist der von *HeavyWork()*.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Starte&nbsp;HeavyWork&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;5                	|	*Task.WaitAll()* startet 2 Methoden: *HeavyWork()* und *PeriodicAsync()*, die zuerst im Thread von *MainAsync()* starten.	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Starte&nbsp;HeavyWork&nbsp;Thread&nbsp;in&nbsp;Thread&nbsp;ID&nbsp;4    	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Periodic&nbsp;Thread&nbsp;ID&nbsp;11                                    	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BEEP!                                           	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Periodic&nbsp;Thread&nbsp;ID&nbsp;10                                    	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BEEP!                                           	|	&nbsp;	|
|	&nbsp;&nbsp;&nbsp;&nbsp;MainAsync06&nbsp;Thread&nbsp;ID&nbsp;5                                                          	|	Durch *Task.WaitAll()* kehrt die Methode wieder in den Thread 5 zurück, da *WaitAll()* blockiert.	|
|	Invalid&nbsp;count&nbsp;value.                                                                                          	|	Exceptions werden in async Methoden, die einen Task liefern, korrekt abgefangen.	|
|	MAIN&nbsp;Thread&nbsp;ID&nbsp;1                                                                                         	|	Durch *MainAsync().Wait()* kehrt die Methode wieder in Thread 1 zurück, da *Wait()* blockiert.	|
