using System;
using System.Collections.Generic;
using IpDeputyApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    [DbContext(typeof(IpDeputyDbContext))]
    [Migration("20261024102345_AddTelegramIdUniqueIndex")]
    partial class AddTelegramIdUniqueIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("IpDeputyApi.Database.Models.AdditionalCouple", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("Cabinet")
                        .HasColumnType("text");

                    b.Property<int?>("CoupleTimeId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("DayOfWeekId")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<int?>("SubgroupId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("TeacherId")
                        .HasColumnType("integer");

                    b.Property<TimeOnly?>("Time")
                        .HasColumnType("time without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CoupleTimeId");

                    b.HasIndex("DayOfWeekId");

                    b.HasIndex("SubgroupId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("SubjectTypeId");

                    b.HasIndex("TeacherId");

                    b.ToTable("AdditionalCouples");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Couple", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("Cabinet")
                        .HasColumnType("text");

                    b.Property<int>("CoupleTimeId")
                        .HasColumnType("integer");

                    b.Property<int>("DayOfWeekId")
                        .HasColumnType("integer");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsRolling")
                        .HasColumnType("boolean");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<DateOnly?>("StartDate")
                        .HasColumnType("date");

                    b.Property<int?>("SubgroupId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("TeacherId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CoupleTimeId");

                    b.HasIndex("DayOfWeekId");

                    b.HasIndex("SubgroupId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("SubjectTypeId");

                    b.HasIndex("TeacherId");

                    b.ToTable("Couples");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.CoupleDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CoupleId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<bool>("IsRemovedDate")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CoupleId");

                    b.ToTable("CoupleDates");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.CoupleTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("TimeEnd")
                        .HasColumnType("time without time zone");

                    b.Property<TimeOnly>("TimeStart")
                        .HasColumnType("time without time zone");

                    b.HasKey("Id");

                    b.ToTable("CoupleTimes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Index = 1,
                            TimeEnd = new TimeOnly(10, 20, 0),
                            TimeStart = new TimeOnly(9, 0, 0)
                        },
                        new
                        {
                            Id = 2,
                            Index = 2,
                            TimeEnd = new TimeOnly(11, 50, 0),
                            TimeStart = new TimeOnly(10, 30, 0)
                        },
                        new
                        {
                            Id = 3,
                            Index = 3,
                            TimeEnd = new TimeOnly(13, 30, 0),
                            TimeStart = new TimeOnly(12, 10, 0)
                        },
                        new
                        {
                            Id = 4,
                            Index = 4,
                            TimeEnd = new TimeOnly(15, 0, 0),
                            TimeStart = new TimeOnly(13, 40, 0)
                        },
                        new
                        {
                            Id = 5,
                            Index = 5,
                            TimeEnd = new TimeOnly(16, 30, 0),
                            TimeStart = new TimeOnly(15, 10, 0)
                        },
                        new
                        {
                            Id = 6,
                            Index = 6,
                            TimeEnd = new TimeOnly(18, 0, 0),
                            TimeStart = new TimeOnly(16, 40, 0)
                        },
                        new
                        {
                            Id = 7,
                            Index = 7,
                            TimeEnd = new TimeOnly(19, 30, 0),
                            TimeStart = new TimeOnly(18, 10, 0)
                        });
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.DayOfWeek", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DayOfWeeks");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Index = 1,
                            Name = "Понеділок"
                        },
                        new
                        {
                            Id = 2,
                            Index = 2,
                            Name = "Вівторок"
                        },
                        new
                        {
                            Id = 3,
                            Index = 3,
                            Name = "Середа"
                        },
                        new
                        {
                            Id = 4,
                            Index = 4,
                            Name = "Четвер"
                        },
                        new
                        {
                            Id = 5,
                            Index = 5,
                            Name = "П'ятниця"
                        },
                        new
                        {
                            Id = 6,
                            Index = 6,
                            Name = "Субота"
                        },
                        new
                        {
                            Id = 7,
                            Index = 0,
                            Name = "Неділя"
                        });
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Link", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Birthday")
                        .HasColumnType("date");

                    b.Property<string>("ContactPhone")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FitEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("SubgroupId")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TelegramNickname")
                        .HasColumnType("text");

                    b.Property<string>("TelegramPhone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SubgroupId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Subgroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subgroups");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LaboratoryCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PracticalCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubjectType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SubjectTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Лабораторне",
                            ShortName = "Лаб."
                        },
                        new
                        {
                            Id = 2,
                            Name = "Практика",
                            ShortName = "Пр."
                        },
                        new
                        {
                            Id = 3,
                            Name = "Лекція",
                            ShortName = "Лек."
                        },
                        new
                        {
                            Id = 4,
                            Name = "Семінар",
                            ShortName = "Сем."
                        },
                        new
                        {
                            Id = 5,
                            Name = "Консультація",
                            ShortName = "Конс."
                        },
                        new
                        {
                            Id = 6,
                            Name = "Залік",
                            ShortName = "Зал."
                        },
                        new
                        {
                            Id = 7,
                            Name = "Екзамен",
                            ShortName = "Екз."
                        },
                        new
                        {
                            Id = 8,
                            Name = "Додаткова",
                            ShortName = "Дод."
                        });
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionStudent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("PreferredPosition")
                        .HasColumnType("integer");

                    b.Property<int>("StudentId")
                        .HasColumnType("integer");

                    b.Property<int>("SubmissionWorkId")
                        .HasColumnType("integer");

                    b.Property<int>("SubmissionsConfigId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("SubmittedAt")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("SubmissionWorkId");

                    b.HasIndex("SubmissionsConfigId");

                    b.ToTable("SubmissionStudents");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionWork", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SubmissionsConfigId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubmissionsConfigId");

                    b.ToTable("SubmissionWorks");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionsConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CustomName")
                        .HasColumnType("text");

                    b.Property<string>("CustomType")
                        .HasColumnType("text");

                    b.Property<int?>("SubgroupId")
                        .HasColumnType("integer");

                    b.Property<int?>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<int?>("SubjectTypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubgroupId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("SubjectTypeId");

                    b.ToTable("SubmissionsConfigs");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Teacher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ContactPhone")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FitEmail")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TelegramNickname")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Telegram", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastActivity")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly?>("LastCongratulations")
                        .HasColumnType("date");

                    b.Property<bool>("ScheduleCompact")
                        .HasColumnType("boolean");

                    b.Property<int>("StudentId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TelegramId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("StudentId")
                        .IsUnique();

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.ToTable("Telegrams");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LegacyPasswordSalt")
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.PrimitiveCollection<List<string>>("Permissions")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<int?>("StudentId")
                        .HasColumnType("integer");

                    b.Property<int>("TokenVersion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("StudentId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.AdditionalCouple", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.CoupleTime", "CoupleTime")
                        .WithMany()
                        .HasForeignKey("CoupleTimeId");

                    b.HasOne("IpDeputyApi.Database.Models.DayOfWeek", "DayOfWeek")
                        .WithMany("AdditionalCouples")
                        .HasForeignKey("DayOfWeekId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.Subgroup", "Subgroup")
                        .WithMany()
                        .HasForeignKey("SubgroupId");

                    b.HasOne("IpDeputyApi.Database.Models.Subject", "Subject")
                        .WithMany("AdditionalCouples")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.SubjectType", "SubjectType")
                        .WithMany("AdditionalCouples")
                        .HasForeignKey("SubjectTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.Teacher", "Teacher")
                        .WithMany("AdditionalCouples")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CoupleTime");

                    b.Navigation("DayOfWeek");

                    b.Navigation("Subgroup");

                    b.Navigation("Subject");

                    b.Navigation("SubjectType");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Couple", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.CoupleTime", "CoupleTime")
                        .WithMany("Couples")
                        .HasForeignKey("CoupleTimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.DayOfWeek", "DayOfWeek")
                        .WithMany("Couples")
                        .HasForeignKey("DayOfWeekId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.Subgroup", "Subgroup")
                        .WithMany()
                        .HasForeignKey("SubgroupId");

                    b.HasOne("IpDeputyApi.Database.Models.Subject", "Subject")
                        .WithMany("Couples")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.SubjectType", "SubjectType")
                        .WithMany("Couples")
                        .HasForeignKey("SubjectTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.Teacher", "Teacher")
                        .WithMany("Couples")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CoupleTime");

                    b.Navigation("DayOfWeek");

                    b.Navigation("Subgroup");

                    b.Navigation("Subject");

                    b.Navigation("SubjectType");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.CoupleDate", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.Couple", "Couple")
                        .WithMany("CoupleDates")
                        .HasForeignKey("CoupleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Couple");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Student", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.Subgroup", "Subgroup")
                        .WithMany("Students")
                        .HasForeignKey("SubgroupId");

                    b.Navigation("Subgroup");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionStudent", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.SubmissionWork", "SubmissionWork")
                        .WithMany("SubmissionStudents")
                        .HasForeignKey("SubmissionWorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IpDeputyApi.Database.Models.SubmissionsConfig", "SubmissionsConfig")
                        .WithMany("SubmissionStudents")
                        .HasForeignKey("SubmissionsConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");

                    b.Navigation("SubmissionWork");

                    b.Navigation("SubmissionsConfig");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionWork", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.SubmissionsConfig", "SubmissionsConfig")
                        .WithMany("SubmissionWorks")
                        .HasForeignKey("SubmissionsConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubmissionsConfig");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionsConfig", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.Subgroup", "Subgroup")
                        .WithMany("SubmissionsConfigs")
                        .HasForeignKey("SubgroupId");

                    b.HasOne("IpDeputyApi.Database.Models.Subject", "Subject")
                        .WithMany("SubmissionsConfigs")
                        .HasForeignKey("SubjectId");

                    b.HasOne("IpDeputyApi.Database.Models.SubjectType", "SubjectType")
                        .WithMany("SubmissionsConfigs")
                        .HasForeignKey("SubjectTypeId");

                    b.Navigation("Subgroup");

                    b.Navigation("Subject");

                    b.Navigation("SubjectType");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Telegram", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.Student", "Student")
                        .WithOne("Telegram")
                        .HasForeignKey("IpDeputyApi.Database.Models.Telegram", "StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.User", b =>
                {
                    b.HasOne("IpDeputyApi.Database.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Student");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Couple", b =>
                {
                    b.Navigation("CoupleDates");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.CoupleTime", b =>
                {
                    b.Navigation("Couples");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.DayOfWeek", b =>
                {
                    b.Navigation("AdditionalCouples");

                    b.Navigation("Couples");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Student", b =>
                {
                    b.Navigation("Telegram");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Subgroup", b =>
                {
                    b.Navigation("Students");

                    b.Navigation("SubmissionsConfigs");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Subject", b =>
                {
                    b.Navigation("AdditionalCouples");

                    b.Navigation("Couples");

                    b.Navigation("SubmissionsConfigs");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubjectType", b =>
                {
                    b.Navigation("AdditionalCouples");

                    b.Navigation("Couples");

                    b.Navigation("SubmissionsConfigs");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionWork", b =>
                {
                    b.Navigation("SubmissionStudents");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.SubmissionsConfig", b =>
                {
                    b.Navigation("SubmissionStudents");

                    b.Navigation("SubmissionWorks");
                });

            modelBuilder.Entity("IpDeputyApi.Database.Models.Teacher", b =>
                {
                    b.Navigation("AdditionalCouples");

                    b.Navigation("Couples");
                });
#pragma warning restore 612, 618
        }
    }
}
