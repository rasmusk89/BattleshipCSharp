using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public virtual string Label { get; set; }
        public virtual string UserChoice { get; set; }

        public virtual Action MethodToExecute { get; set; }

        public MenuItem(string label, string userChoice, Action methodToExecute)
        {
            Label = label;
            UserChoice = userChoice;
            MethodToExecute = methodToExecute;
        }

        public override string ToString()
        {
            return UserChoice + ") " + Label;
        }
    }
}