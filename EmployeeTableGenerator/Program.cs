using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

class Program
{
    static async Task Main()
    {
        // Replace {key} with the actual key
        string apiKey = "your_api_key_here";
        string apiEndpoint = $"https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={apiKey}";

        // Retrieve JSON data from the API endpoint
        List<Employee> employees = await GetEmployees(apiEndpoint);

        // Order employees by total time worked
        employees.Sort((a, b) => a.TotalTimeWorked.CompareTo(b.TotalTimeWorked));

        // Generate HTML content
        string htmlContent = GenerateHtml(employees);

        // Save HTML content to a file or display it as needed
        Console.WriteLine(htmlContent);

        Console.ReadLine(); // Keep console window open
    }

    static async Task<List<Employee>> GetEmployees(string apiEndpoint)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Employee>>();
            }
            else
            {
                throw new Exception($"Failed to fetch data from {apiEndpoint}. Status code: {response.StatusCode}");
            }
        }
    }

    static string GenerateHtml(List<Employee> employees)
    {
        string html = @"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    table {
                        font-family: Arial, sans-serif;
                        border-collapse: collapse;
                        width: 100%;
                    }

                    th, td {
                        border: 1px solid #dddddd;
                        text-align: left;
                        padding: 8px;
                    }

                    tr:nth-child(even) {
                        background-color: #f2f2f2;
                    }

                    tr.less-than-100 {
                        background-color: #ffcccc;
                    }
                </style>
            </head>
            <body>
                <h2>Employee Table</h2>
                <table>
                    <tr>
                        <th>Name</th>
                        <th>Total Time Worked</th>
                    </tr>";

        foreach (var employee in employees)
        {
            html += $@"
                    <tr {(employee.TotalTimeWorked < 100 ? "class='less-than-100'" : "")}>
                        <td>{employee.Name}</td>
                        <td>{employee.TotalTimeWorked} hours</td>
                    </tr>";
        }

        html += @"
                </table>
            </body>
            </html>";

        return html;
    }
}

class Employee
{
    public string Name { get; set; }
    public int TotalTimeWorked { get; set; }
}
