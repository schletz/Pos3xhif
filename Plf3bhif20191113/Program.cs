using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Plf3bhif
{
    class Program
    {

        static void Main(string[] args)
        {
            int score = 0;

            Console.Write("TEST1: Ist die Post Klasse abstrakt? ");
            score += AssertTrue(() => typeof(Post).IsAbstract);

            Console.Write("TEST2: Kann ein TextPost angelegt werden? ");
            DateTime time = DateTime.Now.AddMinutes(-10);
            TextPost tp1 = new TextPost("Mein Titel");
            TextPost tp2 = new TextPost("Mein 2. Titel", time);
            score += AssertTrue(() => tp1.Title == "Mein Titel" && tp2.Created == time);

            Console.Write("TEST3: Kann ein ImagePost angelegt werden? ");
            ImagePost ip1 = new ImagePost("Mein Titel");
            ImagePost ip2 = new ImagePost("Mein Titel", time);
            score += AssertTrue(() => ip1.Title == "Mein Titel" && ip2.Created == time);

            Console.Write("TEST4: Stimmt die Länge des Postings? ");
            TextPost tp3 = new TextPost("Mein Titel", time);
            TextPost tp4 = new TextPost("Mein Titel", time) { Content = "Content" };
            score += AssertTrue(() => tp3.Length == 0 && tp4.Length == 7);

            Console.Write("TEST5: Stimmt das HTML Property von TextPost? ");
            Post tp5 = new TextPost("Mein Titel", time) { Content = "Content" };
            score += AssertTrue(() => tp5.Html == "<p>Content</p>");

            Console.Write("TEST6: Stimmt das HTML Property von ImagePost? ");
            Post ip3 = new ImagePost("Mein Titel", time) { Url = "http://image.png" };
            score += AssertTrue(() => ip3.Html == "<img src=\"http://image.png\">");

            Console.Write("TEST7: Stimmt die ToString Repräsentation von Post? ");
            object ip4 = new ImagePost("Mein Titel", time) { Url = "http://image.png" };
            score += AssertTrue(() => ip4.ToString() == "<img src=\"http://image.png\">");

            Console.Write("TEST8: Ist das Length Property von TextPost readonly? ");
            PropertyInfo info = typeof(TextPost).GetMember("Length")[0] as PropertyInfo;
            score += AssertTrue(() => info.CanWrite == false);

            Console.Write("TEST9: Ist das Title Property von Post readonly? ");
            PropertyInfo info2 = typeof(Post).GetMember("Title")[0] as PropertyInfo;
            score += AssertTrue(() => info2.CanWrite == false);

            Console.Write("TEST10: Funktioniert die PostCollection? ");
            TextPost tp6 = new TextPost("Mein Titel", time) { Content = "Content" };
            ImagePost ip5 = new ImagePost("Mein Titel", time) { Url = "http://image.png" };
            PostCollection pc1 = new PostCollection();
            pc1.Add(tp6);
            pc1.Add(ip5);
            score += AssertTrue(() => pc1.Count == 2);

            Console.Write("TEST11: Funktioniert CalcRating mit Lambdas? ");
            TextPost tp7 = new TextPost("Mein Titel", time) { Content = "Content", Rating = 10 };
            ImagePost ip6 = new ImagePost("Mein Titel", time) { Url = "http://image.png", Rating = -1 };
            PostCollection pc2 = new PostCollection();
            pc2.Add(tp7);
            pc2.Add(ip6);
            score += AssertTrue(() => pc2.CalcRating(p => p.Rating > 0 ? p.Rating : 0) == 10);

            Console.Write("TEST12: Funktioniert ProcessPosts mit Lambdas? ");
            TextPost tp8 = new TextPost("Mein Titel", time) { Content = "Content", Rating = 10 };
            ImagePost ip7 = new ImagePost("Mein Titel", time) { Url = "http://image.png", Rating = -1 };
            PostCollection pc3 = new PostCollection();
            pc3.Add(tp8);
            pc3.Add(ip7);
            int ratingSum = 0;
            pc3.ProcessPosts(p => ratingSum += p.Rating);
            score += AssertTrue(() => ratingSum == 9);

            double percent = score / 12.0;
            int note = percent > 0.875 ? 1 : percent > 0.75 ? 2 : percent > 0.625 ? 3 : percent > 0.5 ? 4 : 5;
            Console.WriteLine($"{score} von 12 Punkten erreicht. Note: {note}.");
            Console.ReadLine();
        }

        static int AssertTrue(Func<bool> action)
        {
            if (action())
            {
                Console.WriteLine("1");
                return 1;
            }
            else
            {
                Console.WriteLine("0");
                return 0;
            }
        }
    }
}
