using Terminal.Gui;

namespace JobShopScheduler.Frontend
{
    public static class LeftPaneBuilder
    {
        public static FrameView BuildLeftPane(Label datasetLabel, Label solverLabel, Label exporterLabel, Button solveButton)
        {
            var leftPane = new FrameView(" Selected Items ")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill()
            };

            leftPane.Add(datasetLabel, solverLabel, exporterLabel, solveButton);
            return leftPane;
        }
    }
}
