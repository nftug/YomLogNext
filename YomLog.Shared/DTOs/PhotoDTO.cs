using System.Text.Json.Serialization;

namespace YomLog.Shared.ValueObjects;

public class PhotoDTO : ValueObject<PhotoDTO>
{
    public Guid? Id { get; init; }

    public string Uri
        => "https://dummyimage.com/500x500/b8b8b8/303030.jpg";

    protected override bool EqualsCore(PhotoDTO other) => Id == other.Id;

    [JsonConstructor]
    public PhotoDTO() { }

    public PhotoDTO(Guid photoId)
    {
        Id = photoId;
    }
}
