public class Vocabulary
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public required string KnownLanguage_Word { get; set; }
    public required string TargetLanguage_Word { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool Learnt { get; set; }
    public DateTime? LastPracticed { get; set; }
    public DateTime? NextPractice { get; set; }
}