using System;
using LanguagesDictionary.Models;
using Microsoft.EntityFrameworkCore;

namespace LanguagesDictionary.Data;

public class DictionaryDBContext : DbContext
{
    public DictionaryDBContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Values> Values { get; set; }
    public DbSet<Languages> Languages { get; set; }
    public DbSet<Keys> Keys { get; set; }

}
