# Google Firebase Demo App

![zerone-consulting.com - All in the NoSQL Family](https://www.zerone-consulting.com/images/nosqlfamily.jpg)
![www.zerone-consulting.com - NoSQL Storage Architecture](https://www.zerone-consulting.com/images/nosqldatabase.jpg)

## 8 Reasons why NoSQL came in 
1. **Schema-Less:** The Biggest reason for NoSQL's sole existence is because of the flexibility provided by NoSQL to model the data as per the application requirements. The RDBMS way of doing is to force our data to fit in a structured way in the form of tables and columns.Most of the NoSQL databases are offering richer form of data storage semantically in which we can store data objects as documents or as key-value pairs or even as graph structures as per the requirements.
1. **Power:** In a NoSQL database, we can add massive volumes of data at a significantly low cost.
1. **Unstructured Data:**In NoSQL databases , we needn't to know how we want to structure the data ahead of its storage. In NoSQL we can store the data first in the Database and structure it later.
1. **Scaling:** In NoSQL databases, the scaling is horizontal as compared to vertical scaling in SQL database whose process is comparatively more time consuming and expensive .
1. **Big Data:** Although SQL is also capable of handling big data but not with as much ease as it is possible with NoSQL databases. It is easier to handle large volumes of structured , semi-structures or unstructured data in NoSQL as compared to SQL databases.
1. **Schema Migration:** As we know that NoSQL is schemaless which makes it easier to deals with stuff like migration.
1. **Write Performance:** Sometimes data is too much to handle on a single storage node and a problem arises i.e data is too big to fit on one node. According to a report Twitter generates almost 8TB of data each day. At almost 100 MB/s it takes more than a day to store 8 TB. Because of this writes need to be distributed over clusters , Key - value access , replication, fault tolerance, Map reduce, consistency issues , etc should be implied. We can also use in-memory systems for faster writes.
1. **Ease:** This changes as per products and vendors but most of the NoSQL vendors are putting lot of efforts on automated operations,ease of access and use , Not much administration is required which leads to lesser costs of operations.

## Firebase vs. Firestore
Google bietet derzeit (März 2019) zwei Möglichkeiten an, semistrukturierte Daten in seiner Cloud zu speichern:
**Firebase** und **Firestore**. 
> - Realtime Database is Firebase's original database. It's an efficient, low-latency solution for mobile 
>   apps that require synced states across clients in realtime.
> - Cloud Firestore is Firebase's new flagship database for mobile app development. It improves on the 
>   successes of the Realtime Database with a new, more intuitive data model. Cloud Firestore also features richer, faster queries and scales better than the Realtime Database.
> -- <cite>[firebase.google.com](https://firebase.google.com/docs/database/rtdb-vs-firestore)</cite>

In dieser Demo beschäftigen wir uns mit **Firebase**, da hier Events in Echtzeit ausgetauscht werden können.
So bekommt unsere Applikation ein Event, wenn jemand z. B. in Amerika über eine App einen Datensatz hinzufügt.

## Erstellen einer Firebase Datenbank
![Firebase Console](FirebaseConsole.png)
<sup>*Screenshot von console.firebase.google.com*</sup>

1. Melde dich auf [der Firebase Console](https://console.firebase.google.com) mit deinem Google Account 
   an.
2. Lege mit *Projekt hinzufügen* ein neues Projekt an. In unserem Beispiel ist dies *Schuelerverwaltung*.
   Die Freigabe für Google Analytics kann abgehakt werden.
3. Bei der Datenfreigabe kann alles abgehakt bleiben. Gehe nun auf *Projekt erstellen*.
4. Das Projekt ist nun auch auf der [Google Cloud Platform](https://console.cloud.google.com) angelegt.
5. Wähle nun im linken Menü der Firebase Console den Punkt *Database* und lege mit *Datenbank erstellen*
   eine neue Cloud Firestore Datenbank an.
6. Bei den Sicherheitsregeln wähle den *gesperrten Modus*, somit kann ohne Authentifizierung nicht gelesen
   oder geschrieben werden.
7. Klicke auf das Zahnrad neben *Projekt Overview* (links oben) in der Firebase Console und wähle *Projekteinstellungen*.
8. Unter *Dienstkonten* kann unter dem Punkt *Datenbank-Secrets* das generierte Secret angezeigt werden,
   indem du mit der Maus über die Punkte fährst und *Anzeigen* wählst. Dieser String wird dann in der API
   benötigt werden.
9. Bei *Firebase Admin SDK* (ebenfalls unter Dienstkonten) ist ein Codeschnipsel für Node.js angegeben. Der String bei *databaseURL*
   gibt die Zugriffsadresse für die Datenbank an.

Informationen über die Einschränkungen der Gratisversion sind unter [Pricing](https://firebase.google.com/pricing/)
abrufbar.

## Zugriff auf die Datenbank
Für Firebase gibt es von Google Bibliotheken für iOS, Android, Javascript, Unity und C++. Die API für diese Plattformen 
ist auf [firebase.google.com](https://firebase.google.com/docs/database) dokumentiert. Es gibt aber
auch eine REST Schnittstelle, die von Drittanbietern verwendet wird. Eine Bibliothek für C# ist *FirebaseDatabase.net*. 
Der Quelltext und die Dokumentation sind auf der [GitHub Seite des Projektes](https://github.com/step-up-labs/firebase-database-dotnet) zu finden.

In der Firebase Console kann unter dem Punkt *Database* bei *Echtzeitdatenbank* der Inhalt der Datenbank
eingesehen werden. Schreibzugriffe können live verfolgt werden.

Ein Mustercode für den Zugriff ist unten abgebildet, es müssen die Variablen *secret* und *database* durch
die eigenen Daten ersetzt werden.

## Modellierung für Firebase
Firebase speichert JSON Daten als **Key - Value Speicher**. Das bedeutet, dass jedes Unterobjekt einen
Key besitzt, mit dem ein Ansprechen möglich ist. Im nachfolgenden Beispiel wurde der Klassenname sowie
der Accountname als eindeutiger Key verwendet. Sehen wir uns ein typisches JSON Dokument an, **wie es 
allerdings nicht sein sollte**:
```js
{
	"Klassen": {
		"3AHIF": {
			"KV": "SZ",
			"Raum": "C3.09",
			"Schueler": {
				"ABC122": {
					"Name": "Nach1",
					"Vorname": "Vor1",
					"Email": "abc122@spengergasse.at"
				},
				"ABC123": {
					"Name": "Nach2",
					"Vorname": "Vor2",
					"Email": "abc123@spengergasse.at"
				}
			}
		},
		"3BHIF": {
			"KV": "SIL",
			"Raum": "C3.10",
			"Schueler": {
				"ABC222": {
					"Name": "Nach1",
					"Vorname": "Vor1",
					"Email": "abc222@spengergasse.at"
				},
				"ABC223": {
					"Name": "Nach2",
					"Vorname": "Vor2",
					"Email": "abc223@spengergasse.at"
				}
			}
		},
		"3CHIF": {
			"KV": "TT",
			"Raum": "C3.11",
			"Schueler": {
				"ABC322": {
					"Name": "Nach1",
					"Vorname": "Vor1",
					"Email": "abc322@spengergasse.at"
				},
				"ABC323": {
					"Name": "Nach2",
					"Vorname": "Vor2",
					"Email": "abc323@spengergasse.at"
				}
			}
		}
	}
}
```

Der Grund, dass wir die Daten nicht so speichern sollten, ist folgender: Angenommen wir möchten eine Übersicht
der Klassen als Liste anzeigen. Dafür brauchen wir nur die Klassenbezeichnung und den KV. Firebase kann
allerdings nur ganze Knoten laden. **Das bedeutet, dass wir für diese Übersichtsliste auch alle Schülerdaten
laden müssen!**

Besser ist folgender Ansatz:
```js
{
	"Klassen": {
		"3AHIF": {
			"KV": "SZ",
			"Raum": "C3.09"
		},
		"3BHIF": {
			"KV": "SIL",
			"Raum": "C3.10"
		},
		"3CHIF": {
			"KV": "TT",
			"Raum": "C3.11"
		}
	},

	"Schueler": {
		"3AHIF": {
			"ABC122": {
				"Name": "Nach1",
				"Vorname": "Vor1",
				"Email": "abc122@spengergasse.at"
			},
			"ABC123": {
				"Name": "Nach2",
				"Vorname": "Vor2",
				"Email": "abc123@spengergasse.at"
			}
		},
		"3BHIF": {
			"ABC222": {
				"Name": "Nach1",
				"Vorname": "Vor1",
				"Email": "abc222@spengergasse.at"
			},
			"ABC223": {
				"Name": "Nach2",
				"Vorname": "Vor2",
				"Email": "abc223@spengergasse.at"
			}
		},
		"3CHIF": {
			"ABC322": {
				"Name": "Nach1",
				"Vorname": "Vor1",
				"Email": "abc322@spengergasse.at"
			},
			"ABC323": {
				"Name": "Nach2",
				"Vorname": "Vor2",
				"Email": "abc323@spengergasse.at"
			}
		}
	}
}
``` 
Bei diesem Ansatz speichern wir die Grundinformationen der Klassen in einem eigenen Dokument *Klassen*.
Die Schüler organisieren wir ebenfalls in Klassen, da unsere Applikation die Schüler klassenweise anzeigen
wird. Wir sehen bereits, **dass wir für eine sinnvolle Modellierung bereits wissen müssen, wie die Applikation
die Daten abfragen wird.**

Weitere Infos gibt es auf [howtofirebase.com](https://howtofirebase.com/firebase-data-modeling-939585ade7f4) 
und auf [firebase.google.com/](https://firebase.google.com/docs/database/web/structure-data) nachzulesen.


## Mustercode
```c#
using Firebase.Database;           // Für FirebaseClient, erfordert das Paket FirebaseDatabase.net
using Firebase.Database.Query;     // Extensions für die Abfragen, sonst werden die Methoden nicht gefunden.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseDemo.Generator
{
    /// <summary>
    /// Modelklasse für Schueler
    /// </summary>
    public class Schueler
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    internal class Program
    {
        /// <summary>
        /// Siehe Datenbank-Secrets auf console.firebase.google.com
        /// </summary>
        private static readonly string secret = "(Ist durch das Secret zu ersetzen)";

        /// <summary>
        /// Siehe Firebase Admin SDK auf console.firebase.google.com
        /// </summary>
        private static readonly string database = "https://(projektname).firebaseio.com";

        /// <summary>
        /// Connection zur Db
        /// </summary>
        private static FirebaseClient client = new FirebaseClient(database, new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(secret) });

        private static void Main(string[] args)
        {
            // Auf console.firebase.google.com -> Database -> Echtzeitdatenbank kann live angesehen werden,
            // die die Daten aktualisiert werden.
            Console.WriteLine("Schreibe oder aktualisiere das Dokument Schueler...");
            WriteData().Wait();
            Console.WriteLine("Lese die Daten des Dokumentes Schueler...");
            ReadData().Wait();

            Console.WriteLine("Press ENTER.");
            Console.ReadLine();
        }

        /// <summary>
        /// Schreibt Musterdaten in das Dokument "Schueler". Beim erneuten Aufruf werden die Werte
        /// aktualisiert, da durch die fixe Initialisierung des Zufallszahlengenerators immer die
        /// gleichen Werte geliefert werden.
        /// </summary>
        /// <returns></returns>
        private static async Task WriteData()
        {
            try
            {
                // Generiert immer die gleichen Zufallszahlen. So wird beim 2. Durchlauf ein UPDATE
                // durchgeführt.
                Random rnd = new Random(100);
                for (int i = 0; i < 10; i++)
                {
                    Schueler s = new Schueler
                    {
                        Id = rnd.Next(),
                        Firstname = $"Firstname{rnd.Next(100, 1000)}",
                        Lastname = $"Lastname{rnd.Next(100, 1000)}",
                        DateOfBirth = DateTime.Now.AddDays(-14 * 365 - rnd.Next(0, 4 * 365))
                    };
                    await client.Child("Schueler").Child(s.Id.ToString()).PutAsync(s);
                }
            }
            catch (FirebaseException e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Liest die Daten aus dem Dokument Schueler aus und zeigt sie auf der Konsole an.
        /// </summary>
        /// <returns></returns>
        private static async Task ReadData()
        {
            try
            {
                IEnumerable<FirebaseObject<Schueler>> schuelerDocument = await client.Child("Schueler").OnceAsync<Schueler>();
                foreach (FirebaseObject<Schueler> schuelerObject in schuelerDocument)
                {
                    Console.WriteLine($"Schueler {schuelerObject.Key}: {schuelerObject.Object.Firstname} {schuelerObject.Object.Lastname}, Birth: {schuelerObject.Object.DateOfBirth}");
                }
            }
            catch (FirebaseException e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}

```
