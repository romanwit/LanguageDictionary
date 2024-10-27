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

        public static void SetupMockDbSet<T>(Mock<DbSet<T>> mockSet, IQueryable<T> data) where T : class
        {
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }


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
            SetupMockDbSet(mockLanguages, languages);
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
            var languageEng = new Languages
            {
                LanguageId = 1,
                LanguageValue = "English"
            };
            var languageFr = new Languages
            {
                LanguageId = 1,
                LanguageValue = "Francais"
            };

            var languages = new List<Languages>
                {languageEng, languageFr}.AsQueryable();

            var keyHello = new Keys
            {
                KeyId = 1,
                KeyValue = "Hello"
            };

            var keyLaundry = new Keys
            {
                KeyId = 1,
                KeyValue = "Laundry"
            };

            var keys = new List<Keys>
                {keyHello, keyLaundry}.AsQueryable();

            var values = new List<Values>
            {
                new Values {
                    RowId = 1,
                    Key = keyHello,
                    Language = languageEng,
                    Value = "World" },
                new Values {
                    RowId = 2,
                    Key = keyLaundry,
                    Language = languageEng,
                    Value = "The laundry" },
                new Values {
                    RowId = 3,
                    Key = keyLaundry,
                    Language = languageFr,
                    Value = "La lassive" }
            }.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            SetupMockDbSet(mockLanguages, languages);

            var mockKeys = new Mock<DbSet<Keys>>();
            SetupMockDbSet(mockKeys, keys);

            var mockValues = new Mock<DbSet<Values>>();
            SetupMockDbSet(mockValues, values);

            _mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            _mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);
            _mockContext.Setup(c => c.Values).Returns(mockValues.Object);

            // Act
            var result = _controller.Get("English");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = new OkObjectResult(result.Result);
            Assert.That(okResult, Is.Not.Null);
            var json = JObject.Parse(okResult.Value.ToJson());
            Assert.That(json.First.Children().Children().Count(), Is.EqualTo(2));
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
            SetupMockDbSet(mockKeys, keys);

            var language = new Languages
            {
                LanguageValue = "English"
            };
            var languages = new List<Languages>
                {language}.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            SetupMockDbSet(mockLanguages, languages);

            var values = new List<Values> {
                new Values {
                    Key = key,
                    Language = language,
                    Value = "OldValue"
                }
            }.AsQueryable();

            var mockValues = new Mock<DbSet<Values>>();
            SetupMockDbSet(mockValues, values);

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
        public void Post_AddsNewValue_WhenValueDontExist()
        {

            var keys = new List<Keys> {
                new Keys {
                    KeyId = 1,
                    KeyValue = "Key"
                }
             }.AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            SetupMockDbSet(mockKeys, keys);

            var language = new Languages
            {
                LanguageId = 1,
                LanguageValue = "English"
            };
            var languages = new List<Languages>
                {language}.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            SetupMockDbSet(mockLanguages, languages);

            var listOfValues = new List<Values>();
            var values = listOfValues.AsQueryable();
            var mockValues = new Mock<DbSet<Values>>();
            SetupMockDbSet(mockValues, values);
            mockValues.Setup(m => m.Add(It.IsAny<Values>())).Callback<Values>(listOfValues.Add);

            var mockContext = new Mock<DictionaryDBContext>();
            mockContext.Setup(c => c.Values).Returns(mockValues.Object);
            mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            var controller = new DictionaryInformationController(_mockLogger.Object, mockContext.Object);

            var args = new UpdateValueArgs
            {
                LanguageValue = "English",
                KeyValue = "Key",
                Value = "Value"
            };

            // Act
            var result = controller.Post(args);
            var count = mockContext.Object.Values.Count();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var getResult = controller.Get("English");

            // Assert
            Assert.That(getResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = new OkObjectResult(getResult.Result);
            Assert.That(okResult, Is.Not.Null);
            var json = JObject.Parse(okResult.Value.ToJson());
            var data = json.First.Children().
                Children().ToArray();
            Assert.That(data.Count(), Is.EqualTo(1));
            var getValue = data[0].ToObject<Values>();
            Assert.That(getValue, Is.InstanceOf<Values>());
            Assert.That(getValue.Value, Is.EqualTo("Value"));
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
            SetupMockDbSet(mockKeys, keys);

            var language = new Languages
            {
                LanguageValue = "English"
            };
            var languages = new List<Languages>
                {language}.AsQueryable();

            var mockLanguages = new Mock<DbSet<Languages>>();
            SetupMockDbSet(mockLanguages, languages);

            var values = new List<Values> {
                new Values {
                    Key = key,
                    Language = language,
                    Value = "OldValue"
                }
            }.AsQueryable();

            var mockValues = new Mock<DbSet<Values>>();
            SetupMockDbSet(mockValues, values);

            var mockContext = new Mock<DictionaryDBContext>();
            mockContext.Setup(c => c.Values).Returns(mockValues.Object);
            mockContext.Setup(c => c.Languages).Returns(mockLanguages.Object);
            mockContext.Setup(c => c.Keys).Returns(mockKeys.Object);

            var arr = mockContext.Object.Values.ToArray();
            Assert.That(arr.Count, Is.EqualTo(1));
            var value = arr[0].Value;
            Assert.That(value, Is.EqualTo("OldValue"));

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
            Assert.That(arr[0].Value, Is.EqualTo("NewValue"));
            var getResult = controller.Get("English");

            // Assert
            Assert.That(getResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = new OkObjectResult(getResult.Result);
            Assert.That(okResult, Is.Not.Null);
            var json = JObject.Parse(okResult.Value.ToJson());
            var data = json.First.Children().
                Children().ToArray();
            Assert.That(data.Count(), Is.EqualTo(1));
            var getValue = data[0].ToObject<Values>();
            Assert.That(getValue, Is.InstanceOf<Values>());
            Assert.That(getValue.Value, Is.EqualTo("NewValue"));
        }

        [Test]
        public void AddKey_Returns406_WhenKeyAlreadyExists()
        {
            // Arrange
            var args = new AddKeyArgs { NewKey = "Hello" };
            var keys = new List<Keys> { new Keys { KeyId = 1, KeyValue = "Hello" } }.AsQueryable();

            var mockKeys = new Mock<DbSet<Keys>>();
            SetupMockDbSet(mockKeys, keys);

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
            SetupMockDbSet(mockKeys, keys);

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
            SetupMockDbSet(mockKeys, keys);

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
            SetupMockDbSet(mockKeys, keys);

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
            SetupMockDbSet(mockLanguages, languages);

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
            SetupMockDbSet(mockLanguages, languages);

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
            SetupMockDbSet(mockLanguages, languages);

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
