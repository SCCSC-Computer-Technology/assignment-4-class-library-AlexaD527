namespace Lab4Library
{
    public class StateManager
    {
        //Method for Loading the States from the Database into the Combo Box
        public void LoadStatesIntoComboBox(ComboBox stateDrop)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var stateNames = db.States.OrderBy(state => state.Name).Select(state => state.Name).ToList();
                    stateDrop.DataSource = stateNames;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load states into ComboBox: {ex.Message}");
            }
        }
        //Data Grid
        public void LoadStatesIntoDataGridView(DataGridView statesDataGridView)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var states = db.States.OrderBy(state => state.Name).ToList();
                    statesDataGridView.DataSource = states;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load states into DataGridView: {ex.Message}");
            }
        }

        public void InsertState(State newState)
        {
            using (var db = new DataClasses1DataContext())
            {
                db.States.InsertOnSubmit(newState);
                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("State inserted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to insert state: {ex.Message}");
                }
            }
        }
        
        public void UpdateState(string selectedStateName, string birdText, string populationText)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var stateToUpdate = db.States.FirstOrDefault(state => state.Name == selectedStateName);
                    if (stateToUpdate != null)
                    {
                        stateToUpdate.StateBird = birdText;

                        if (!string.IsNullOrWhiteSpace(populationText) && int.TryParse(populationText, out int population))
                        {
                            stateToUpdate.Population = population;
                        }
                        else if (!string.IsNullOrWhiteSpace(populationText))
                        {
                            MessageBox.Show("Invalid population format.");
                            return;
                        }

                        db.SubmitChanges();
                        MessageBox.Show("State updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Selected state not found in the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update state: {ex.Message}");
            }
        }

        public void GenerateHtmlForState(string selectedStateName)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var selectedState = db.States.FirstOrDefault(s => s.Name == selectedStateName);
                    if (selectedState != null)
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string htmlFilePath = Path.Combine(desktopPath, "SelectedStateInfo.html");

                        StringBuilder htmlContent = new StringBuilder();
                        htmlContent.AppendLine("<html><head><title>State Information</title></head><body>");
                        htmlContent.AppendLine("<h1>Selected State Information</h1>");
                        htmlContent.AppendLine($"<p>Name: {selectedState.Name}</p>");
                        htmlContent.AppendLine($"<p>Population: {selectedState.Population}</p>");
                        htmlContent.AppendLine($"<p>Flag Description: {selectedState.FlagDescription}</p>");
                        htmlContent.AppendLine($"<p>State Flower: {selectedState.StateFlower}</p>");
                        htmlContent.AppendLine($"<p>State Bird: {selectedState.StateBird}</p>");
                        htmlContent.AppendLine($"<p>Colors: {selectedState.Colors}</p>");
                        htmlContent.AppendLine($"<p>Largest Cities: {selectedState.LargestCities}</p>");
                        htmlContent.AppendLine($"<p>Capitol: {selectedState.Capitol}</p>");
                        htmlContent.AppendLine($"<p>Median Income: {selectedState.MedianIncome}</p>");
                        htmlContent.AppendLine($"<p>Computer Jobs Percentage: {selectedState.ComputerJobsPercentage}</p>");
                        htmlContent.AppendLine("</body></html>");

                        File.WriteAllText(htmlFilePath, htmlContent.ToString());

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlFilePath) { UseShellExecute = true });

                        MessageBox.Show($"Information for {selectedStateName} has been saved to {htmlFilePath} and will open in your browser.");
                    }
                    else
                    {
                        MessageBox.Show("State not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate HTML for state information: {ex.Message}");
            }
        }
        //Method for searching for states
        public void SearchStates(string searchTerm, DataGridView statesDataGridView)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var searchResults = db.States
                        .Where(state => state.Name.Contains(searchTerm) ||
                                        state.StateBird.Contains(searchTerm) ||
                                        state.Capitol.Contains(searchTerm))
                        .ToList();

                    statesDataGridView.DataSource = searchResults;

                    if (searchResults.Count == 0)
                    {
                        MessageBox.Show("No entries found matching the search criteria.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to perform search: {ex.Message}");
            }
        }
        //Method for Sorting States by pop/income/alphabetically
        public void SortStates(string sortBy, DataGridView statesDataGridView)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    IQueryable<State> query;

                    switch (sortBy)
                    {
                        case "Population":
                            query = db.States.OrderBy(state => state.Population);
                            break;
                        case "MedianIncome":
                            query = db.States.OrderBy(state => state.MedianIncome);
                            break;
                        case "Alphabetically":
                            query = db.States.OrderBy(state => state.Name);
                            break;
                        default:
                            query = db.States;
                            break;
                    }

                    statesDataGridView.DataSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to sort states: {ex.Message}");
            }
        }
    }
}

