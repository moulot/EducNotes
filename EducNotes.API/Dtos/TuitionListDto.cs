namespace EducNotes.API.Dtos
{
  public class TuitionListDto
  {
    public int ClassLevelId { get; set; }
    public string ClassLevelName { get; set; }
    public int NbTuitions { get; set; }
    public int NbTuitionsOK { get; set; }
    public decimal LevelAmount { get; set; }
    public string strLevelAmount { get; set; }
    public decimal LevelAmountOK { get; set; }
    public string strLevelAmountOK { get; set; }
  }
}