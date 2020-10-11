using LiteDB;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace LiteRepoDemo
{
    class BaseRepository
    {
        private readonly Func<LiteDatabase> Connect;
        private readonly Func<LiteDatabase> ConnectReadonly;

        /// <summary>
        /// Erstellt ein neues Repository für eine Dateidatenbank.
        /// </summary>
        /// <param name="filename"></param>
        public BaseRepository(string filename)
        {
            Connect = () => new LiteDatabase($"Filename={filename}.db;Mode=Shared");
            ConnectReadonly = () => new LiteDatabase($"Filename={filename}.db;Mode=ReadOnly");
        }

        /// <summary>
        /// Sucht einen Datensatz nach der ID.
        /// </summary>
        public Task<T> Find<T>(BsonValue id) => Task.Run(() =>
        {
            using var db = ConnectReadonly();
            return db.GetCollection<T>().FindById(id);
        });
        /// <summary>
        /// Sucht einen Datensatz mittels Expression.
        /// </summary>
        public Task<List<T>> Find<T>(Expression<Func<T, bool>> predicate) => Task.Run(() =>
        {
            using var db = ConnectReadonly();
            return db.GetCollection<T>().Find(predicate).ToList();
        });
        /// <summary>
        /// Gibt die gesamte Collection zurück.
        /// </summary>
        public Task<List<T>> Find<T>() => Task.Run(() =>
        {
            using var db = ConnectReadonly();
            return db.GetCollection<T>().FindAll().ToList();
        });


        public Task<bool> Upsert<T>(T document) => Task.Run(() =>
        {
            using var db = Connect();
            return db.GetCollection<T>().Upsert(document);
        });
        public Task<int> Upsert<T>(IEnumerable<T> documents) => Task.Run(() =>
        {
            using var db = Connect();
            return db.GetCollection<T>().Upsert(documents);
        });

        public Task<bool> Delete<T>(BsonValue id) => Task.Run(() =>
        {
            using var db = Connect();
            return db.GetCollection<T>().Delete(id);
        });
        public Task<int> Delete<T>(Expression<Func<T, bool>> predicate) => Task.Run(() =>
        {
            using var db = Connect();
            return db.GetCollection<T>().Delete(predicate);
        });
    }

    class UserRepository : BaseRepository
    {
        public UserRepository(string filename) : base(filename) { }
        // TODO: Eigene Abfragen der Businesslogik.
    }

    class Student
    {
        [BsonId]
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            var repo = new UserRepository("user1");
            await repo.Upsert<Student>(new Student[]
            {
                new Student { Id = 1, Firstname = "FN1", Lastname = "LN1" },
                new Student { Id = 2, Firstname = "FN2", Lastname = "LN2" },
                new Student { Id = 3, Firstname = "FN3", Lastname = "LN3" },
                new Student { Id = 4, Firstname = "FN4", Lastname = "LN4" },
                new Student { Id = 5, Firstname = "FN5", Lastname = "LN5" },
            });

            await repo.Upsert(new Student { Id = 2, Firstname = "FN2_New", Lastname = "LN2_New" });
            await repo.Delete<Student>(s => s.Id == 1);
            await repo.Delete<Student>(4);
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(await repo.Find<Student>(2)));
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(await repo.Find<Student>(s => s.Id > 3)));
        }
    }
}
