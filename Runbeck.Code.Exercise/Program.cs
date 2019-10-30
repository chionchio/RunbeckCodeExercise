using System;

namespace Runbeck.Code.Exercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Ask for file location and exit if it does not exist.
            Console.WriteLine("File Location:");
            var fileLocation = Console.ReadLine();

            // Parse quotes for file paths with spaces if quotes exists.
            if(fileLocation.StartsWith("\"") || fileLocation.EndsWith("\""))
            {
                fileLocation = fileLocation.Replace("\"", "");
            }

            if (string.IsNullOrEmpty(fileLocation) || !System.IO.File.Exists(fileLocation))
            {
                WriteConsoleError("File not found");
                return;
            }

            // Ask for file format, exit if format is not one of the two formats.
            Console.WriteLine();
            Console.WriteLine("File format C = CSV (comma-separated values) , T = TSV (tab-separated values):");
            var keyCode = Console.ReadKey();
            if (keyCode.Modifiers == 0 && keyCode.KeyChar != 'C' && keyCode.KeyChar != 'T' && keyCode.KeyChar != 'c' && keyCode.KeyChar != 't')
            {
                WriteConsoleError("Incorrect format selected.");
                return;
            }

            // Ask for the number of fields that should be in each row. Exit if a number is not provided.
            Console.WriteLine();
            Console.WriteLine("How many fields:");
            var numberOfFields = Console.ReadLine();
            if(!int.TryParse(numberOfFields,System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out int numberOfFieldsResult))
            {
                WriteConsoleError("Incorrect number.");
                return;
            }

            // Process the file.
            ProcessFile(fileLocation, keyCode.KeyChar, numberOfFieldsResult);
            Console.WriteLine("File Processed");
        }

        /// <summary>
        /// Write an error message to the console window.
        /// </summary>
        /// <param name="message">The error message to write.</param>
        private static void WriteConsoleError(string message)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }

        /// <summary>
        /// Process file with provided format and field count.
        /// </summary>
        /// <param name="fileLocation">File location.</param>
        /// <param name="format">Format of file C = CSV (comma-separated values) , T = TSV (tab-separated values).</param>
        /// <param name="fieldCount">The number of fields in each row.</param>
        private static void ProcessFile(string fileLocation, char format, int fieldCount)
        {
            var directory = System.IO.Path.GetDirectoryName(fileLocation);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(fileLocation);
            var fileExtension = System.IO.Path.GetExtension(fileLocation);

            // Since no instuction on what names or location to write result files I will use
            // the file location of original file with good or bad appended to file name.
            string goodFilePath = string.Format("{0}{1}{2}", System.IO.Path.Combine(directory, fileName), ".good.", fileExtension);
            string badFilePath = string.Format("{0}{1}{2}", System.IO.Path.Combine(directory, fileName), ".bad.", fileExtension);

            using (var fileStream = new System.IO.FileStream(fileLocation, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var textStream = new System.IO.StreamReader(fileStream);

                // Skip header line.
                var headerLine = textStream.ReadLine();
                while (!textStream.EndOfStream)
                {
                    var line = textStream.ReadLine();
                    // Skip empty line
                    if (string.IsNullOrEmpty(line.Trim()))
                        continue;

                    string[] lineParts;
                    switch (format)
                    {
                        case 'C':
                        case 'c':

                            // CSV file
                            lineParts = line.Split(',');
                            break;
                        default:

                            // TSV file
                            lineParts = line.Split('\t');
                            break;
                    }

                    // Based on the logic if no incorrect lines are found no file will be created.
                    // Same goes for the correct line.
                    if(lineParts.Length != fieldCount)
                    {
                        // Write to incorrect file.
                        using(var badFile = new System.IO.FileStream(badFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                        {
                            WriteFileLine(line, badFile);
                        }
                    }
                    else
                    {
                        // Write to correct file.
                        using (var goodFile = new System.IO.FileStream(goodFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                        {
                            WriteFileLine(line, goodFile);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write line to output stream.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <param name="stream">The file stream to write to.</param>
        private static void WriteFileLine(string line, System.IO.FileStream stream)
        {
            using (var streamWriter = new System.IO.StreamWriter(stream))
            {
                streamWriter.WriteLine(line);
            }
        }
    }
}
