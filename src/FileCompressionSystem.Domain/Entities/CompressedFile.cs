namespace FileCompressionSystem.Domain.Entities;

public class CompressedFile
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
}