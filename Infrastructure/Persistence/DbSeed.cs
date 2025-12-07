using System;
using System.Linq;
using Domain.Surveys;

namespace Infrastructure.Persistence;

public static class DbSeed
{
    public static void SeedDailySurvey(MindWaveDbContext db)
    {
        if (db.SurveyTemplates.Any(t => t.Name == "daily"))
        {
            return;
        }

        var initialId = Guid.NewGuid();
        var depId = Guid.NewGuid();
        var hypoId = Guid.NewGuid();
        var maniaId = Guid.NewGuid();

        var initial = SurveyTemplate.Create(initialId, "daily", "initial");
        initial.AddQuestion(1, "Oce? swój dzisiejszy nastrój 1/10", 1);
        initial.AddQuestion(2, "Oce? swoje zainteresowanie otoczeniem 1/10", 2);
        initial.AddQuestion(3, "Czy co? sprawi?o Ci dzisiaj rado??? 1/10", 3);
        initial.AddQuestion(4, "Oce? dzi? swoj? energiczno?? 1/10", 4);

        var depression = SurveyTemplate.Create(depId, "daily", "depression");
        depression.AddQuestion(101, "Jak jest twoja dzisiejsza samoocena? 1/10", 1);
        depression.AddQuestion(102, "Jak oceniasz przysz?o??? 1/10", 2);
        depression.AddQuestion(103, "Oce? swoj? potrzeb? snu 1/10", 3);
        depression.AddQuestion(104, "Jak oceniasz swoje dzisiejsze poczucie winy? 1/10", 4);
        depression.AddQuestion(105, "Oce? swoj? ch?? do ?ycia 1/10", 5);
        depression.AddQuestion(106, "Oce? jak ?atwo by?o Ci si? dzisiaj na czym? skupi? 1/10", 6);
        depression.AddQuestion(107, "Oce? swój apetyt 1/10", 7);

        var hypomania = SurveyTemplate.Create(hypoId, "daily", "hypomania");
        hypomania.AddQuestion(201, "Jak jest twoja dzisiejsza samoocena? 1/10", 1);
        hypomania.AddQuestion(202, "Jak oceniasz przysz?o??? 1/10", 2);
        hypomania.AddQuestion(203, "Oce? swoj? potrzeb? snu 1/10", 3);
        hypomania.AddQuestion(204, "Oce? swoje libido 1/10", 4);
        hypomania.AddQuestion(205, "Oce? swoj? gadatliwo?? 1/10", 5);
        hypomania.AddQuestion(206, "Oce? swoj? sk?onno?? do ryzyka 1/10", 6);
        hypomania.AddQuestion(207, "Oce? swoj? spontaniczno?? 1/10", 7);

        var mania = SurveyTemplate.Create(maniaId, "daily", "mania");
        mania.AddQuestion(301, "Jak jest twoja dzisiejsza samoocena? 1/10", 1);
        mania.AddQuestion(302, "Jak oceniasz przysz?o??? 1/10", 2);
        mania.AddQuestion(303, "Oce? swoj? potrzeb? snu 1/10", 3);
        mania.AddQuestion(304, "Oce? swoje libido 1/10", 4);
        mania.AddQuestion(305, "Oce? swoj? gadatliwo?? 1/10", 5);
        mania.AddQuestion(306, "Oce? swoj? sk?onno?? do ryzyka 1/10", 6);
        mania.AddQuestion(307, "Oce? swoj? spontaniczno?? 1/10", 7);

        db.SurveyTemplates.AddRange(initial, depression, hypomania, mania);
        db.SaveChanges();
    }
}