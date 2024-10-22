using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguagesDictionary.Models;

public class Keys
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required(ErrorMessage = "{0} is required")]
    public int KeyId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public required string KeyValue { get; set; }
}