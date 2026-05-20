using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class ReportService
    {
        private ReportRepository reportRepository;

        public ReportService()
        {
            reportRepository = new ReportRepository();
        }

        public async Task<List<Client>> GetClientsByTrainerId(int trainerId)
        {
            if (trainerId <= 0)
                throw new Exception("ID trenera nije ispravan!");

            return await reportRepository.FindAllByTrainerId(trainerId);
        }

        public bool Validate(int clientId, DateTime startDate, DateTime endDate)
        {
            if (clientId <= 0) return false;
            if (startDate > endDate) return false;
            return true;
        }

        public async Task<TrainingReportDTO> GenerateReport(int clientId, DateTime startDate, DateTime endDate)
        {
            if (!Validate(clientId, startDate, endDate))
                throw new Exception("Neispravan unos — odaberi klijenta i valjan vremenski period.");

            var measurements = await reportRepository.GetMeasurements(clientId, startDate, endDate);

            var personalRecords = await reportRepository.GetPersonalRecords(clientId);

            var trainingData = await reportRepository.GetTrainingData(clientId, startDate, endDate);

            var dto = new TrainingReportDTO();

            foreach (var m in measurements)
            {
                if (m.measurement_date.HasValue)
                    dto.WeightDates.Add(m.measurement_date.Value);
                dto.WeightValues.Add((double)(m.weight ?? 0));
            }

            dto.TotalTrainings = trainingData.Count;
            dto.PersonalRecords = personalRecords;

            foreach (var fb in trainingData)
            {
                dto.PlannedWeights.Add((double)(fb.Exercise_Workout?.weight ?? 0));
                dto.ActualWeights.Add((double)(fb.completed_weight ?? 0));
                dto.TrainingLabels.Add(fb.Exercise_Workout?.Exercise?.exercise_name ?? "—");
            }

            return dto;
        }
    }
}
