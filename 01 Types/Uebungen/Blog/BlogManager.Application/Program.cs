using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace BlogManager.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                foreach (var type in new Type[] { typeof(User), typeof(Comment), typeof(Post), typeof(ImagePost), typeof(TextPost) })
                {
                    CheckAndWrite(() => !type.HasDefaultConstructor(), $"{type.Name} does not have a default constructor.");
                    CheckAndWrite(() => type.IsImmutable(), $"{type.Name} is immutable.");
                }
                CheckAndWrite(() => typeof(Post).IsAbstract, "Post is abstract.");
                CheckAndWrite(() => typeof(Post).HasProtectedConstructor(typeof(User), typeof(string)), "Post has a protected constructor.");
                CheckAndWrite(() => typeof(Post).PropertyHasType<IReadOnlyList<Comment>>(nameof(Post.Comments)),
                    "Post.Comments is IReadOnlyList<Comment>.");
                CheckAndWrite(() => typeof(Post).PropertyHasType<decimal?>(nameof(Post.AverageRating)),
                    "Post.AverageRating is decimal?.");
                CheckAndWrite(() => typeof(Post).IsAbstractProperty(nameof(Post.Html)), "Post.Html is abstract.");
            }
            {
                Console.WriteLine("Checking constructors");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                CheckAndWrite(() => user.Email == "email1" && user.Firstname == "firstname1" && user.Lastname == "lastname1",
                    "Constructor of User sets the values.");
                var imagePost = new ImagePost(user, "imageTitle", "imageUrl");
                CheckAndWrite(() => imagePost.User == user && imagePost.Title == "imageTitle" && imagePost.Url == "imageUrl",
                    "Constructor of ImagePost sets the values.");
                var textPost = new TextPost(user, "textTitle", "textContent");
                CheckAndWrite(() => textPost.User == user && textPost.Title == "textTitle" && textPost.Content == "textContent",
                    "Constructor of TextPost sets the values.");
                CheckAndWrite(() => imagePost.Comments.Count == 0 && imagePost.RatingCount == 0,
                    "A new post has no comments and no ratings.");
            }
            {
                Console.WriteLine("Checking HTML output");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                Post imagePost = new ImagePost(user, "title", "url");
                Post textPost = new TextPost(user, "title", "content");
                CheckAndWrite(() => imagePost.Html == "<img src=\"url\" />", "ImagePost.Html returns the correct HTML string.");
                CheckAndWrite(() => textPost.Html == "<h1>title</h1><p>content</p>", "TextPost.Html returns the correct HTML string.");
            }
            {
                Console.WriteLine("Checking comments");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                var commentator = new User(email: "email2", firstname: "firstname2", lastname: "lastname2");
                var commentator2 = new User(email: "email3", firstname: "firstname3", lastname: "lastname3");
                Post post = new ImagePost(user, "title", "url");
                Post otherPost = new TextPost(user, "title", "content");
                DateTime beforeComment = DateTime.UtcNow;
                post.AddComment(commentator2, "comment B");
                DateTime afterComment = DateTime.UtcNow;
                post.AddComment(commentator, "comment A");
                CheckAndWrite(() =>
                    post.Comments.Count == 2
                    && post.Comments[0].User == commentator2 && post.Comments[0].Text == "comment B"
                    && post.Comments[1].User == commentator && post.Comments[1].Text == "comment A", "Post.AddComment adds the comments in the correct order.", 2);
                CheckAndWrite(() => otherPost.Comments.Count == 0, "Each post has its own comments.");
                CheckAndWrite(() =>
                    post.Comments[0].Created.Kind == DateTimeKind.Utc
                    && post.Comments[0].Created >= beforeComment && post.Comments[0].Created <= afterComment,
                    "Constructor of Comment sets Created to DateTime.UtcNow.", 2);
            }
            {
                Console.WriteLine("Checking ratings");
                var user = new User(email: "email2", firstname: "firstname2", lastname: "lastname2");
                var userSameEmail = new User(email: "email2", firstname: "firstname3", lastname: "lastname3");
                var otherUser = new User(email: "email4", firstname: "firstname4", lastname: "lastname4");
                Post post = new ImagePost(user, "title", "url");
                CheckAndWrite(() => !post.TryRate(user, 0) && !post.TryRate(user, 6) && !post.TryRate(user, -1) && post.RatingCount == 0,
                    "Post.TryRate rejects invalid values.", 2);
                CheckAndWrite(() => post.TryRate(user, 1) && post.RatingCount == 1, "Post.TryRate adds a rating.", 2);
                CheckAndWrite(() => post.TryRate(otherUser, 5) && post.RatingCount == 2, "Post.TryRate accepts the rating 5.");
                CheckAndWrite(() => !post.TryRate(userSameEmail, 2) && post.RatingCount == 2 && post.AverageRating == 3M,
                    "Post.TryRate returns false if the email has already rated.", 2);
            }
            {
                Console.WriteLine("Checking AverageRating");
                var user = new User(email: "email1", firstname: "firstname1", lastname: "lastname1");
                var user2 = new User(email: "email2", firstname: "firstname2", lastname: "lastname2");
                var user3 = new User(email: "email3", firstname: "firstname3", lastname: "lastname3");
                var user4 = new User(email: "email4", firstname: "firstname4", lastname: "lastname4");
                Post post = new ImagePost(user, "title", "url");
                CheckAndWrite(() => post.AverageRating is null, "Post.AverageRating is null if there is no rating.", 2);
                post.TryRate(user, 2);
                post.TryRate(user2, 3);
                CheckAndWrite(() => post.AverageRating == 2.5M, "Post.AverageRating returns the average of the ratings (2 and 3).", 2);
                post.TryRate(user3, 1);
                post.TryRate(user4, 3);
                CheckAndWrite(() => post.AverageRating == 2.25M && post.RatingCount == 4, "Post.AverageRating returns the average of the ratings (2, 3, 1 and 3).", 2);

                Post post2 = new TextPost(user, "title", "content");
                post2.TryRate(user, 1);
                post2.TryRate(user2, 1);
                post2.TryRate(user3, 2);
                CheckAndWrite(() => post2.AverageRating > 1.3333M && post2.AverageRating < 1.3334M && post2.RatingCount == 3,
                    "Post.AverageRating is not rounded (1, 1 and 2 give 1.3333...).");
            }

            WriteSummary();
        }
    }
}
