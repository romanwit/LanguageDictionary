using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguagesDictionary.Models;
public class Languages
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required(ErrorMessage = "{0} is required")]
    public int LanguageId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(2)]
    public required string LanguageValue { get; set; }
}