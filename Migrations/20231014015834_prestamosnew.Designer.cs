﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using tallerbiblioteca.Context;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    [DbContext(typeof(BibliotecaDbContext))]
    [Migration("20231014015834_prestamosnew")]
    partial class prestamosnew
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("tallerbiblioteca.Models.Autor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NombreAutor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Autores");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.AutorLibro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Id_autor")
                        .HasColumnType("int");

                    b.Property<int>("Id_libro")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id_autor");

                    b.HasIndex("Id_libro");

                    b.ToTable("AutoresLibros");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Configuracion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id_permiso")
                        .HasColumnType("int");

                    b.Property<int>("Id_rol")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id_permiso");

                    b.HasIndex("Id_rol");

                    b.ToTable("Configuracion");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Ejemplar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EstadoEjemplar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id_libro")
                        .HasColumnType("int");

                    b.Property<string>("Isbn_libro")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Id_libro");

                    b.ToTable("Ejemplares");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Genero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NombreGenero")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genero");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.GeneroLibro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Id_genero")
                        .HasColumnType("int");

                    b.Property<int>("Id_libro")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id_genero");

                    b.HasIndex("Id_libro");

                    b.ToTable("GenerosLibros");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Libro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CantidadLibros")
                        .HasColumnType("int");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagenLibro")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Libros");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Permiso", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permisos");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Peticiones", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaPeticion")
                        .HasColumnType("datetime2");

                    b.Property<int>("Id_ejemplar")
                        .HasColumnType("int");

                    b.Property<int>("Id_usuario")
                        .HasColumnType("int");

                    b.Property<string>("Motivo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Id_ejemplar");

                    b.HasIndex("Id_usuario");

                    b.ToTable("Peticiones");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Prestamo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Fecha_fin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Fecha_inicio")
                        .HasColumnType("datetime2");

                    b.Property<int>("Id_peticion")
                        .HasColumnType("int");

                    b.Property<int>("peticionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("peticionId");

                    b.ToTable("Prestamos");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Rol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Rol");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Contraseña")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id_rol")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Numero_documento")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id_rol");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.AutorLibro", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Autor", "Autor")
                        .WithMany()
                        .HasForeignKey("Id_autor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tallerbiblioteca.Models.Libro", "Libro")
                        .WithMany()
                        .HasForeignKey("Id_libro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Configuracion", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Permiso", "Permiso")
                        .WithMany()
                        .HasForeignKey("Id_permiso")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tallerbiblioteca.Models.Rol", "Rol")
                        .WithMany()
                        .HasForeignKey("Id_rol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permiso");

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Ejemplar", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Libro", "Libro")
                        .WithMany()
                        .HasForeignKey("Id_libro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.GeneroLibro", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Genero", "Genero")
                        .WithMany()
                        .HasForeignKey("Id_genero")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tallerbiblioteca.Models.Libro", "Libro")
                        .WithMany()
                        .HasForeignKey("Id_libro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genero");

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Peticiones", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Ejemplar", "Ejemplar")
                        .WithMany()
                        .HasForeignKey("Id_ejemplar")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tallerbiblioteca.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("Id_usuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ejemplar");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Prestamo", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Peticiones", "peticion")
                        .WithMany()
                        .HasForeignKey("peticionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("peticion");
                });

            modelBuilder.Entity("tallerbiblioteca.Models.Usuario", b =>
                {
                    b.HasOne("tallerbiblioteca.Models.Rol", "Rol")
                        .WithMany()
                        .HasForeignKey("Id_rol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");
                });
#pragma warning restore 612, 618
        }
    }
}
