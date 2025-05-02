using System;
using Terminal.Gui;

namespace JobShopScheduler.Frontend
{
    public static class ComponentFactory
    {
        public static Label CreateLabel(string text, Pos x, Pos y)
        {
            return new Label(text)
            {
                X = x,
                Y = y
            };
        }

        public static Button CreateButton(string text, Pos x, Pos y, Action onClick)
        {
            var button = new Button(text)
            {
                X = x,
                Y = y
            };
            button.Clicked += onClick;
            return button;
        }
    }
}
