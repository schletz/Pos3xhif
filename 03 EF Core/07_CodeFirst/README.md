# Code First in EF Core

Es ist auch möglich, zuerst die Modelklassen in C# zu schreiben und danach die Datenbank erstellen
zu lassen. Natürlich müssen dabei zusätzliche Informationen wie Längen (C# kennt nur *string* und
kein *VARCHAR(n)*), Fremdschlüssel und Indizes bekannt gegeben werden.

Dabei stehen grundsätzlich 2 Möglichkeiten zur Verfügung

1. Verwendung von Data Annotations in den Modelklassen
2. Verwendung der Fluent API in DbContext

Variante (1) hat den Vorteil, dass die Annotation direkt über der Spalte steht und bei Codeänderungen
leichter angepasst werden kann. Nachteile sind allerdings

- "Unsaubere" Modelklassen mit Abhängigkeiten zu EF Core (die Annotations). Sie lassen sich daher
  schwerer in eine shared library schreiben.
- Es sind nicht alle Möglichkeiten der Datenbank umsetzbar (kombinierte Fremdschlüssel, ...).

Variante (2) liefert alle Features, die EF Core beherrscht. Allerdings hat diese Variante auch
Nachteile:

- Definition im DB Context, daher schwerer wartbar.
- Schwerere Lesbarkeit und mehr Schreibarbeit.

Es gibt daher - wie meist in der Programmierung - keine generelle Empfehlung für eine dieser beiden
Techniken.

## Convention over Configuration

Man muss nicht jede Eigenschaft der Spalte händisch bekannt geben. Wird z. B. eine Spalte
*BlogId* in der Klasse Blog
definiert, so erkennt dies EF Core automatisch als Primärschlüssel. Eine Übersicht über die
Konventionen in EF Core ist auf [docs.microsoft.com](https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#conventions)
dargestellt.

Folgendes Modell verwendet nur die Conventions von EF Core zur Erzeugung der Datenbank:

```c#
class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }    // Verweis auf die Klassen. Legt die Tabelle an.
    public DbSet<Post> Posts { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("DataSource=Blog.db");
}

class Blog
{
    public int BlogId { get; set; }           // Wird durch (Klasse)Id als PK erkannt.
    public string Name { get; set; }
    public List<Post> Posts { get; set; }     // Wird durch List<Post> als Navigation erkannt.
}
class Post
{
    public int PostId { get; set; }
    public string Content { get; set; }
    public Blog Blog { get; set; }            // Wird durch den Typ Blog als FK und Navigation erkannt.
}
```

Die oberen Modelklassen erzeugen folgenden SQL Code:

```sql
CREATE TABLE Blogs (
    BlogId INTEGER NOT NULL CONSTRAINT PK_Blogs PRIMARY KEY AUTOINCREMENT,
    Name TEXT NULL
);


CREATE TABLE Posts (
    PostId INTEGER NOT NULL CONSTRAINT PK_Posts PRIMARY KEY AUTOINCREMENT,
    Content TEXT NULL,
    BlogId INTEGER NULL,
    CONSTRAINT FK_Posts_Blogs_BlogId FOREIGN KEY (BlogId) REFERENCES Blogs (BlogId) ON DELETE RESTRICT
);

CREATE INDEX IX_Posts_BlogId ON Posts (BlogId);
```

## Syntax von Code First

Auf https://docs.microsoft.com/en-us/ef/core/modeling/ ist eine gute Dokumentation, der nichts
hinzuzufügen ist. Die relevantesten Punkte sind

- [Entity Types](https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations)
- [Entity Properties](https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwithout-nrt)
- [Keys](https://docs.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations)
- [Generated Values](https://docs.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations)
- [Relationships](https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key)
