# Mustercode: Benutzeroberflächen als State Machine auffassen
Mit Hilfe einer State Machine können die verschiedenen Zustände einer Benutzeroberfläche (ein- und ausgeblendete Inhalte, aktivierte und deaktivierte Steuerelemente, …) elegant programmiert werden. Das nuget Paket Stateless [1] stellt hierfür die Klasse StateMachine bereit, die verschiedene States und Trigger, die den Übergang zwischen den States hervorrufen, verwalten kann. Dieses Beispiel implementiert ein Kassenterminal für Bankomatkarten. Es gibt bei dieser kleinen Applikation bereits recht viele mögliche Zustände:

Grafik gezeichnet mit draw.io

In der Datei CashApp.zip befindet sich die vollständig ausprogrammierte Lösung als Basis für weitere Projekte.

[1] www.nuget.org/packages/stateless
