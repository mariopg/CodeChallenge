using CodeChallenge.Server.DB;
using CodeChallenge.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Server.Helpers
{
    public class DataErrorLog
    {
        private List<ErrorLog> errors;

        public DataErrorLog()
        {
            errors = new List<ErrorLog>();
        }

        public void AddError(int mainErrorType, int errorType, int columnType, int rowNumber)
        {
            ErrorLog newError = new ErrorLog
            {
                type = MainErrorType(mainErrorType),
                description = ErrorType(errorType) + ColumnType(columnType),
                row = rowNumber
            };
            errors.Add(newError);
        }

        public void AddGeneralError(int mainErrorType, int errorType)
        {
            ErrorLog newError = new ErrorLog
            {
                type = MainErrorType(mainErrorType),
                description = ErrorType(errorType),
                row = 0
            };
            errors.Add(newError);
        }

        private string MainErrorType(int code)
        {
            switch(code)
            {
                case 1:
                    {
                        return "CSV loading";
                    }
                case 2:
                    {
                        return "Duplicated value";
                    }
                case 3:
                    {
                        return "Invalid file format";
                    }
                case 4:
                    {
                        return "Database error";
                    }
                default:
                    {
                        return "Not defined";
                    }
            }
        }

        private string ErrorType(int code)
        {
            switch (code)
            {
                case 1:
                    {
                        return "Incorrect value for ";
                    }
                case 2:
                    {
                        return "Value already exists for column ";
                    }
                case 3:
                    {
                        return "Not enough data to read";
                    }
                case 4:
                    {
                        return "Invalid number of columns";
                    }
                case 5:
                    {
                        return "Undefined error while saving changes to database";
                    }
                default:
                    {
                        return "Not defined";
                    }
            }
        }

        private string ColumnType(int column)
        {
            switch (column)
            {
                case 1:
                    {
                        return "Rank";
                    }
                case 2:
                    {
                        return "Team";
                    }
                case 3:
                    {
                        return "Mascot";
                    }
                case 4:
                    {
                        return "Last date of win";
                    }
                case 5:
                    {
                        return "Winning percentage";
                    }
                case 6:
                    {
                        return "Wins";
                    }
                case 7:
                    {
                        return "Losses";
                    }
                case 8:
                    {
                        return "Ties";
                    }
                case 9:
                    {
                        return "Games";
                    }
                default:
                    {
                        return "Not defined";
                    }
            }
        }

        public async Task SaveLog()
        {
            if(errors.Count > 0)
            {
                string path = "./Log";

                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                List<string> errorList = ErrorList();

                var fileName = $"import_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
                var filePath = Path.Combine(path, fileName);

                await File.WriteAllLinesAsync(filePath, errorList);
            }
        }

        private List<string> ErrorList()
        {
            List<string> errorFullText = new List<string>();
            foreach (ErrorLog error in errors)
            {
                string value = error.type + ": " + error.description + " at Row: " + error.row;
                errorFullText.Add(value);
            }

            return errorFullText;
        }
    }
}
