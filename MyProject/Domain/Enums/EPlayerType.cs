using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum EPlayerType
    {
        [Display(Name="Human")]
        Human,
     
        [Display(Name="AI")]
        Ai
    }
}