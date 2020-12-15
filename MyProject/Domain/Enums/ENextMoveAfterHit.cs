using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum ENextMoveAfterHit
    {
        [Display(Name="Other Player")]
        OtherPlayer,
        
        [Display(Name="Same Player")]
        SamePlayer
    }
}