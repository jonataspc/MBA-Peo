﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Peo.GestaoAlunos.Infra.Data.Contexts;


#nullable disable

namespace Peo.GestaoAlunos.Infra.Data.Migrations
{
    [DbContext(typeof(GestaoEstudantesContext))]
    [Migration("20250515021556_TraducaoPtBr")]
    partial class TraducaoPtBr
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("Peo.Core.Entities.Usuario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("NomeCompleto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Certificado", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Conteudo")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataEmissao")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MatriculaId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<string>("NumeroCertificado")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MatriculaId");

                    b.HasIndex("NumeroCertificado")
                        .IsUnique();

                    b.ToTable("Certificado");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Estudante", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EstaAtivo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UsuarioId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId")
                        .IsUnique();

                    b.ToTable("Estudante");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Matricula", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CursoId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataConclusao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataMatricula")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EstudanteId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<int>("PercentualProgresso")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("PendentePagamento");

                    b.HasKey("Id");

                    b.HasIndex("EstudanteId", "CursoId")
                        .IsUnique();

                    b.ToTable("Matricula");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.ProgressoMatricula", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AulaId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataConclusao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataInicio")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MatriculaId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasPrecision(0)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MatriculaId", "AulaId")
                        .IsUnique();

                    b.ToTable("ProgressoMatricula");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Certificado", b =>
                {
                    b.HasOne("Peo.GestaoAlunos.Domain.Entities.Matricula", "Matricula")
                        .WithMany()
                        .HasForeignKey("MatriculaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Matricula");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Estudante", b =>
                {
                    b.HasOne("Peo.Core.Entities.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Matricula", b =>
                {
                    b.HasOne("Peo.GestaoAlunos.Domain.Entities.Estudante", "Estudante")
                        .WithMany("Matriculas")
                        .HasForeignKey("EstudanteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Estudante");
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.ProgressoMatricula", b =>
                {
                    b.HasOne("Peo.GestaoAlunos.Domain.Entities.Matricula", null)
                        .WithMany()
                        .HasForeignKey("MatriculaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Peo.GestaoAlunos.Domain.Entities.Estudante", b =>
                {
                    b.Navigation("Matriculas");
                });
#pragma warning restore 612, 618
        }
    }
}
