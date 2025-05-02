using JobShopScheduler.Interfaces;
using NStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terminal.Gui;

namespace JobShopScheduler.Frontend
{
    public static class MenuBarBuilder
    {
        public static MenuBar BuildMenuBar(Label datasetLabel, Label solverLabel, Label exporterLabel)
        {
            return 
            new MenuBar(new MenuBarItem[] {
                new MenuBarItem("_File", new MenuItem[] {
                    new MenuItem("_Open Dataset", "Select a .csv dataset", () => {
                        var openDialog = new OpenDialog("Open Dataset", "Select a CSV file")
                        {
                            AllowsMultipleSelection = false,
                            CanChooseDirectories = false,
                            DirectoryPath = Path.GetFullPath("../../../Jobs/") // Set default path
                        };

                        // Show dialog
                        Application.Run(openDialog);

                        if (!openDialog.Canceled && openDialog.FilePath != null)
                        {
                            var selectedFile = openDialog.FilePath.ToString();
                                
                            // Validate extension
                            if (Path.GetExtension(selectedFile).ToLower() != ".csv")
                            {
                                MessageBox.ErrorQuery("Invalid File", "Please select a .csv file.", "OK");
                                return;
                            }
                                
                            // Display in UI
                            datasetLabel.Text = $"Dataset: {selectedFile}";
                        }
                    })
                }),
                new MenuBarItem("_Solver", new MenuItem[] {
                    new MenuItem("_Choose Solver", "Select a solver", () => { 
                        // Use reflection to get all classes implementing IScheduleSolver
                        var solverTypes = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => typeof(IScheduleSolver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                            .ToList();

                        if (solverTypes.Count == 0)
                        {
                            MessageBox.ErrorQuery("No Solvers Found", "No classes implementing IScheduleSolver were found.", "OK");
                            return;
                        }

                        // Create a list of solver names
                        var solverNames = solverTypes.Select(t => t.Name).ToArray();

                        int selectedIndex = MessageBox.Query("Select Solver", "Choose a solver:", solverNames.Select(ustring.Make).ToArray());

                        if (selectedIndex >= 0 && selectedIndex < solverNames.Length)
                        {
                            var selectedSolver = solverTypes[selectedIndex];

                            // Optional: Instantiate or use the selected solver here
                            solverLabel.Text = $"Solver: {selectedSolver.Name}";

                        }
                    })
                }),
                new MenuBarItem("_Exporter", new MenuItem[] {
                    new MenuItem("_Choose Exporter", "Select an exporter", () => {
                        List<Type> exporterTypes = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => typeof(IScheduleExporter).IsAssignableFrom(t) && ! t.IsInterface & !t.IsAbstract)
                            .ToList();

                        if (exporterTypes.Count == 0)
                        {
                            MessageBox.ErrorQuery("No Exporters Found", "No classes implementing IScheduleExporter were found.", "OK");
                            return;
                        }

                        // Create a list of solver names
                        string[] exporterNames = exporterTypes.Select(t => t.Name).ToArray();

                        int selectedIndex = MessageBox.Query("Select Exporter", "Choose a exporter:", exporterNames.Select(ustring.Make).ToArray());

                        if (selectedIndex >= 0 && selectedIndex < exporterNames.Length)
                        {
                            var selectedExporter = exporterTypes[selectedIndex];

                            // Optional: Instantiate or use the selected solver here
                            exporterLabel.Text = $"Exporter: {selectedExporter.Name}";
                        }
                    })
                }),
                new MenuBarItem("_Exit", new MenuItem[] {
                    new MenuItem("_Quit", "", () => Application.RequestStop())
                })
            });
        }
    }
}
