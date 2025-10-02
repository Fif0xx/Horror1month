public enum PhotoRating
{
    Terrible,
    Bad,
    Normal,
    Good,
    Perfect,
}

public enum PhotoTargetType
{
    Small,
    Medium,
    Large,
}

public class PhotoResult
{
    
    public PhotoRating Rating { get; set; }
    public int Score { get; set; }
}
