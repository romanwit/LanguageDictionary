using LanguagesDictionary.Controllers;
using LanguagesDictionary.Data;
using LanguagesDictionary.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace LanguagesDictionary.Tests
{
    [TestFixture]
    public class DictionaryInformationControllerTests
    {
        private Mock<DictionaryDBContext> _mockContext;
        private Mock<ILogger<DictionaryInformationController>> _mockLogger;
        private DictionaryInformationController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<DictionaryDBContext>();
            _mockLogger = new Mock<ILogger<DictionaryInformationController>>();
            _controller = new DictionaryInformationController(_mockLogger.Object, _mockContext.Object);
        }

        [Test]
        public void Get_Returns404_WhenLanguageDoesNotExist()
        {
            // Arrange
            var languages = new List<Languages> { new Languages { LanguageValue = "English" } }.AsQueryable();
            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());

            _mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);

            // Act
            var result = _controller.Get("Spanish");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundObject = result.Result as NotFoundObjectResult;
            Assert.That(notFoundObject.StatusCode, Is.EqualTo(404));

        }

        [Test]
        public void Get_ReturnsListOfValues_WhenLanguageExists()
        {
            // Arrange
            var language = new Languages
            {
                LanguageId = 1,
                LanguageValue = "English"
            };

            var languages = new List<Languages>
                {language}.AsQueryable();

            var key = new Keys
            {
                KeyId = 1,
                KeyValue = "Hello"
            };

            var keys = new List<Keys>
                {key}.AsQueryable();

            var values = new List<Values>
            {
                new Values {
                    RowId = 1,
                    Key = key,
                    Language = language,
                    Value = "World" }
            }.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());

            var mockValues = new Mock<DbSet<Values>>();
            mockValues.As<IQueryable<Values>>().Setup(m => m.Provider).Returns(values.Provider);
            mockValues.As<IQueryable<Values>>().Setup(m => m.Expression).Returns(values.Expression);
            mockValues.As<IQueryable<Values>>().Setup(m => m.ElementType).Returns(values.ElementType);
            mockValues.As<IQueryable<Values>>().Setup(m => m.GetEnumerator()).Returns(values.GetEnumerator());

            _mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            _mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);
            _mockContext.Setup(c => c.Values).Returns(mockValues.Object);

            // Act
            var result = _controller.Get("English");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = new OkObjectResult(result.Result);
            //var returnedValues = okResult.Value as IEnumerable<Values>;
            Assert.That(okResult, Is.Not.Null);
            var json = JObject.Parse(okResult.Value.ToJson());
            Assert.That(json.First.Count, Is.EqualTo(1));
        }

        [Test]
        public void Post_AddsNewValue_WhenKeyAndLanguageDontExist()
        {

            var key = new Keys
            {
                KeyId = 1,
                KeyValue = "Key"
            };

            var keys = new List<Keys> { key }.AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());


            var language = new Languages
            {
                LanguageValue = "English"
            };
            var languages = new List<Languages>
                {language}.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());


            var values = new List<Values> {
                new Values {
                    Key = key,
                    Language = language,
                    Value = "OldValue"
                }
            }.AsQueryable();

            var mockValues = new Mock<DbSet<Values>>();
            mockValues.As<IQueryable<Values>>().Setup(m => m.Provider).Returns(values.Provider);
            mockValues.As<IQueryable<Values>>().Setup(m => m.Expression).Returns(values.Expression);
            mockValues.As<IQueryable<Values>>().Setup(m => m.ElementType).Returns(values.ElementType);
            mockValues.As<IQueryable<Values>>().Setup(m => m.GetEnumerator()).Returns(values.GetEnumerator());

            var mockContext = new Mock<DictionaryDBContext>();
            mockContext.Setup(c => c.Values).Returns(mockValues.Object);
            mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            var controller = new DictionaryInformationController(_mockLogger.Object, mockContext.Object);

            var args = new UpdateValueArgs
            {
                LanguageValue = "English",
                KeyValue = "KeyDoesntExist",
                Value = "NewValue"
            };

            // Act
            var result = controller.Post(args);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void Post_AddsNewValue_WhenKeyAndLanguageExist()
        {

            var key = new Keys
            {
                KeyId = 1,
                KeyValue = "Key"
            };

            var keys = new List<Keys> { key }.AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());


            var language = new Languages
            {
                LanguageValue = "English"
            };
            var languages = new List<Languages>
                {language}.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());


            var values = new List<Values> {
                new Values {
                    Key = key,
                    Language = language,
                    Value = "OldValue"
                }
            }.AsQueryable();

            var mockValues = new Mock<DbSet<Values>>();
            mockValues.As<IQueryable<Values>>().Setup(m => m.Provider).Returns(values.Provider);
            mockValues.As<IQueryable<Values>>().Setup(m => m.Expression).Returns(values.Expression);
            mockValues.As<IQueryable<Values>>().Setup(m => m.ElementType).Returns(values.ElementType);
            mockValues.As<IQueryable<Values>>().Setup(m => m.GetEnumerator()).Returns(values.GetEnumerator());

            var mockContext = new Mock<DictionaryDBContext>();
            mockContext.Setup(c => c.Values).Returns(mockValues.Object);
            mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            var controller = new DictionaryInformationController(_mockLogger.Object, mockContext.Object);

            var args = new UpdateValueArgs
            {
                LanguageValue = "English",
                KeyValue = "Key",
                Value = "NewValue"
            };

            // Act
            var result = controller.Post(args);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void AddKey_Returns406_WhenKeyAlreadyExists()
        {
            // Arrange
            var args = new AddKeyArgs { NewKey = "Hello" };
            var keys = new List<Keys> { new Keys { KeyId = 1, KeyValue = "Hello" } }.AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());

            _mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            // Act
            var result = _controller.AddKey(args);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(result.StatusCode, Is.EqualTo(406));
        }

        [Test]
        public void AddKey_AddsNewKey_WhenKeyDoesNotExist()
        {
            // Arrange
            var args = new AddKeyArgs { NewKey = "Hi" };
            var keys = new List<Keys>().AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());

            _mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);
            _mockContext.Setup(c => c.Keys.Add(It.IsAny<Keys>()));

            // Act
            var result = _controller.AddKey(args);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            _mockContext.Verify(c => c.Keys.Add(It.IsAny<Keys>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void EditKey_Returns404_WhenKeyNotFound()
        {
            // Arrange
            var args = new EditKeyArgs { OldKey = "Goodbye", NewKey = "Farewell" };
            var keys = new List<Keys>().AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());

            _mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            // Act
            var result = _controller.EditKey(args);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void EditKey_UpdatesKey_WhenKeyExists()
        {
            // Arrange
            var args = new EditKeyArgs { OldKey = "Hello", NewKey = "Hi" };
            var keys = new List<Keys> { new Keys { KeyId = 1, KeyValue = "Hello" } }.AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Provider).Returns(keys.Provider);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.Expression).Returns(keys.Expression);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.ElementType).Returns(keys.ElementType);
            mockKeys.As<IQueryable<Keys>>().Setup(m => m.GetEnumerator()).Returns(keys.GetEnumerator());

            _mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            // Act
            var result = _controller.EditKey(args);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void ListOfLanguages_ReturnsLanguagesList()
        {
            // Arrange
            var languages = new List<Languages>
            {
                new Languages { LanguageValue = "English" },
                new Languages { LanguageValue = "Spanish" }
            }.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());

            _mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);

            // Act
            var result = _controller.ListOfLanguages();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void AddLanguage_Returns406_WhenLanguageExists()
        {
            // Arrange
            var args = new AddLanguageArgs { Language = "English" };
            var languages = new List<Languages> { new Languages { LanguageValue = "English" } }.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());

            _mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);

            // Act
            var result = _controller.AddLanguage(args);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(result.StatusCode, Is.EqualTo(406));
        }

        [Test]
        public void AddLanguage_AddsNewLanguage_WhenLanguageDoesNotExist()
        {
            // Arrange
            var args = new AddLanguageArgs { Language = "Spanish" };
            var languages = new List<Languages>().AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Provider).Returns(languages.Provider);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.Expression).Returns(languages.Expression);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.ElementType).Returns(languages.ElementType);
            mockLanguages.As<IQueryable<Languages>>().Setup(m => m.GetEnumerator()).Returns(languages.GetEnumerator());

            _mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            _mockContext.Setup(c => c.Languages.Add(It.IsAny<Languages>()));

            // Act
            var result = _controller.AddLanguage(args);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            _mockContext.Verify(c => c.Languages.Add(It.IsAny<Languages>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
