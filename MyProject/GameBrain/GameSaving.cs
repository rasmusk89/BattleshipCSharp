using System;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace GameBrain
{
    public class GameSaving
    {
        private static AppDbContext GetConnection()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(
                @"
                Server=barrel.itcollege.ee,1533;
                User Id=student;
                Password=Student.Bad.password.0;
                Database=raskil_db;
                MultipleActiveResultSets=true;
                ").Options;
            return new AppDbContext(dbOptions);
        }
        
        public void SaveGame(GameOptions options)
        {
            using var dbCtx = GetConnection();

            var gameOptions = new Domain.GameOption()
            {
                BoardWidth = options.GetBoardWidth(),
                BoardHeight = options.GetBoardHeight(),
                EShipsCanTouch = options.GetShipsCanTouch(),
                NextMoveAfterHit = options.GetNextMoveAfterHit(),
                NextMoveByPlayerA = options.GetNextMoveByPlayerA()
            };
        }
    }
}