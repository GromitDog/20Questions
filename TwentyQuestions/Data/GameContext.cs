using Microsoft.EntityFrameworkCore;
using TwentyQuestions.Models;

namespace TwentyQuestions.Data;

public class GameContext :  DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options) { }

    public DbSet<GameState> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameState>()
            .HasMany(g => g.Questions)
            .WithOne()
            .HasForeignKey(q => q.GameStateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasKey(q => q.QuestionId);

        modelBuilder.Entity<Question>()
            .Property(q => q.QuestionId)
            .ValueGeneratedNever(); // We generate GUIDs in code, not in DB
    }
}