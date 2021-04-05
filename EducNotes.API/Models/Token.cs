using System;

namespace EducNotes.API.Models
{
  public class Token
  {
    public Token()
    {
      Internal = true;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string TokenString { get; set; }
    public Boolean Internal { get; set; }
    public int? TokenTypeId { get; set; }
    public TokenType TokenType { get; set; }

  }
}