using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguagesDictionary.Models;

public class Values
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required(ErrorMessage = "{0} is required")]
    public int RowId { get; set; }

    [ForeignKey("KeyId")]
    [Required(ErrorMessage = "{0} is required")]
    public required Keys Key { get; set; }

    [ForeignKey("LanguageId")]
    [Required(ErrorMessage = "{0} is required")]
    public required Languages Language { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public required string Value { get; set; }
}