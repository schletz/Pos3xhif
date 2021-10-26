using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogManager.Application
{
    public class Program
    {
        private static int _testCount = 0;
        private static int _testsSucceeded = 0;
        private static int _points = 0;
        private static int _pointsMax = 0;

        public static void Main(string[] args)
        {
            {
                Console.WriteLine("Teste Klassenimplementierung.");
                foreach (var type in new Type[] { typeof(User), typeof(Comment), typeof(Post), typeof(ImagePost), typeof(TextPost) })
                {
                    CheckAndWrite(() => !HasDefaultConstructor(type), $"Kein Defaultkonstruktor in {type.Name}.");
                    CheckAndWrite(() => IsImmutable(type), $"{type.Name} ist immutable.");
                }
                CheckAndWrite(() => PropertyHasType<IReadOnlyList<Comment>>(typeof(Post), nameof(Post.Comments)),
                    "Post.Comments ist vom Typ IReadOnlyList<Comment>.");
                CheckAndWrite(() => typeof(Post).GetProperty(nameof(Post.Html))?.GetMethod?.IsAbstract == true,
                    "Post.Html ist abstrakt.");
            }
            {
                Console.WriteLine("Teste HTML Ausgabe.");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                Post imagePost = new ImagePost(user, "title", "url");
                Post textPost = new TextPost(user, "title", "content");
                CheckAndWrite(() => imagePost.Html == "<img src=\"url\" />", "ImagePost.Html liefert den richtign HTML String.");
                CheckAndWrite(() => textPost.Html == "<h1>title</h1><p>content</p>", "TextPost.Html liefert den richtign HTML String.");
            }
            {
                Console.WriteLine("Teste Kommentieren");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                var commentator = new User(email: "email2", firstname: "firstname2", lastname: "lastname2");
                Post post = new ImagePost(user, "title", "url");
                post.AddComment(commentator, "comment");
                CheckAndWrite(() => post.Comments.Count == 1, "Post.AddComment fügt einen Kommentar hinzu.", 2);
                CheckAndWrite(() => post.Comments[0].Created > DateTime.UtcNow.AddMinutes(-1), "Post.AddComment setzt Created auf UtcNow.", 2);
            }
            {
                Console.WriteLine("Teste Rating");
                var user = new User(email: "email2", firstname: "firstname2", lastname: "lastname2");
                var user2 = new User(email: "email2", firstname: "firstname3", lastname: "lastname3");
                Post post = new ImagePost(user, "title", "url");
                CheckAndWrite(() => !post.TryRate(user, 6), "Post.TryRate lehnt ungültige Werte ab.", 2);
                CheckAndWrite(() => post.TryRate(user, 1) && post.RatingCount == 1, "Post.TryRate ein Rating hinzu.", 2);
                CheckAndWrite(() => !post.TryRate(user2, 2) && post.RatingCount == 1, "Post.TryRate liefert false, wenn die Email schon geratet hat.", 2);
            }
            {
                Console.WriteLine("Teste AverageRating");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                var user2 = new User(email: "email2", firstname: "firstname2", lastname: "lastname2");
                Post post = new ImagePost(user, "title", "url");
                CheckAndWrite(() => post.AverageRating is null, "Post.AverageRating ist null, wenn kein Rating abgegeben wurde.", 2);
                post.TryRate(user, 2);
                post.TryRate(user2, 3);
                CheckAndWrite(() => post.AverageRating == 2.5M, "Post.AverageRating berechnet den Durchschnitt der Ratings.", 2);
            }
            Console.WriteLine($"{_testsSucceeded} von {_testCount} Tests erfüllt.");
            Console.WriteLine($"{_points} von {_pointsMax} Punkte erreicht.");
        }

        private static void CheckAndWrite(Func<bool> predicate, string message, int weight = 1)
        {
            _testCount++;
            _pointsMax += weight;
            if (predicate())
            {
                Console.WriteLine($"   {_testCount} OK: {message}");
                _testsSucceeded++;
                _points += weight;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   {_testCount} Nicht erfüllt: {message}");
            Console.ResetColor();
        }

        private static bool HasDefaultConstructor(Type type) =>
            type.GetConstructor(Type.EmptyTypes) is not null;

        private static bool PropertyHasType<Ttarget>(Type type, string propertyName) =>
            type.GetProperty(propertyName)?.PropertyType == typeof(Ttarget);

        private static bool IsImmutable(Type type)
        {
            var properties = type.GetProperties();
            if (!properties.Any()) { return false; }
            return type.GetProperties().All(p => p.CanWrite == false);
        }
    }
}