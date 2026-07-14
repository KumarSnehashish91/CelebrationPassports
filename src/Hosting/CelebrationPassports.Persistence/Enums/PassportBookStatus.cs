namespace CelebrationPassports.Persistence.Enums;

// ERD doesn't enumerate values for this one — starter lifecycle set, easy to extend.
public enum PassportBookStatus
{
    Draft = 1,
    Generating = 2,
    Ready = 3,
    Failed = 4
}
