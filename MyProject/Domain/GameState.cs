using Domain.Enums;

namespace Domain
{
    public class GameState
    {
        public int GameStateId { get; set; }

        public bool NextMoveByPlayerA { get; set; }

        public string? PlayerABoardState { get; set; }

        public string? PlayerBBoardState { get; set; }

        public int? GameId { get; set; }
        public Game Game { get; set; } = default!;
    }
}