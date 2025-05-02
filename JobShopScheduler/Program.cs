using JobShopScheduler.Helpers;
using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using JobShopScheduler.Frontend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Terminal.Gui;
using System.Threading.Tasks;

namespace JobShopScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Schedule schedule = null;

            Application.Init();
            View top = Application.Top;

            // Left Pane Labels
            Label datasetLabel = ComponentFactory.CreateLabel("Dataset: Not selected", Pos.Center(), Pos.Center() - 4);
            Label solverLabel = ComponentFactory.CreateLabel("Solver: Not selected", Pos.Center(), Pos.Bottom(datasetLabel) + 1);
            Label exporterLabel = ComponentFactory.CreateLabel("Exporter: Not selected", Pos.Center(), Pos.Bottom(solverLabel) + 1);

            // Right Pane Labels
            Label makespanLabel = ComponentFactory.CreateLabel("Makespan: N/a", Pos.Center(), Pos.Center() - 4);
            Label solvingTimeLabel = ComponentFactory.CreateLabel("Solving Time: N/a", Pos.Center(), Pos.Bottom(makespanLabel) + 1);

            // Right Pane Buttons
            Button exportButton = null;
            exportButton = ComponentFactory.CreateButton("Export Schedule", Pos.Center(), Pos.Bottom(solvingTimeLabel) + 2, onClick: () => 
            {
                exportButton.Enabled = false;

                if (exporterLabel.Text.ToString() == "Exporter: Not selected")
                {
                    MessageBox.ErrorQuery("Error", "Please select an exporter before exporting the schedule.", "OK");
                    exportButton.Enabled = true; // Reactivate the button
                    return;
                }

                try
                {
                    // Extract selected exporter name
                    var exporterName = exporterLabel.Text.ToString().Replace("Exporter: ", "");

                    // Use reflection to instantiate exporter
                    var exporterType = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .FirstOrDefault(t => t.Name == exporterName && typeof(IScheduleExporter).IsAssignableFrom(t));

                    if (exporterType == null)
                    {
                        MessageBox.ErrorQuery("Error", "Failed to instantiate exporter.", "OK");
                        exportButton.Enabled = true; // Reactivate the button
                        return;
                    }

                    IScheduleExporter exporter = (IScheduleExporter)Activator.CreateInstance(exporterType);

                    // Export the schedule
                    if (schedule == null)
                    {
                        MessageBox.ErrorQuery("Error", "No schedule available to export. Please run the solver first.", "OK");
                        exportButton.Enabled = true; // Reactivate the button
                        return;
                    }

                    if (exporterLabel.Text.ToString() == "Exporter: ConsoleExporter")
                    {
                        Console.Clear();
                        exporter.Export(schedule);
                    }

                    exporter.Export(schedule);
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"An error occurred: {ex.Message}", "OK");
                }
                finally
                {
                    // Reactivate the button
                    exportButton.Enabled = true;
                }
            });

            // Left Pane Buttons
            Button solveButton = null;
            solveButton = ComponentFactory.CreateButton("Run Solver", Pos.Center(), Pos.Bottom(exporterLabel) + 2, async () =>
            {
                solveButton.Enabled = false;

                // Check if dataset, solver, and exporter are selected
                if (datasetLabel.Text.ToString() == "Dataset: Not selected")
                {
                    MessageBox.ErrorQuery("Error", "Please select a dataset before running the solver.", "OK");
                    solveButton.Enabled = true; // Reactivate the button
                    return;
                }

                if (solverLabel.Text.ToString() == "Solver: Not selected")
                {
                    MessageBox.ErrorQuery("Error", "Please select a solver before running the solver.", "OK");
                    solveButton.Enabled = true; // Reactivate the button
                    return;
                }

                try
                {
                    // Extract selected dataset, solver, and exporter names
                    var datasetPath = datasetLabel.Text.ToString().Replace("Dataset: ", "");
                    var solverName = solverLabel.Text.ToString().Replace("Solver: ", "");

                    // Use reflection to instantiate solver
                    var solverType = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .FirstOrDefault(t => t.Name == solverName && typeof(IScheduleSolver).IsAssignableFrom(t));

                    if (solverType == null)
                    {
                        MessageBox.ErrorQuery("Error", "Failed to instantiate solver.", "OK");
                        solveButton.Enabled = true; // Reactivate the button
                        return;
                    }

                    List<Job> jobs = JobReader.ReadJobs(datasetPath);

                    if (jobs == null || !jobs.Any())
                    {
                        MessageBox.ErrorQuery("Error", "Failed to parse jobs from the selected dataset.", "OK");
                        solveButton.Enabled = true; // Reactivate the button
                        return;
                    }

                    IScheduleSolver solver = (IScheduleSolver)Activator.CreateInstance(solverType, jobs);

                    // Run the solver asynchronously
                    Stopwatch stopwatch = new();
                    stopwatch.Start();

                    schedule = await Task.Run(() =>
                    {
                        Schedule bestSchedule = solver.Solve();
                        for (int i = 0; i < 40; i++)
                        {
                            Schedule compareSchedule = solver.Solve();
                            if (compareSchedule.Fitness < bestSchedule.Fitness)
                                bestSchedule = compareSchedule;
                        }
                        return bestSchedule;
                    });

                    stopwatch.Stop();

                    makespanLabel.Text = $"Makespan: {schedule.Fitness.ToString()}";
                    solvingTimeLabel.Text = $"Solving Time: {stopwatch.ElapsedMilliseconds.ToString()}ms";
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"An error occurred: {ex.Message}", "OK");
                }
                finally
                {
                    // Reactivate the button
                    solveButton.Enabled = true;
                }
            });


            // Build UI components
            MenuBar menu = MenuBarBuilder.BuildMenuBar(datasetLabel, solverLabel, exporterLabel);
            View leftPane = LeftPaneBuilder.BuildLeftPane(datasetLabel, solverLabel, exporterLabel, solveButton);
            View rightPane = RightPaneBuilder.BuildRightPane(makespanLabel, solvingTimeLabel, exportButton);

            // Frame setup
            View frame = new FrameView(" Job Shop Scheduler ")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            frame.Add(leftPane, rightPane);

            top.Add(menu, frame);

            Application.Run();
            Application.Shutdown();
        }
    }
}
