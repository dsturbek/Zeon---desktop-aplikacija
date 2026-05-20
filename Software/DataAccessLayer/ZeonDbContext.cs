using EntitiesLayer.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DataAccessLayer
{
    public partial class ZeonDbContext : DbContext
    {
        public ZeonDbContext()
            : base("name=ZeonDbContext1")
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Client_measurement> Client_measurement { get; set; }
        public virtual DbSet<Client_profile> Client_profile { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Exercise> Exercises { get; set; }
        public virtual DbSet<Exercise_Workout> Exercise_Workout { get; set; }
        public virtual DbSet<Feedback_meal> Feedback_meal { get; set; }
        public virtual DbSet<Feedback_training> Feedback_training { get; set; }
        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<Food_diary> Food_diary { get; set; }
        public virtual DbSet<Food_Food_diary> Food_Food_diary { get; set; }
        public virtual DbSet<Food_plan> Food_plan { get; set; }
        public virtual DbSet<Food_Recipe> Food_Recipe { get; set; }
        public virtual DbSet<Goal> Goals { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Personal_record> Personal_record { get; set; }
        public virtual DbSet<Planned_meal> Planned_meal { get; set; }
        public virtual DbSet<Planned_meal_Food> Planned_meal_Food { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Trainer> Trainers { get; set; }
        public virtual DbSet<Trainer_profile> Trainer_profile { get; set; }
        public virtual DbSet<Workout> Workouts { get; set; }
        public virtual DbSet<Workout_plan_assigned> Workout_plan_assigned { get; set; }
        public virtual DbSet<Workout_plan_assigned_Workout> Workout_plan_assigned_Workout { get; set; }
        public virtual DbSet<Workout_plan_template> Workout_plan_template { get; set; }
        public virtual DbSet<Workout_Workout_plan_template> Workout_Workout_plan_template { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .Property(e => e.name_surname)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Client_profile)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Conversations)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Feedback_training)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Food_diary)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Food_plan)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Goals)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Reports)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Workout_plan_assigned)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.SenderClientId);

            modelBuilder.Entity<Client_measurement>()
                .Property(e => e.height)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Client_measurement>()
                .Property(e => e.weight)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Client_profile>()
                .Property(e => e.height)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Client_profile>()
                .Property(e => e.weight)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Client_profile>()
                .Property(e => e.gender)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Client_profile>()
                .HasMany(e => e.Client_measurement)
                .WithOptional(e => e.Client_profile)
                .HasForeignKey(e => e.Client_profileId);

            modelBuilder.Entity<Client_profile>()
                .HasMany(e => e.Personal_record)
                .WithOptional(e => e.Client_profile)
                .HasForeignKey(e => e.Client_profileId);

            modelBuilder.Entity<Conversation>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Conversation)
                .HasForeignKey(e => e.ConversationId);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.exercise_name)
                .IsUnicode(false);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.muscle)
                .IsUnicode(false);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.video_url)
                .IsUnicode(false);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.Exercise_Workout)
                .WithRequired(e => e.Exercise)
                .HasForeignKey(e => e.ExerciseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.Personal_record)
                .WithOptional(e => e.Exercise)
                .HasForeignKey(e => e.ExerciseId);

            modelBuilder.Entity<Exercise_Workout>()
                .Property(e => e.weight)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Exercise_Workout>()
                .HasMany(e => e.Feedback_training)
                .WithOptional(e => e.Exercise_Workout)
                .HasForeignKey(e => new { e.Exercise_WorkoutExerciseId, e.Exercise_WorkoutWorkoutId });

            modelBuilder.Entity<Feedback_meal>()
                .Property(e => e.comment)
                .IsUnicode(false);

            modelBuilder.Entity<Feedback_training>()
                .Property(e => e.feedback_message)
                .IsUnicode(false);

            modelBuilder.Entity<Feedback_training>()
                .Property(e => e.completed_weight)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Food>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Food>()
                .Property(e => e.kCal)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Food>()
                .Property(e => e.proteins)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Food>()
                .Property(e => e.fat)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Food>()
                .Property(e => e.carbohydrates)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Food>()
                .HasMany(e => e.Food_Food_diary)
                .WithRequired(e => e.Food)
                .HasForeignKey(e => e.FoodId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Food>()
                .HasMany(e => e.Food_Recipe)
                .WithRequired(e => e.Food)
                .HasForeignKey(e => e.FoodId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Food>()
                .HasMany(e => e.Planned_meal_Food)
                .WithRequired(e => e.Food)
                .HasForeignKey(e => e.FoodId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Food_diary>()
                .HasMany(e => e.Food_Food_diary)
                .WithRequired(e => e.Food_diary)
                .HasForeignKey(e => e.Food_diaryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Food_Food_diary>()
                .Property(e => e.amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Food_plan>()
                .Property(e => e.food_plan_name)
                .IsUnicode(false);

            modelBuilder.Entity<Food_plan>()
                .Property(e => e.food_plan_description)
                .IsUnicode(false);

            modelBuilder.Entity<Food_plan>()
                .HasMany(e => e.Planned_meal)
                .WithOptional(e => e.Food_plan)
                .HasForeignKey(e => e.Food_planId);

            modelBuilder.Entity<Food_Recipe>()
                .Property(e => e.amount_grams)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Goal>()
                .Property(e => e.goal_name)
                .IsUnicode(false);

            modelBuilder.Entity<Goal>()
                .Property(e => e.goal_description)
                .IsUnicode(false);

            modelBuilder.Entity<Message>()
                .Property(e => e.message_content)
                .IsUnicode(false);

            modelBuilder.Entity<Personal_record>()
                .Property(e => e.max_weight)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Personal_record>()
                .Property(e => e.max_volument)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Planned_meal>()
                .Property(e => e.meal_type)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Planned_meal>()
                .HasMany(e => e.Feedback_meal)
                .WithOptional(e => e.Planned_meal)
                .HasForeignKey(e => e.Planned_mealId);

            modelBuilder.Entity<Planned_meal>()
                .HasMany(e => e.Planned_meal_Food)
                .WithRequired(e => e.Planned_meal)
                .HasForeignKey(e => e.Planned_mealId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.recipe_name)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.recipe_description)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.instructions)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.Food_Recipe)
                .WithRequired(e => e.Recipe)
                .HasForeignKey(e => e.RecipeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Report>()
                .Property(e => e.report_name)
                .IsUnicode(false);

            modelBuilder.Entity<Report>()
                .Property(e => e.report_content)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer>()
                .Property(e => e.name_surname)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer>()
                .Property(e => e.specialization)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer>()
                .HasMany(e => e.Clients)
                .WithOptional(e => e.Trainer)
                .HasForeignKey(e => e.TrainerId);

            modelBuilder.Entity<Trainer>()
                .HasMany(e => e.Conversations)
                .WithOptional(e => e.Trainer)
                .HasForeignKey(e => e.TrainerId);

            modelBuilder.Entity<Trainer>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Trainer)
                .HasForeignKey(e => e.SenderTrainerId);

            modelBuilder.Entity<Trainer>()
                .HasMany(e => e.Reports)
                .WithOptional(e => e.Trainer)
                .HasForeignKey(e => e.TrainerId);

            modelBuilder.Entity<Trainer>()
                .HasMany(e => e.Trainer_profile)
                .WithOptional(e => e.Trainer)
                .HasForeignKey(e => e.TrainerId);

            modelBuilder.Entity<Trainer>()
                .HasMany(e => e.Workout_plan_template)
                .WithOptional(e => e.Trainer)
                .HasForeignKey(e => e.TrainerId);

            modelBuilder.Entity<Trainer_profile>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Trainer_profile>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Trainer_profile>()
                .Property(e => e.rating)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Workout>()
                .Property(e => e.workout_name)
                .IsUnicode(false);

            modelBuilder.Entity<Workout>()
                .Property(e => e.muscle_group)
                .IsUnicode(false);

            modelBuilder.Entity<Workout>()
                .HasMany(e => e.Exercise_Workout)
                .WithRequired(e => e.Workout)
                .HasForeignKey(e => e.WorkoutId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workout>()
                .HasMany(e => e.Workout_plan_assigned_Workout)
                .WithRequired(e => e.Workout)
                .HasForeignKey(e => e.WorkoutId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workout>()
                .HasMany(e => e.Workout_Workout_plan_template)
                .WithRequired(e => e.Workout)
                .HasForeignKey(e => e.WorkoutId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workout_plan_assigned>()
                .Property(e => e.workout_assigned_name)
                .IsUnicode(false);

            modelBuilder.Entity<Workout_plan_assigned>()
                .HasMany(e => e.Workout_plan_assigned_Workout)
                .WithRequired(e => e.Workout_plan_assigned)
                .HasForeignKey(e => e.Workout_plan_assignedId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workout_plan_template>()
                .Property(e => e.workout_template_name)
                .IsUnicode(false);

            modelBuilder.Entity<Workout_plan_template>()
                .HasMany(e => e.Workout_plan_assigned)
                .WithOptional(e => e.Workout_plan_template)
                .HasForeignKey(e => e.Workout_plan_templateId);

            modelBuilder.Entity<Workout_plan_template>()
                .HasMany(e => e.Workout_Workout_plan_template)
                .WithRequired(e => e.Workout_plan_template)
                .HasForeignKey(e => e.Workout_plan_templateId)
                .WillCascadeOnDelete(false);
        }
    }
}
