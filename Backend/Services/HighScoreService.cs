﻿using System.Security.Claims;

using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Shared.Enums;
using Shared.Data.Models;

namespace Backend.Services
{
    public class HighScoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HighScoreService> _logger;

        public HighScoreService(ApplicationDbContext context, ILogger<HighScoreService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<LeaderboardEntry>?> GetGameHighScoresAsync(AvailableGames game)
        {
            try
            {
                var query = from a in _context.HighScores.Where(e => e.GameId == game)
                            join b in _context.Users
                                on a.Id equals b.Id
                            orderby a.HighScore descending
                            select new LeaderboardEntry(b.UserName ?? "Anon", a.HighScore, a.RecordDate);

                var res = await query.ToListAsync();

                return res;
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException)
            {
                _logger.LogError("{error}", e.Message);
            }

            return null;
        }

        public async Task<List<LeaderboardEntry>?> GetAllHighScoresAsync()
        {
            try
            {
                var query = from a in _context.HighScores
                            join b in _context.Users
                                on a.Id equals b.Id
                            orderby a.HighScore descending
                            select new LeaderboardEntry(b.UserName ?? "Anon", a.HighScore, a.RecordDate);

                return await query.ToListAsync();
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException)
            {
                _logger.LogError("{error}", e.Message);
            }

            return null;
        }

        public async Task<HighScoresEntry?> GetUserHighScoreAsync(AvailableGames game, string id)
        {
            var user_id = id;

            if (user_id == null)
            {
                return null;
            }

            try
            {
                var todoItem = await _context.HighScores
                    .Where(m => m.GameId == game && m.Id == user_id)
                    .SingleOrDefaultAsync();

                return todoItem;
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException || e is InvalidOperationException)
            {
                _logger.LogError("{error}", e.Message);
            }

            return null;
        }

        // Gal turetu neleisti rasyti mazesnio highscore?
        public async Task<bool> PutUserHighScoreAsync(AvailableGames game, int newHighScore, string id)
        {
            var user_id = id;
            if (user_id == null)
            {
                return false;
            }

            var entry = new HighScoresEntry()
            {
                GameId = game,
                Id = user_id,
                HighScore = newHighScore,
                RecordDate = DateTime.UtcNow
            };

            try
            {
                if (EntryExists(user_id, entry.GameId))
                {
                    _context.Entry(entry).State = EntityState.Modified;
                }
                else
                {
                    _context.Entry(entry).State = EntityState.Added;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("{error}", e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteUserHighScoreAsync(AvailableGames game, string id)
        {
            var user_id = id;
            var highScoresEntry = await _context.HighScores.FindAsync(user_id, game);
            if (highScoresEntry == null)
            {
                return false;
            }

            try
            {
                _context.HighScores.Remove(highScoresEntry);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("{error}", e.Message);
                return false;
            }

            return true;
        }

        private bool EntryExists(string id, AvailableGames gameId)
        {
            return _context.HighScores.Any(e => e.GameId == gameId && e.Id == id);
        }
    }
}