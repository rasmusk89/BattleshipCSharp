namespace AnotherDomain
{
    public class PlayerBoardState
    {
        public int PlayerBoardStateId { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        
        //Serialized from JSON
        public string BoardState { get; set; } = null!;


    }
}