**KlassenController.cs**
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StundenplanApi.Models;

namespace StundenplanApi.Controllers
{
    public class KlassenController : ApiController
    {
        /// <summary>
        /// Reagiert auf localhost:.../api/klassen
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {

            using (StundenplanDb db = new StundenplanDb())
            {
                // ToList ist wichtig, da die Verbindung
                // bei return ja schon zu ist
                // --> Laufzeitfehler
                var klassen = (from k in db.Klasses
                              orderby k.ID
                              select k).ToList();
                // Liefere HTTP 200 mit den Daten aus klassen.
                return Ok(klassen);
            }
        }

        public IHttpActionResult Get(string id)
        {
            using (StundenplanDb db = new StundenplanDb())
            {
                var klasse = (from k in db.Klasses
                              where k.ID == id
                              select k).ToList();
                return Ok(klasse);
            }
        }
    }
}
```
**Application_Start in Global.asax.cs**
```
        protected void Application_Start()
        {
            /* Wir wollen ein JSON aus Ausgabe, kein XML. Wichtig ist der Data Contract, damit
             * der Formatter nicht durch die Navigation Properties "im Kreis" geht. */
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            /* Damit können wir application/x-www-url-formencoded POST Requests lesen. */
            GlobalConfiguration.Configuration.Formatters.
                Add(new FormUrlEncodedMediaTypeFormatter());
            GlobalConfiguration.Configuration.Formatters.
                Add(new JQueryMvcFormUrlEncodedFormatter());
        }
```
