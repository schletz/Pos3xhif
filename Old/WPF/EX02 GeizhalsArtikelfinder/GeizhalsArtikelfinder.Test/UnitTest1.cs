using System;
using GeizhalsArtikelfinder.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeizhalsArtikelfinder.Test
{
    [TestClass]
    public class GeizhalsArtikelfinderTestclass
    {
        readonly MainViewModel viewModel;
        public GeizhalsArtikelfinderTestclass()
        {
            viewModel = new MainViewModel();
        }

        /// <summary>
        /// Prüft, ob die Suche bei leerem von und bis Datum funktioniert.
        /// </summary>
        [TestMethod]
        public void SearchEanOnlyTest()
        {
            viewModel.Ean = "1000008";
            viewModel.ArticleSearchCommand.Execute(null);
            Assert.IsTrue(viewModel.CurrentArticle.Name == "Facing the Truth (At kende sandheden)");
            Assert.IsTrue(viewModel.CurrentArticle.MaxPrice == 254.8538M);
            Assert.IsTrue(viewModel.CurrentArticle.MinPrice == 160.4549M);
            // Bei der double Methode von Assert kann ein Epsilon zum Berücksichtigen von eventuell vorhandenen
            // Rundungsfehlern angegeben werden.
            Assert.AreEqual(216.5174188, (double)viewModel.CurrentArticle.AvgPrice, 0.001);
        }
        /// <summary>
        /// Prüft, ob die Suche bei eingestelltem von und leerem bis Datum funktioniert.
        /// </summary>
        [TestMethod]
        public void SearchDateFromTest()
        {
            viewModel.Ean = "1000008";
            viewModel.DateFrom = "13.11.2018";

            viewModel.ArticleSearchCommand.Execute(null);
            Assert.IsTrue(viewModel.CurrentArticle.Name == "Facing the Truth (At kende sandheden)");
            Assert.IsTrue(viewModel.CurrentArticle.MaxPrice == 254.8538M);
            Assert.IsTrue(viewModel.CurrentArticle.MinPrice == 160.4549M);
            Assert.AreEqual(219.3795852, (double)viewModel.CurrentArticle.AvgPrice, 0.001);
        }
        /// <summary>
        /// Prüft, ob die Suche bei leerem von und angegebenen bis Datum funktioniert.
        /// </summary>
        [TestMethod]
        public void SearchDateToTest()
        {
            viewModel.Ean = "1000008";
            viewModel.DateTo = "15.11.2018";

            viewModel.ArticleSearchCommand.Execute(null);
            Assert.IsTrue(viewModel.CurrentArticle.Name == "Facing the Truth (At kende sandheden)");
            Assert.IsTrue(viewModel.CurrentArticle.MaxPrice == 254.8538M);
            Assert.IsTrue(viewModel.CurrentArticle.MinPrice == 160.4549M);
            Assert.AreEqual(212.5542893, (double)viewModel.CurrentArticle.AvgPrice, 0.001);
        }
        /// <summary>
        /// Prüft, ob die Suche bei eingestellten von und bis Datum funktioniert.
        /// </summary>
        [TestMethod]
        public void SearchDateFromToTest()
        {
            viewModel.Ean = "1000008";
            viewModel.DateFrom = "16.11.2018";
            viewModel.DateTo = "17.11.2018";

            viewModel.ArticleSearchCommand.Execute(null);
            Assert.IsTrue(viewModel.CurrentArticle.Name == "Facing the Truth (At kende sandheden)");
            Assert.IsTrue(viewModel.CurrentArticle.MaxPrice == 253.4952M);
            Assert.IsTrue(viewModel.CurrentArticle.MinPrice == 161.3187M);
            Assert.AreEqual(222.0658, (double)viewModel.CurrentArticle.AvgPrice, 0.001);
        }
    }
}
