# Verbindung mit dem VPN der Spengergasse
1. Von [gate.spengergasse.at](https://gate.spengergasse.at/+CSCOE+/logon.html#form_title_text) nach dem Login *Cisco AnyConnect VPN* herunterladen.
1. Das Programm mit den Standardeinstellungen installieren.
1. Cisco AnyConnect aufrufen. Nun kann man sich mit dem Server *gate.spengergasse.at* verbinden. Beim
   Benutzernamen muss keine Domäne eingefügt werden, es wird einfach der Accountname (z. B. ABC1234)
   angegeben.
1. Folgende Netzwerke werden über das VPN geroutet. Das Internet ist dadurch lokal weiter verfügbar und
   wird auch nicht über die Schule geroutet:

   ``` 
   IPv4-Routentabelle
   ===========================================================================
   Aktive Routen:
        Netzwerkziel    Netzwerkmaske          Gateway    Schnittstelle Metrik
            10.0.0.0        255.0.0.0       172.23.0.1     172.23.1.166      2
          10.50.5.11  255.255.255.255       172.23.0.1     172.23.1.166      2
          10.50.5.12  255.255.255.255       172.23.0.1     172.23.1.166      2
        192.168.56.0    255.255.255.0       172.23.0.1     172.23.1.166      2
       192.168.192.0    255.255.240.0       172.23.0.1     172.23.1.166      2
       192.168.250.0    255.255.255.0       172.23.0.1     172.23.1.166      2
        192.189.51.0    255.255.255.0       172.23.0.1     172.23.1.166      2
       193.170.108.0    255.255.255.0       172.23.0.1     172.23.1.166      2
   ===========================================================================
   ```
   
## Zugriff auf den Storage (Netzlaufwerke in den Laboren)   
1. Im Explorer (und nicht im Browser!) kann sich mit dem Pfad *\\\enterprise.htl-wien5.schule* mit den
   Laufwerken der Schule verbunden werden. Beim Benutzernamen muss die Domäne vorangestellt werden, also
   *htl-wien5\abc1234* wenn der Accountname abc1234 ist.
    - **Unterricht (U Laufwerk):** *\\\enterprise.htl-wien5.schule\ausbildung\unterricht*
    - **Software:** *\\\enterprise.htl-wien5.schule\ausbildung\services\software*
   
## Einrichten von Outlook
1. Beim Startbildschirm nach der ersten Installation muss nur die E-Mail Adresse (*abc1234@spengergasse.at*)
   eingegeben werden. Nach dem Klicken auf *Verbinden* wählt man den Kontotyp *Exchange*.
   