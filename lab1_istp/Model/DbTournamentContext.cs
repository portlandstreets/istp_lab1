//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace tournamentdomain.Model;

//public partial class DbTournamentContext : DbContext
//{
//    public DbTournamentContext()
//    {
//    }

//    public DbTournamentContext(DbContextOptions<DbTournamentContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<Captain> Captains { get; set; }

//    public virtual DbSet<Coach> Coaches { get; set; }

//    public virtual DbSet<Match> Matches { get; set; }

//    public virtual DbSet<Organizer> Organizers { get; set; }

//    public virtual DbSet<Player> Players { get; set; }

//    public virtual DbSet<Sponsor> Sponsors { get; set; }

//    public virtual DbSet<Sponsorship> Sponsorships { get; set; }

//    public virtual DbSet<Statistic> Statistics { get; set; }

//    public virtual DbSet<Team> Teams { get; set; }

//    public virtual DbSet<Tournament> Tournaments { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Tournament_lab1;Username=postgres;Password=123;");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Captain>(entity =>
//        {
//            entity.HasKey(e => e.Nickname).HasName("captain_pkey");

//            entity.ToTable("captain");

//            entity.HasIndex(e => e.TeamName, "captain_team_name_key").IsUnique();

//            entity.Property(e => e.Nickname)
//                .HasMaxLength(50)
//                .HasColumnName("nickname");
//            entity.Property(e => e.TeamName)
//                .HasMaxLength(100)
//                .HasColumnName("team_name");

//            entity.HasOne(d => d.NicknameNavigation).WithOne(p => p.Captain)
//                .HasForeignKey<Captain>(d => d.Nickname)
//                .HasConstraintName("captain_nickname_fkey");

//            entity.HasOne(d => d.TeamNameNavigation).WithOne(p => p.Captain)
//                .HasForeignKey<Captain>(d => d.TeamName)
//                .OnDelete(DeleteBehavior.Cascade)
//                .HasConstraintName("captain_team_name_fkey");
//        });

//        modelBuilder.Entity<Coach>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("coach_pkey");

//            entity.ToTable("coach");

//            entity.Property(e => e.Id).HasColumnName("id");
//            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
//            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
//            entity.Property(e => e.Name)
//                .HasMaxLength(50)
//                .HasColumnName("name");
//            entity.Property(e => e.Salary)
//                .HasPrecision(12, 2)
//                .HasColumnName("salary");
//            entity.Property(e => e.Surname)
//                .HasMaxLength(50)
//                .HasColumnName("surname");
//        });

//        modelBuilder.Entity<Match>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("match_pkey");

//            entity.ToTable("match");

//            entity.Property(e => e.Id).HasColumnName("id");
//            entity.Property(e => e.Duration).HasColumnName("duration");
//            entity.Property(e => e.MatchDate)
//                .HasColumnType("timestamp without time zone")
//                .HasColumnName("match_date");
//            entity.Property(e => e.Stage)
//                .HasMaxLength(50)
//                .HasColumnName("stage");
//            entity.Property(e => e.TournamentName)
//                .HasMaxLength(100)
//                .HasColumnName("tournament_name");
//            entity.Property(e => e.WinnerTeam)
//                .HasMaxLength(100)
//                .HasColumnName("winner_team");

//            entity.HasOne(d => d.TournamentNameNavigation).WithMany(p => p.Matches)
//                .HasForeignKey(d => d.TournamentName)
//                .OnDelete(DeleteBehavior.Cascade)
//                .HasConstraintName("match_tournament_name_fkey");

//            entity.HasOne(d => d.WinnerTeamNavigation).WithMany(p => p.Matches)
//                .HasForeignKey(d => d.WinnerTeam)
//                .HasConstraintName("match_winner_team_fkey");
//        });

//        modelBuilder.Entity<Organizer>(entity =>
//        {
//            entity.HasKey(e => e.Name).HasName("organizer_pkey");

//            entity.ToTable("organizer");

//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//            entity.Property(e => e.Company)
//                .HasMaxLength(100)
//                .HasColumnName("company");
//            entity.Property(e => e.ContactEmail)
//                .HasMaxLength(100)
//                .HasColumnName("contact_email");
//        });

//        modelBuilder.Entity<Player>(entity =>
//        {
//            entity.HasKey(e => e.Nickname).HasName("player_pkey");

//            entity.ToTable("player");

//            entity.Property(e => e.Nickname)
//                .HasMaxLength(50)
//                .HasColumnName("nickname");
//            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
//            entity.Property(e => e.Country)
//                .HasMaxLength(50)
//                .HasColumnName("country");
//            entity.Property(e => e.RealName)
//                .HasMaxLength(100)
//                .HasColumnName("real_name");
//            entity.Property(e => e.Role)
//                .HasMaxLength(50)
//                .HasColumnName("role");
//            entity.Property(e => e.SubstituteFor)
//                .HasMaxLength(50)
//                .HasColumnName("substitute_for");
//            entity.Property(e => e.TeamName)
//                .HasMaxLength(100)
//                .HasColumnName("team_name");

//            entity.HasOne(d => d.SubstituteForNavigation).WithMany(p => p.InverseSubstituteForNavigation)
//                .HasForeignKey(d => d.SubstituteFor)
//                .OnDelete(DeleteBehavior.SetNull)
//                .HasConstraintName("player_substitute_for_fkey");

//            entity.HasOne(d => d.TeamNameNavigation).WithMany(p => p.Players)
//                .HasForeignKey(d => d.TeamName)
//                .OnDelete(DeleteBehavior.SetNull)
//                .HasConstraintName("player_team_name_fkey");
//        });

//        modelBuilder.Entity<Sponsor>(entity =>
//        {
//            entity.HasKey(e => e.Name).HasName("sponsor_pkey");

//            entity.ToTable("sponsor");

//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//            entity.Property(e => e.ContactEmail)
//                .HasMaxLength(100)
//                .HasColumnName("contact_email");
//            entity.Property(e => e.Country)
//                .HasMaxLength(50)
//                .HasColumnName("country");
//            entity.Property(e => e.Type)
//                .HasMaxLength(50)
//                .HasColumnName("type");
//        });

//        modelBuilder.Entity<Sponsorship>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("sponsorship_pkey");

//            entity.ToTable("sponsorship");

//            entity.Property(e => e.Id).HasColumnName("id");
//            entity.Property(e => e.Amount)
//                .HasPrecision(15, 2)
//                .HasColumnName("amount");
//            entity.Property(e => e.EntityName)
//                .HasMaxLength(100)
//                .HasColumnName("entity_name");
//            entity.Property(e => e.EntityType)
//                .HasMaxLength(20)
//                .HasColumnName("entity_type");
//            entity.Property(e => e.SponsorName)
//                .HasMaxLength(100)
//                .HasColumnName("sponsor_name");

//            entity.HasOne(d => d.SponsorNameNavigation).WithMany(p => p.Sponsorships)
//                .HasForeignKey(d => d.SponsorName)
//                .OnDelete(DeleteBehavior.Cascade)
//                .HasConstraintName("sponsorship_sponsor_name_fkey");
//        });

//        modelBuilder.Entity<Statistic>(entity =>
//        {
//            entity.HasKey(e => new { e.MatchId, e.TeamName }).HasName("statistics_pkey");

//            entity.ToTable("statistics");

//            entity.Property(e => e.MatchId).HasColumnName("match_id");
//            entity.Property(e => e.TeamName)
//                .HasMaxLength(100)
//                .HasColumnName("team_name");
//            entity.Property(e => e.Damage)
//                .HasDefaultValue(0)
//                .HasColumnName("damage");
//            entity.Property(e => e.Deaths)
//                .HasDefaultValue(0)
//                .HasColumnName("deaths");
//            entity.Property(e => e.Kills)
//                .HasDefaultValue(0)
//                .HasColumnName("kills");

//            entity.HasOne(d => d.Match).WithMany(p => p.Statistics)
//                .HasForeignKey(d => d.MatchId)
//                .HasConstraintName("statistics_match_id_fkey");

//            entity.HasOne(d => d.TeamNameNavigation).WithMany(p => p.Statistics)
//                .HasForeignKey(d => d.TeamName)
//                .HasConstraintName("statistics_team_name_fkey");
//        });

//        modelBuilder.Entity<Team>(entity =>
//        {
//            entity.HasKey(e => e.Name).HasName("team_pkey");

//            entity.ToTable("team");

//            entity.HasIndex(e => e.CoachId, "team_coach_id_key").IsUnique();

//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//            entity.Property(e => e.CoachId).HasColumnName("coach_id");
//            entity.Property(e => e.Country)
//                .HasMaxLength(50)
//                .HasColumnName("country");
//            entity.Property(e => e.CreationYear).HasColumnName("creation_year");
//            entity.Property(e => e.RankingPoints)
//                .HasDefaultValue(0)
//                .HasColumnName("ranking_points");

//            entity.HasOne(d => d.Coach).WithOne(p => p.Team)
//                .HasForeignKey<Team>(d => d.CoachId)
//                .OnDelete(DeleteBehavior.SetNull)
//                .HasConstraintName("team_coach_id_fkey");
//        });

//        modelBuilder.Entity<Tournament>(entity =>
//        {
//            entity.HasKey(e => e.Name).HasName("tournament_pkey");

//            entity.ToTable("tournament");

//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//            entity.Property(e => e.EndDate).HasColumnName("end_date");
//            entity.Property(e => e.Location)
//                .HasMaxLength(100)
//                .HasColumnName("location");
//            entity.Property(e => e.OrganizerName)
//                .HasMaxLength(100)
//                .HasColumnName("organizer_name");
//            entity.Property(e => e.PrizePool)
//                .HasPrecision(15, 2)
//                .HasColumnName("prize_pool");
//            entity.Property(e => e.StartDate).HasColumnName("start_date");

//            entity.HasOne(d => d.OrganizerNameNavigation).WithMany(p => p.Tournaments)
//                .HasForeignKey(d => d.OrganizerName)
//                .OnDelete(DeleteBehavior.Cascade)
//                .HasConstraintName("tournament_organizer_name_fkey");

//            entity.HasMany(d => d.TeamNames).WithMany(p => p.TournamentNames)
//                .UsingEntity<Dictionary<string, object>>(
//                    "TournamentParticipation",
//                    r => r.HasOne<Team>().WithMany()
//                        .HasForeignKey("TeamName")
//                        .HasConstraintName("tournament_participation_team_name_fkey"),
//                    l => l.HasOne<Tournament>().WithMany()
//                        .HasForeignKey("TournamentName")
//                        .HasConstraintName("tournament_participation_tournament_name_fkey"),
//                    j =>
//                    {
//                        j.HasKey("TournamentName", "TeamName").HasName("tournament_participation_pkey");
//                        j.ToTable("tournament_participation");
//                        j.IndexerProperty<string>("TournamentName")
//                            .HasMaxLength(100)
//                            .HasColumnName("tournament_name");
//                        j.IndexerProperty<string>("TeamName")
//                            .HasMaxLength(100)
//                            .HasColumnName("team_name");
//                    });
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
