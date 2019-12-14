using Microsoft.EntityFrameworkCore;
using Crud.Model;
using System;
using System.Linq;
using System.Text.Json;

namespace Crud
{
    class Program
    {
        static void Main(string[] args)
        {
            // Am Ende des Blockes wird Dispose() aufgerufen und die Verbindung wird geschlossen.
            using (TestsContext context = new TestsContext())
            {

            }

        }
    }
}
