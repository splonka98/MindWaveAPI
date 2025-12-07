using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstractions.Surveys;
using Infrastructure.Surveys;

namespace Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MindWaveDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ISurveyService, SurveyService>();
        services.AddScoped<ISurveyQuestionProvider, DbSurveyQuestionProvider>();

        return services;
    }
}