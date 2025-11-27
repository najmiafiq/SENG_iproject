using NUnit.Framework;
using SENG_Game_Backend.src;
using System.Linq;

namespace GameBackend.Tests
{
    [TestFixture]
    public class FighterCharacterCatalogTests
    {
        private CharacterCatalog _catalog;

        // Setup method runs before each test
        [SetUp]
        public void Setup()
        {
            // Initialize a fresh catalog for every test to ensure isolation
            _catalog = new CharacterCatalog();
        }

        // --- 1. CRUD Tests ---

        [Test]
        public void Create_NewCharacter_AddsAndAssignsId()
        {
            // Arrange
            var character = new FighterCharacter { name = "TestFighter", style = "Boxing" };

            // Act
            _catalog.Add(character);
            var retrieved = _catalog.GetById(character.id);

            // Assert
            Assert.That(_catalog.GetAll().Count, Is.EqualTo(1), "The count should be 1 after adding one character.");
            Assert.That(character.id, Is.GreaterThan(0), "The catalog should assign a unique ID.");
            Assert.That(retrieved.name, Is.EqualTo("TestFighter"), "The retrieved character name must match.");
        }

        [Test]
        public void Read_ExistingCharacter_ReturnsCorrectCharacter()
        {
            // Arrange
            var character1 = new FighterCharacter { name = "Ryu", style = "Karate" };
            var character2 = new FighterCharacter { name = "Ken", style = "Karate" };
            _catalog.Add(character1);
            _catalog.Add(character2);

            // Act
            var retrieved = _catalog.GetById(character1.id);

            // Assert
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.name, Is.EqualTo("Ryu"));
        }

        [Test]
        public void Update_ExistingCharacter_ChangesProperties()
        {
            // Arrange
            var originalChar = new FighterCharacter { name = "OldName", healthBase = 1000, style = "OldStyle" };
            _catalog.Add(originalChar);
            int originalId = originalChar.id;

            var updatedChar = new FighterCharacter
            {
                id = originalId,
                name = "NewName",
                healthBase = 1200, // Stat change
                attackMultiplier = 1.25,
                style = "NewStyle"
            };

            // Act
            bool success = _catalog.Update(updatedChar);
            var retrieved = _catalog.GetById(originalId);

            // Assert
            Assert.That(success, Is.True, "Update operation should return true on success.");
            Assert.That(retrieved.name, Is.EqualTo("NewName"));
            Assert.That(retrieved.healthBase, Is.EqualTo(1200));
            Assert.That(retrieved.style, Is.EqualTo("NewStyle"));
        }

        [Test]
        public void Delete_ExistingCharacter_RemovesFromCatalog()
        {
            // Arrange
            var characterToDelete = new FighterCharacter { name = "DeletionTarget" };
            _catalog.Add(characterToDelete);
            int idToDelete = characterToDelete.id;

            // Act
            bool success = _catalog.Delete(idToDelete);
            var retrieved = _catalog.GetById(idToDelete);

            // Assert
            Assert.That(success, Is.True, "Delete operation should return true on success.");
            Assert.That(retrieved, Is.Null, "The character should be null after deletion.");
            Assert.That(_catalog.GetAll().Any(c => c.id == idToDelete), Is.False, "The character should not exist in the list.");
        }

        // --- 2. Random Data Generation Test ---

        [Test]
        public void GenerateSampleData_AddsCorrectNumberOfCharacters()
        {
            // Arrange
            int countToGenerate = 5;

            // Act
            _catalog.GenerateSampleData(countToGenerate);

            // Assert
            Assert.That(_catalog.GetAll().Count, Is.EqualTo(countToGenerate));
            Assert.That(_catalog.GetAll().All(c => c.matchesPlayed >= 0), Is.True, "All generated characters must have non-negative matches played.");
        }

        // --- 3. Negative and Edge Case Tests ---

        [Test]
        public void Read_NonExistentCharacter_ReturnsNull()
        {
            // Act
            var retrieved = _catalog.GetById(999); // ID 999 is guaranteed not to exist in a fresh catalog

            // Assert
            Assert.That(retrieved, Is.Null, "Reading a non-existent ID should return null.");
        }

        [Test]
        public void Update_NonExistentCharacter_ReturnsFalse()
        {
            // Arrange
            var nonExistentChar = new FighterCharacter { id = 999, name = "Ghost" };

            // Act
            bool success = _catalog.Update(nonExistentChar);

            // Assert
            Assert.That(success, Is.False, "Updating a non-existent ID should return false.");
        }

        [Test]
        public void Delete_NonExistentCharacter_ReturnsFalse()
        {
            // Act
            bool success = _catalog.Delete(999);

            // Assert
            Assert.That(success, Is.False, "Deleting a non-existent ID should return false.");
        }
    }
}