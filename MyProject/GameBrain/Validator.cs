using Domain.Enums;

namespace GameBrain
{
    public class Validator
    {
        public static bool OrientationIsValid(string input)
        {
            return input.ToLower() == "h" || input.ToLower() == "v";
        }

        public static bool ColumnInputIsValid(string input, int boardWidth)
        {
            if (input.Length < 1)
            {
                return false;
            }

            if (input.Contains("-") || input.Contains("+") || input.Contains("*") || input.Contains("/"))
            {
                return false;
            }

            var number = ConvertStringToInteger(input.ToUpper());

            return number <= boardWidth && number > 0;
        }

        public bool RowInputIsValid(string input, int boardHeight)
        {
            if (input.Contains("-") || input.Contains("+") || input.Contains("*") || input.Contains("/"))
            {
                return false;
            }

            if (IsNumeric(input))
            {
                return int.Parse(input) <= boardHeight && int.Parse(input) > 0;
            }

            return false;
        }

        public static bool BombCoordinatesAreValid(int column, int row, int boardWidth, int boardHeight,
            Player opponent)
        {
            var sizeValid = column >= 0 && column <= boardWidth && row >= 0 && row <= boardHeight;
            var noBomb = opponent.GetPlayerBoard()[column, row] == ECellState.Empty
                         || opponent.GetPlayerBoard()[column, row] != ECellState.Bomb
                         && opponent.GetPlayerBoard()[column, row] != ECellState.Hit;

            return sizeValid && noBomb;
        }


        private bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }

        public static int ConvertStringToInteger(string input)
        {
            int numericValueOfString;

            if (input.Length > 1)
            {
                char[] listOfInput = input.ToCharArray();
                numericValueOfString = 26 + char.ToUpper(listOfInput[1]) - 64;
            }
            else
            {
                var c = char.Parse(input);
                numericValueOfString = char.ToUpper(c) - 64;
            }

            return numericValueOfString;
        }
    }
}