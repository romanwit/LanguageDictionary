using System;
using LanguagesDictionary.Models;
using Microsoft.EntityFrameworkCore;

namespace LanguagesDictionary.Data;

public class DictionaryDBContext : DbContext
{

    public DictionaryDBContext() { }
    public DictionaryDBContext(DbContextOptions options) : base(options)
    {

    }

    public virtual DbSet<Values> Values { get; set; }
    public virtual DbSet<Languages> Languages { get; set; }
    public virtual DbSet<Keys> Keys { get; set; }

}
