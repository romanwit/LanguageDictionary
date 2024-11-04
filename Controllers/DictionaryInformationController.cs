using LanguagesDictionary.Data;
using LanguagesDictionary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LanguagesDictionary.Controllers;

//[ApiController]
[Route("[controller]")]
public class DictionaryInformationController : ApiController
{

    private readonly DictionaryDBContext _context;

    private readonly ILogger<DictionaryInformationController> _logger;

    public DictionaryInformationController(
        ILogger<DictionaryInformationController> logger,
        DictionaryDBContext context
        )
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Values>> Get(string Language)
    {

        /*
                var killValues =
                    _context.Values;//.Where(row => row.RowId != 1);
                foreach (var killValue in killValues)
                {
                    _context.Values.Remove(killValue);
                }
                var killKeys =
                    _context.Keys;//.Where(row => row.KeyId != 1);
                foreach (var killKey in killKeys)
                {
                    _context.Keys.Remove(killKey);
                }
                var killLanguages =
                    _context.Languages;//.Where(row => row.KeyId != 1);
                foreach (var killLanguage in killLanguages)
                {
                    _context.Languages.Remove(killLanguage);
                }
                _context.SaveChanges();
        */

        if (!_context.Languages.Select(row => row.LanguageValue).
            Contains(Language))
        {
            var msg = $"Language {Language} does not exists";
            _logger.LogError(msg);
            return NotFound(msg);
        }

        _logger.LogInformation($"Getting details is started for {Language}");

        var keysWithoutValues = _context.Keys.Except(
            _context.Values.
            Where(v => v.Language.LanguageValue == Language).
            Select(v => v.Key).
            Distinct()
        );

        var keysWithoutValuesCount = keysWithoutValues.Count();

        if (keysWithoutValuesCount != 0)
        {
            _logger.LogInformation($"Keys without values are found, total {keysWithoutValues.Count()}");
        }

        var emptyValues = keysWithoutValues.Select(
            k => new Values
            {
                Key = new Keys
                {
                    KeyId = k.KeyId,
                    KeyValue = Regex.Unescape(k.KeyValue)
                },
                Language = new Languages
                {
                    LanguageId = 0,
                    LanguageValue = ""
                },
                Value = ""
            }
        );

        var fullValues = _context.Values.
            Include(row => row.Language).
            Include(row => row.Key).
            Select(
                row => new Values
                {
                    RowId = row.RowId,
                    Key = new Keys
                    {
                        KeyId = row.Key.KeyId,
                        KeyValue = Regex.Unescape(row.Key.KeyValue)
                    },
                    Language = row.Language,
                    Value = Regex.Unescape(row.Value)

                }).
            Where(row => row.Language.LanguageValue == Language);

        _logger.LogInformation($"All keys, total {fullValues.Count()}");

        var result = fullValues.ToList().Union(emptyValues.ToList()).
                    OrderBy(row => row.Key.KeyValue);
        return Ok(result);
    }

    [HttpPost]
    [Route(RequestsUrls.EditRequest)]
    public JsonResult Post(UpdateValueArgs args)
    {
        if (
            _context.Values.Count(v =>
                v.Language.LanguageValue == args.LanguageValue
                && v.Key.KeyValue == Regex.Escape(args.KeyValue)) == 0
        )
        {
            _logger.LogWarning("Value or language not found, starting adding value");
            var lang = _context.Languages.FirstOrDefault(
                l => l.LanguageValue == args.LanguageValue
            ) ?? _context.Languages.
                    FirstOrDefault(l =>
                        l.LanguageValue == Constants.defaultLanguage);


            if (!_context.Keys.Any(
                k => k.KeyValue ==
                Regex.Escape(args.KeyValue)))
            {
                _logger.LogError($"Key {args.KeyValue} not found");
                var errorResponse = new JsonResult($"Key {args.KeyValue} not found");
                errorResponse.StatusCode = 404;
                return errorResponse;
            }


            var key = _context.Keys.First(
                    k => k.KeyValue == Regex.Escape(args.KeyValue));

            if (lang != null)
            {
                var rowValues = new Values
                {
                    Language = lang,
                    Key = key,
                    Value = Regex.Escape(args.Value)
                };
                _logger.LogInformation($"New value added with rowId {rowValues.RowId}");
                _context.Values.Add(rowValues);
                _context.SaveChanges();
                rowValues.Value = args.Value;
                var response = new JsonResult(rowValues);
                response.StatusCode = 200;
                return response;
            }
            else
            {
                throw new Exception("Lang is null");
            }
        }
        var row = _context.Values.First(
            row => row.Language.LanguageValue == args.LanguageValue
            &&
            row.Key.KeyValue == Regex.Escape(args.KeyValue));
        row.Value = Regex.Escape(args.Value);
        _context.SaveChanges();
        _logger.LogInformation("Existed value updated successfully");
        row.Value = args.Value;
        var result = new JsonResult(row);
        result.StatusCode = 200;
        return result;
    }

    [HttpPost]
    [Route(RequestsUrls.AddKeyRequest)]
    public JsonResult AddKey(AddKeyArgs args)
    {

        if (_context.Keys.Any(row => row.KeyValue == Regex.Escape(args.NewKey)))
        {
            var response = new JsonResult($"Key {args.NewKey} exists already");
            response.StatusCode = 406;
            _logger.LogError($"Key {args.NewKey} exists already. Returning 406");
            return response;
        }

        var rowKey = new Keys
        {
            KeyValue = Regex.Escape(args.NewKey)
        };
        _context.Keys.Add(rowKey);
        _context.SaveChanges();
        _logger.LogInformation($"Successfully added new key {rowKey.KeyValue} with rowId {rowKey.KeyId}");
        return new JsonResult(args.NewKey);

    }

    [HttpPost]
    [Route(RequestsUrls.EditKeyRequest)]
    public JsonResult EditKey(EditKeyArgs args)
    {
        if (!_context.Keys.Any(row => row.KeyValue == Regex.Escape(args.OldKey)))
        {
            var response = new JsonResult($"Key {args.OldKey} not found");
            response.StatusCode = 404;
            _logger.LogError($"Key {args.OldKey} not found. Returning 404");
            return response;
        }

        var row = _context.Keys.FirstOrDefault(row => row.KeyValue == args.OldKey);
        if (row != null)
        {
            row.KeyValue = args.NewKey;
            _context.SaveChanges();

            _logger.LogInformation($"Key {args.OldKey} updated successfully to {args.NewKey}");

            return new JsonResult(row);
        }
        else
        {
            throw new Exception("row is null");
        }
    }

    [HttpGet]
    [Route(RequestsUrls.ListOfLanguagesRequest)]
    public IEnumerable<string> ListOfLanguages()
    {
        var result = _context.Languages.
            Select(row => row.LanguageValue).ToList();
        return result;
    }

    [HttpPost]
    [Route(RequestsUrls.AddLanguageRequest)]
    public JsonResult AddLanguage(AddLanguageArgs args)
    {
        if (_context.Languages.Any(row =>
            row.LanguageValue == args.Language))
        {
            var response = new JsonResult($"Language {args.Language} exists already");
            response.StatusCode = 406;
            _logger.LogError($"Key {args.Language} exists already. Returning 406");
            return response;
        }

        var newLanguage = new Languages
        {
            LanguageValue = args.Language
        };

        _context.Languages.Add(newLanguage);
        _context.SaveChanges();

        _logger.LogInformation($"Language {args.Language} added successfully");
        return new JsonResult(newLanguage);
    }
}
