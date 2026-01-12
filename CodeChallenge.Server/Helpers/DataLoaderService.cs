using CodeChallenge.Server.DB;
using CodeChallenge.Server.Models;
using System.Globalization;
using System;
using System.Runtime.CompilerServices;
using System.Reflection.PortableExecutable;

namespace CodeChallenge.Server.Helpers
{
    public class DataLoaderService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly InitializationState _state;
        private CollegeFootballContext _context;
        const int headerCount = 9;
        private DataErrorLog errors;
        
        public DataLoaderService(IServiceProvider serviceProvider, InitializationState state)
        {
            _serviceProvider = serviceProvider;
            _state = state;
            var scope = _serviceProvider.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<CollegeFootballContext>();
            errors = new DataErrorLog();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await LoadCsvAsync("./Data/CollegeFootballTeamWinsWithMascots.csv");
            await errors.SaveLog();
            _state.IsReady = true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        private async Task LoadCsvAsync(string path)
        {
            string[] rows = File.ReadAllLines(path);

            if (rows.Length <= 1)
            {
                errors.AddGeneralError(3, 3);
                return;
            }
            List<FileColumnHeader> headerData = LoadHeaders(rows[0]);
            if (headerData is null)
            {
                errors.AddGeneralError(3, 4);
                return;
            }
            List<TeamStats> dataRows = LoadRows(rows);
            await LoadToDBAsync(headerData, dataRows);

        }

        private List<FileColumnHeader> LoadHeaders(string headerRow)
        {
            var cols = new List<FileColumnHeader>();

            string[] headers = headerRow.Split(',');
            if (headers.Length != headerCount)
            {
                return null;
            }
            for(int i = 0; i < headers.Length; i++)
            {
                var columnHeaderName = headers[i].Trim().Length > 0 ? headers[i].Trim() : "UNKNOWN";
                cols.Add(new FileColumnHeader {
                    ColumnHeaderName = columnHeaderName
                });
            }
            return cols;
        }

        private List<TeamStats> LoadRows(string[] rows)
        {
            var stats = new List<TeamStats>();

            for (int i = 1; i < rows.Length; i++)
            {
                var rowData = rows[i].Split(',');
                if (rowData.Length == headerCount)
                {
                    TeamStats newStats = ValidateRow(rowData, i + 1);
                    if (newStats != null)
                    {
                        if(ValidateDuplicates(stats, newStats, i + 1))
                        {
                            stats.Add(newStats);
                        }
                    }
                }
            }
            return stats;
        }

        private TeamStats ValidateRow(string[] rowData, int rowNumber)
        {
            TeamStats statsData = new TeamStats();
            try
            {
                statsData.Rank = DataParser.IntParser(1, rowData[0]);
                if (statsData.Rank < 1)
                {
                    errors.AddError(1, 1, 1, rowNumber);
                    return null;
                }
                statsData.Team = rowData[1].Trim();
                if (statsData.Team.Length < 1)
                {
                    errors.AddError(1, 1, 2, rowNumber);
                    return null;
                }
                statsData.Mascot = rowData[2].Trim();
                if (statsData.Mascot.Length < 1)
                {
                    errors.AddError(1, 1, 3, rowNumber);
                    return null;
                }
                var isDate = DataParser.DateParser("M/d/yy", rowData[3]);
                if (!isDate)
                {
                    errors.AddError(1, 1, 4, rowNumber);
                    return null;
                }
                statsData.LastWinDate = rowData[3];
                statsData.Percentage = DataParser.DoubleParser(0, rowData[4]);
                if (statsData.Percentage < 0)
                {
                    errors.AddError(1, 1, 5, rowNumber);
                    return null;
                }
                statsData.Wins = DataParser.IntParser(0, rowData[5]);
                if (statsData.Wins < 0)
                {
                    errors.AddError(1, 1, 6, rowNumber);
                    return null;
                }
                statsData.Losses = DataParser.IntParser(0, rowData[6]);
                if (statsData.Losses < 0)
                {
                    errors.AddError(1, 1, 7, rowNumber);
                    return null;
                }
                statsData.Ties = DataParser.IntParser(0, rowData[7]);
                if (statsData.Ties < 0)
                {
                    errors.AddError(1, 1, 8, rowNumber);
                    return null;
                }
                statsData.Games = DataParser.IntParser(0, rowData[8]);
                if (statsData.Games < 0)
                {
                    errors.AddError(1, 1, 9, rowNumber);
                    return null;
                }
            }
            catch
            {
                return null;
            }
            return statsData;
        }

        private bool ValidateDuplicates(List<TeamStats> loaded, TeamStats newStats, int rowNumber)
        {
            if(loaded.Exists(x => x.Rank == newStats.Rank))
            {
                errors.AddError(2, 2, 1, rowNumber);
                return false;
            }
            if (loaded.Exists(x => x.Team == newStats.Team))
            {
                errors.AddError(2, 2, 2, rowNumber);
                return false;
            }
            if (loaded.Exists(x => x.Mascot == newStats.Mascot))
            {
                errors.AddError(2, 2, 3, rowNumber);
                return false;
            }
            return true;
        }

        private async Task LoadToDBAsync(List<FileColumnHeader> headers, List<TeamStats> data)
        {
            try
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
                _context.FileColumns.AddRange(headers);
                _context.TeamStats.AddRange(data);
                await _context.SaveChangesAsync();
            }
            catch 
            {
                errors.AddGeneralError(4, 5);
            };
        }

        
    }
}
