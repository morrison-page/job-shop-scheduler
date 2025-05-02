using Terminal.Gui;

namespace JobShopScheduler.Frontend
{
    public static class RightPaneBuilder
    {
        public static FrameView BuildRightPane(Label makespanLabel, Label solvingTimeLabel, Button exportButton)
        {
            var rightPane = new FrameView(" Execution Output ")
            {
                X = Pos.Percent(50),
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill()
            };

            rightPane.Add(makespanLabel, solvingTimeLabel, exportButton);
            return rightPane;
        }
    }
}
