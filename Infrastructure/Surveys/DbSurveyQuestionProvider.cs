using Application.Abstractions.Surveys;
using Application.Contracts.Surveys;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Surveys;

public sealed class DbSurveyQuestionProvider : ISurveyQuestionProvider
{
    private readonly MindWaveDbContext _db;

    public DbSurveyQuestionProvider(MindWaveDbContext db) => _db = db;

    public QuestionDto[] GetInitiatingQuestions()
    {
        return _db.SurveyTemplates
            .Where(t => t.Name == "daily" && t.EpisodePath == "initial")
            .SelectMany(t => t.Questions.OrderBy(q => q.Order))
            .Select(q => new QuestionDto { Id = q.Id, Text = q.Text })
            .ToArray();
    }

    public QuestionDto[] GetFollowupQuestions(string path)
    {
        return _db.SurveyTemplates
            .Where(t => t.Name == "daily" && t.EpisodePath == path)
            .SelectMany(t => t.Questions.OrderBy(q => q.Order))
            .Select(q => new QuestionDto { Id = q.Id, Text = q.Text })
            .ToArray();
    }

    public int[] GetFollowupQuestionIds(string path)
    {
        return _db.SurveyTemplates
            .Where(t => t.Name == "daily" && t.EpisodePath == path)
            .SelectMany(t => t.Questions)
            .OrderBy(q => q.Order)
            .Select(q => q.Id)
            .ToArray();
    }
}