using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;



namespace Chatbot
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Page
    {
        string connectionString = @"Sever=(localDB)\MSSQLLOCALDB;Database=TaskAsisstant;Trusted Connection=True";
        string step = "";
        string pendingTitle = "";
        string pendingDescription = "";
        string pendingReminder = "";
        string status = "";



        public About()
        {
            InitializeComponent();
            LoadTasks();

            Bot("welcome to chatbot");
            Bot("type:add task - task title");

            // Start the video loop automatically on startup
            BackgroundVideo.Play();

        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Rewind the video to the beginning and replay it
            BackgroundVideo.Position = TimeSpan.FromMilliseconds(1);
            BackgroundVideo.Play();

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string message = txtinput.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("please type something");
                return;
            }
            lstTask2.Items.Add("user: " + message);
            txtinput.Clear();
            ProcessMessage(message);
        }

        private void ProcessMessage(string message)
        {
            string msg = message.ToLower();

            // 1. Handle Delete Request
            if (IsDeleteRequest(message))
            {
                int id = ExtractNumber(message);
                if (id == 0)
                {
                    Bot("please type task number. e.g: delete 1");
                    return;
                }
                DeleteTask(id);
                return;
            }

            // 2. Handle Complete Request
            if (IsCompleteRequest(message))
            {
                int id = ExtractNumber(message);
                if (id == 0)
                {
                    Bot("please type task number. e.g: done 1");
                    return;
                }
                CompleteTask(id); // Code will handle output message inside this method
                return;
            }

            // 3. Handle Multistep Conversation Flow
            if (step == "description")
            {
                pendingDescription = message;
                if (pendingDescription.ToLower() == "none")
                {
                    pendingDescription = "NO description";
                }
                step = "reminder";
                Bot("enter reminder or none");
                return;
            }

            if (step == "reminder")
            {
                pendingReminder = message;
                if (pendingReminder.ToLower() == "none")
                {
                    pendingReminder = "none";
                }
                saveTask(pendingTitle, pendingDescription, pendingReminder);
                ClearingPendingTask();
                return;
            }

            // 4. Handle Show/List Requests
            if (msg.Contains("show") || msg.Contains("display") || msg.Contains("list") || msg.Contains("view"))
            {
                LoadTasks();
                Bot("here are ur tasks");
                return; // Added return to prevent falling through to task creation
            }

            // 5. Handle New Task Requests
            if (IsTaskRequest(message))
            {
                string title = ExtractTask(message);
                string description = ExtractDescription(message);
                string reminder = ExtractReminder(message);

                if (string.IsNullOrEmpty(title))
                {
                    Bot("please type the title");
                    return;
                }

                // Fixed logic: Check local 'reminder', not 'pendingReminder'
                if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(reminder))
                {
                    saveTask(title, description, reminder);
                    ClearingPendingTask();
                    return;
                }

                pendingTitle = title;

                if (string.IsNullOrEmpty(description))
                {
                    step = "description";
                    Bot("enter the description");
                    return;
                }

                pendingDescription = description;

                if (string.IsNullOrEmpty(reminder))
                {
                    step = "reminder";
                    Bot("enter the reminder");
                    return;
                }

                saveTask(pendingTitle, pendingDescription, reminder);
                ClearingPendingTask();
                return;
            } // Fixed: Properly closed IsTaskRequest block

            // Fallback if chatbot doesn't understand the command
            Bot("I didn't quite get that. Try saying 'add buy milk', 'list', 'done 1', or 'delete 1'.");
        } // Fixed: Properly closed ProcessMessage method

       


        //NLP in Regex
        private bool IsTaskRequest(string message)
        {

            return Regex.IsMatch(message,
            @"\b(add|create|make)\s+(a\s+)?(new\s+)?(task|reminder|todo)\b|" +
            @"\bset\s+(a\s+)?reminder\b|" + @"\bremind\s+me\b|" + @"\bi\s+need\s+to\b",
            RegexOptions.IgnoreCase);
        }

        private bool IsDeleteRequest(string message)
        {
            return Regex.IsMatch(
                message, @"\b(delete | remove | erase | clear| cancel)\b", RegexOptions.IgnoreCase);
        }

        private bool IsCompleteRequest(string message)
        {
            return Regex.IsMatch(
                message, @"\b(done | finish | complete | completed)\b", RegexOptions.IgnoreCase);
        }

        private int ExtractNumber(string message)
        {

            Match match = Regex.Match(message, @"\d+");
            if (match.Success)
            {

                return int.Parse(match.Value);
            }
            return 0;
        }

        private string ExtractTask(string message)
        {

            string task = message.Trim();
            task = Regex.Replace(task, @"\bdescription\b .* ", "", RegexOptions.IgnoreCase);
            task = Regex.Replace(task, @"\breminder\b .* ", "", RegexOptions.IgnoreCase);
            task = Regex.Replace(task, @"^(please\s?(can you\s+)?(could you)\s+)?", "", RegexOptions.IgnoreCase);
            task = Regex.Replace(task, @" [ ?. ! ]+$", "");

            return task.Trim();
        }
        private string ExtractDescription(string message)
        {

            Match match = Regex.Match(message,
            @"\bdescription\b\s*[:\-]?\s*( .*? ](\breminder\b |$)",
            RegexOptions.IgnoreCase);
            if (match.Success)
            {

                return match.Groups[1].Value.Trim();
            }
            return "";
        }


        private string ExtractReminder(string message)
        {

            Match match = Regex.Match(message,
            @"\breminder\b\s*[:\-]?\s*( .*? ](\breminder\b |$)",
            RegexOptions.IgnoreCase);
            if (match.Success)
            {

                return match.Groups[1].Value.Trim();
            }
            return "";
        }


        private void saveTask(string title, string description, string reminder)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Removed single quotes around parameters
                    string query = @"INSERT INTO dbo.Table1(Title, Description, Reminder, IsCompleted)
                             VALUES(@Title, @Description, @Reminder, 0)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // 2. Map parameters BEFORE running ExecuteNonQuery
                    // 3. Map to the correct variables (title, description, reminder)
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Reminder", reminder);

                    // 4. Run the query after parameters are set
                    cmd.ExecuteNonQuery();

                    Bot("Task saved:" + title);
                    Bot("description:" + description);
                    Bot("Reminder:" + reminder);
                }
            }
            catch (Exception ex)
            {
                Bot("error: task not saved");
                MessageBox.Show(ex.Message);
            }
        }

        private void CompleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE dbo.Table1 SET IsCompleted = 1 WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int rows = cmd.ExecuteNonQuery();

                        // Fixed ternary string interpolation syntax
                        Bot(rows > 0 ? $"Task completed (ID: {id})" : $"{id} : task# does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Bot("error updating task");
                MessageBox.Show(ex.Message);
            }
        }

        // Delete Function
        private void DeleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Fixed SQL Syntax: Removed invalid 'SET IsCompleted = 1'
                    string query = "DELETE FROM dbo.Table1 WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int rows = cmd.ExecuteNonQuery();

                        // Fixed ternary string interpolation syntax
                        Bot(rows > 0 ? $"Task deleted (ID: {id})" : $"{id} : task# does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Bot("error: could not delete task");
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadTasks()
        {
            lstTask.Items.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Fixed: Added 'conn' to the SqlCommand constructor
                    string query = "SELECT Id, Title, Description, Reminder, IsCompleted FROM dbo.Table1 ORDER BY Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Read status explicitly from the database row
                            bool isCompleted = Convert.ToBoolean(reader["IsCompleted"]);
                            string status = isCompleted ? "[Completed]" : "[Pending]";

                            lstTask.Items.Add($"{reader["Id"]} {reader["Title"]} | {reader["Description"]} | {reader["Reminder"]} {status}");
                        }
                    }

                    if (lstTask.Items.Count == 0)
                    {
                        lstTask.Items.Add("no tasks saved yet");
                    }
                }
            }
            catch (Exception ex)
            {
                Bot("error: failed to load data");
                MessageBox.Show(ex.Message); // Helpful for debugging why it failed
            }
        }

        private void ClearingPendingTask()
        {
            step = "";
            pendingTitle = "";
            pendingDescription = "";
            pendingReminder = "";
        }

        private void Bot(string text)
        {
            lstTask.Items.Add("chatbot: " + text);
        }
    }
}




  