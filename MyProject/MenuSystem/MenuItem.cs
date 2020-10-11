using System;

namespace MenuSystem
{
    // public delegate string MenuActionType();  == Func<string>
    public sealed class MenuItem
    {
        public string Label { get; set; }
        public string UserChoice { get; set; }

        public Func<string>? MethodToExecute { get; set; }

        public MenuItem(string label, string userChoice, Func<string> methodToExecute)
        {
            Label = label.Trim();
            UserChoice = userChoice.Trim().ToLower();
            MethodToExecute = methodToExecute;
        }

        public override string ToString()
        {
            return UserChoice.ToUpper() + ") " + Label;
        }
    }
}