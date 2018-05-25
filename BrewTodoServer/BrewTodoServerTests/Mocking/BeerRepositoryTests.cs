﻿using BrewTodoServer;
using BrewTodoServer.Data;
using BrewTodoServer.Models;
using Effort;
using NUnit.Framework;
using System.Configuration;
using System.Linq;

namespace BrewTodoServerTests.Mocking
{
    [TestFixture]
    public class BeerRepositoryTests
    { 
        private DbContext _context;
        private IRepository<Beer> _beerRepository;
        private IRepository<BeerType> _beerTypeRepository;
        private IRepository<Brewery> _breweryRepository;
        private static readonly BeerType testBeerType = new BeerType
        {
            BeerTypeID = 0,
            BeerTypeName = "testType"
        };
        private static readonly State testState = new State
        {
            StateID = 0,
            StateAbbr = "FL"
        };
        private static readonly Brewery testBrewery = new Brewery
        {
            Name = "Test",
            Description = "whatever",
            ImageURL = "dsfds",
            Address = "123 Main street",
            ZipCode = "45335",
            State = testState,
            PhoneNumber = "fdfsfds",
            BusinessHours = "fdsfdsfs",
            HasFood = true,
            HasGrowler = true,
            HasMug = true,
            HasTShirt = true
        };

        [SetUp]
        public void SetUp()
        {
            string connStr = ConfigurationManager.ConnectionStrings["BrewTodoDb"].ConnectionString;
            var connection = DbConnectionFactory.CreateTransient();
            _context = new DbContext(connection);
            _beerRepository = new BeerRepository(_context);
            _beerTypeRepository = new BeerTypeRepository(_context);
            _breweryRepository = new BreweryRepository(_context);
        }

        [Test]
        public void GetBeer_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            const int nonExistingId = 5;
            // Arrange
            var Beer = new Beer
            {
                ABV = 5.0,
                BeerName = "test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            _context.Beers.Add(Beer);
            _context.SaveChanges();

            // Act
            var result = _beerRepository.Get(nonExistingId);  

            // Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public void GetBeer_WithExistingId_ReturnsBeer()
        {
            // Arrange
            var Beer = new Beer
            {
                ABV = 5.0,
                BeerName = "test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(Beer);
            _context.SaveChanges();

            // Act
            var retrievedBrewrey = _beerRepository.Get(Beer.BeerID);
            Assert.AreEqual(Beer.BeerName, retrievedBrewrey.BeerName);
        }

        [Test]
        public void DeleteBeer_ReturnsNull()
        {
            // Arrange
            var Beer = new Beer
            {
                ABV = 5.0,
                BeerName = "test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            int BeerIdNumber = _context.Beers.FirstOrDefault().BeerID;
            _beerRepository.Delete(BeerIdNumber);

            //Act
            var result = _beerRepository.Get(BeerIdNumber);

            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void DeleteBeer_BeerNotDeletedReturnsNotNull()
        {
            // Arrange
            var Beer = new Beer
            {
                ABV = 5.0,
                BeerName = "test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            int idDeleted = 2;
            int notDeleted = 3;
            _beerRepository.Delete(idDeleted);

            //Act
            var result = _beerRepository.Get(notDeleted);
            
            //Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public void DeleteBeer_BeerNotDeletedReturnsExpectedBeer()
        {
            // Arrange
            var Beer = new Beer
            {
                ABV = 5.0,
                BeerName = "test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            _context.Beers.Add(Beer);
            _context.SaveChanges();
            _context.Beers.Add(Beer);
            _context.SaveChanges();

            int idDeleted = 2;
            int notDeleted = 3;
            var expected = _beerRepository.Get(notDeleted);
            _beerRepository.Delete(idDeleted);

            //Act
            var actual = _beerRepository.Get(notDeleted);

            //Assert
            Assert.AreEqual(actual,expected);
        }
        [Test]
        public void PutBeer_AffectedFieldIsChanged()
        {
            // Arrange
            var beer = new Beer
            {
                ABV = 5.0,
                BeerName = "Test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(beer);
            _context.SaveChanges();

            // Act
            beer = _context.Beers.ToList().LastOrDefault();
            beer.BeerName = "newName";
            _beerRepository.Put(beer.BeerID, beer);
            var actual = _beerRepository.Get(beer.BeerID).BeerName;

            // Assert
            Assert.AreNotEqual(actual, "Test");
        }
        [Test]
        public void PutBeer_NonUpdatedBeerHasNoChange()
        {
            var dummyBeer = new Beer
            {
                ABV = 5.0,
                BeerName = "dummy",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "dummy"
            };
            var originalBeer = new Beer
            {
                ABV = 5.0,
                BeerName = "Test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(dummyBeer);
            _context.SaveChanges();
            _context.Beers.Add(originalBeer);
            _context.SaveChanges();

            var updatedBeer = _context.Beers.ToList().LastOrDefault();
            updatedBeer.BeerName = "newName";
            _beerRepository.Put(updatedBeer.BeerID, updatedBeer);
            
            var actual = _beerRepository.Get(updatedBeer.BeerID).BeerName; 

            Assert.AreNotEqual(actual, dummyBeer.BeerName);
            Assert.AreEqual(dummyBeer.BeerName, _context.Beers.ToList().FirstOrDefault().BeerName);
        }
        [Test]
        public void PutBeer_UnnaffectedFieldHasNoChange()
        {
            // Arrange
            var originalBeer = new Beer
            {
                ABV = 5.0,
                BeerName = "Test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };
            _context.Beers.Add(originalBeer);
            _context.SaveChanges();

            // Act
            var updatedBeer = _context.Beers.ToList().LastOrDefault();
            updatedBeer.BeerName = "newName";
            _beerRepository.Put(updatedBeer.BeerID, updatedBeer);
            var actual = _beerRepository.Get(updatedBeer.BeerID).Description;

            // Assert
            Assert.AreEqual(actual, originalBeer.Description);
        }
        [Test]
        public void PostBeer_ReturnsBeerExists()
        {
            // Arrange
            var beer = new Beer
            {
                ABV = 5.0,
                BeerName = "Test",
                BeerType = testBeerType,
                Brewery = testBrewery,
                Description = "test"
            };

            // Act
            var preResult = _context.Beers.Any();
            _context.Beers.Add(beer);
            _context.SaveChanges();

            var postResult = _context.Beers.Any();

            // Assert
            Assert.IsFalse(preResult);
            Assert.IsTrue(postResult);
        }
    }
}