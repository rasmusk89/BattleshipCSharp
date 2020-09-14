namespace MenuSystem
{
    public class MenuItem
    {
        public virtual string Label { get; set; }
        public virtual string UserChoice { get; set; }

        public MenuItem(string label, string userChoice)
        {
            Label = label;
            UserChoice = userChoice;
        }

        public override string ToString()
        {
            return UserChoice + ") " + Label;
        }
    }
}