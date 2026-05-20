namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Feedback_training
    {
        [Key]
        public int id_feedback { get; set; }

        [StringLength(255)]
        public string feedback_message { get; set; }

        public int? completed_sets { get; set; }

        public int? completed_reps { get; set; }

        public decimal? completed_weight { get; set; }

        public int? ClientId { get; set; }

        public int? Exercise_WorkoutExerciseId { get; set; }

        public int? Exercise_WorkoutWorkoutId { get; set; }

        public virtual Client Client { get; set; }

        public virtual Exercise_Workout Exercise_Workout { get; set; }
    }
}
